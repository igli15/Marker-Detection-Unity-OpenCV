using OpenCvSharp;
using OpenCvSharp.Aruco;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MarkerDetector : MonoBehaviour
{
    public WebCamera webCamera;

    //Events thrown each frame holding all ids found/lost
    public static event Action<int[]> OnMarkersDetected;
    public static event Action<int[]> OnMarkersLost;

    public Camera cam;
    public PredefinedDictionaryName markerDictionaryType;
    [SerializeField] private bool doCornerRefinement = true;
    public bool debugMode = false;

    public CalibrationData calibrationData;
    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    private Mat grayedImg = new Mat();
    private Mat img = new Mat();
    private Mat imgBuffer;

    private Dictionary<int, MarkerBehaviour> allDetectedMarkers = new Dictionary<int, MarkerBehaviour>();
    private List<int> lostIds = new List<int>();

    private Point2f[][] corners;
    private int[] ids;

    private Point2f[][] cornersCache;
    private int[] idsCache;

    private Point2f[][] rejectedImgPoints;

    private Thread detectMarkersThread;

    private int imgCols;
    private int imgRows;

    bool updateThread = false;

    private int threadCounter = 0;
    private bool outputImage = false;

    private Semaphore threadSemaphore = new Semaphore(1, 1);

    protected void Start()
    {
        Init();

        DetectMarkerAsync();
        //detectMarkersThread.Priority = ThreadPriority.Highest;
    }

    private void OnEnable()
    {
        webCamera.OnProcessTexture += ProcessTexture;
    }

    private void OnDisable()
    {
        webCamera.OnProcessTexture -= ProcessTexture;
        updateThread = false;
        if (!img.IsDisposed) img.Release();
        if (!imgBuffer.IsDisposed) imgBuffer.Release();
        if (!grayedImg.IsDisposed) grayedImg.Release();
    }

    void Init()
    {
        detectorParameters = DetectorParameters.Create();

        detectorParameters.DoCornerRefinement = doCornerRefinement;
        //detectorParameters.CornerRefinementMinAccuracy = 0.01f;

        dictionary = CvAruco.GetPredefinedDictionary(markerDictionaryType);
    }

    // Our sketch generation function
    private bool ProcessTexture(WebCamTexture input, ref Texture2D output,
        ARucoUnityHelper.TextureConversionParams textureParameters)
    {
        imgBuffer = ARucoUnityHelper.TextureToMat(input, textureParameters);

        imgRows = imgBuffer.Rows;
        imgCols = imgBuffer.Cols;

        // Cv2.CvtColor(img, grayedImg, ColorConversionCodes.BGR2GRAY);

        //DetectMarkerAsync();

        if (idsCache != null)
        {
            corners = cornersCache;
            ids = idsCache;
        }

        //Only draw the markers on debug mode and if on debug dont throw events so we can see the drawn markers.
        if (ids != null)
        {
            if (debugMode)
            {
                CvAruco.DrawDetectedMarkers(imgBuffer, corners, ids);

                //lose all markers
                foreach (MarkerBehaviour lostMarker in allDetectedMarkers.Values)
                {
                    lostMarker.OnMarkerLost.Invoke();
                }

                allDetectedMarkers.Clear();
            }
            else
            {
                CheckIfLostMarkers();
                CheckIfDetectedMarkers();
            }
        }

        updateThread = true;

        /*
        if (threadSemaphore.WaitOne())
        {
            if (img != null)
            {
                output = ARucoUnityHelper.MatToTexture(img, output);
                img.Release();
            }

            threadSemaphore.Release();
        }
        else
        {
            output = ARucoUnityHelper.MatToTexture(imgBuffer, output);
            imgBuffer.Release();
            //threadSemaphore.Release();
        }
*/

        if (threadCounter == 0)
        {
            imgBuffer.CopyTo(img);
            Interlocked.Increment(ref threadCounter);
          
        }
       
        if(outputImage)
        {
            output = ARucoUnityHelper.MatToTexture(img, output);
            outputImage = false;
        }
        else
        {
            output = ARucoUnityHelper.MatToTexture(imgBuffer, output);
        }
        
        imgBuffer.Release();
        return true;
    }


    private void DetectMarkerAsync()
    {
        if (detectMarkersThread == null || !detectMarkersThread.IsAlive)
        {
            detectMarkersThread = new Thread(DetectMarkers);
            detectMarkersThread.Start();
        }
    }

    private void CheckIfLostMarkers()
    {
        if (ids.Length == 0)
        {
            foreach (MarkerBehaviour lostMarker in allDetectedMarkers.Values)
            {
                lostMarker.OnMarkerLost.Invoke();
            }

            allDetectedMarkers.Clear();
        }
        else
        {
            foreach (int id in allDetectedMarkers.Keys)
            {
                int idNotFound = -1;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (id != ids[i])
                    {
                        idNotFound = id;
                    }
                    else
                    {
                        idNotFound = -1;
                        break;
                    }
                }

                if (idNotFound >= 0)
                {
                    allDetectedMarkers[idNotFound].OnMarkerLost.Invoke();
                    lostIds.Add(idNotFound);
                }
            }
        }

        OnMarkersLost?.Invoke(lostIds.ToArray());

        foreach (int i in lostIds)
        {
            allDetectedMarkers.Remove(i);
        }

        lostIds.Clear();
    }

    private void CheckIfDetectedMarkers()
    {
        if (ids.Length > 0 && OnMarkersDetected != null)
        {
            OnMarkersDetected.Invoke(ids);
        }

        for (int i = 0; i < ids.Length; i++)
        {
            //Cv2.CornerSubPix(grayedImg, corners[i], new Size(5, 5), new Size(-1, -1), TermCriteria.Both(30, 0.1));

            if (!MarkerManager.IsMarkerRegistered(ids[i]))
            {
                continue;
            }

            MarkerBehaviour m = MarkerManager.GetMarker(ids[i]);

            if (!allDetectedMarkers.ContainsKey(ids[i]))
            {
                m.OnMarkerDetected.Invoke();
                allDetectedMarkers.Add(m.GetMarkerID(), m);
            }

            // m.UpdateMarker(img.Cols, img.Rows, corners[i], rejectedImgPoints[i]);
            m.UpdateMarker(imgCols, imgRows, corners[i], calibrationData.GetCameraMatrix(),
                calibrationData.GetDistortionCoefficients(), grayedImg);
        }
    }

    private void DetectMarkers()
    {
        //Debug.Log(elapsed);
        while (true)
        {
            if (!updateThread)
            {
                //we skip updating the thread when not needed and also avoids memory exceptions when we disable the 
                //mono behaviour or we haven't updated the main thread yet!

                // Debug.Log("grayed img was disposed");
                continue;
            }

            //threadSemaphore.WaitOne();

            if (threadCounter > 0)
            {
                Cv2.CvtColor(img, grayedImg, ColorConversionCodes.BGR2GRAY);

                CvAruco.DetectMarkers(grayedImg, dictionary, out cornersCache, out idsCache, detectorParameters,
                    out rejectedImgPoints);
                //threadSemaphore.Release();
                outputImage = true;
                Interlocked.Exchange(ref threadCounter, 0);
            }
        }
    }

    private void GrayScaleImage()
    {
        while (true)
        {
            if (!updateThread) continue;

            threadSemaphore.WaitOne();
            Cv2.CvtColor(img, grayedImg, ColorConversionCodes.BGR2GRAY);
            threadSemaphore.Release();
        }
    }

    public void ToggleDebugMode()
    {
        debugMode = !debugMode;
    }
}