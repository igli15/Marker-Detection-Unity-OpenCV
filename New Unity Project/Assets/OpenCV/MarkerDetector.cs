using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Aruco;
using System;
using System.Threading;

public class MarkerDetector : MonoBehaviour
{
    public WebCamera webCamera;

    //Events thrown each frame holding all ids found/lost
    public static event Action<int[]> OnMarkersDetected;
    public static event Action<int[]> OnMarkersLost;

    public Camera cam;
    public PredefinedDictionaryName markerDictionaryType;
    [SerializeField] private bool doCornerRefinement = true;

    public CalibrationData calibrationData;
    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    private Mat grayedImg = new Mat();
    private Mat img;

    private Dictionary<int, MarkerBehaviour> allDetectedMarkers = new Dictionary<int, MarkerBehaviour>();
    private List<int> lostIds = new List<int>();

    private Point2f[][] corners;
    private int[] ids;
    private Point2f[][] rejectedImgPoints;

    private Thread detectMarkersThread;

    private int imgCols;
    private int imgRows;

    bool updateThread = false;
    
    protected void Start()
    {
        Init();
        
        DetectMarkerAsync();
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
        img = ARucoUnityHelper.TextureToMat(input, textureParameters);

        imgRows = img.Rows;
        imgCols = img.Cols;
        
        Cv2.CvtColor(img, grayedImg, ColorConversionCodes.BGR2GRAY);
        
        updateThread = true;
        //DetectMarkerAsync();

        output = ARucoUnityHelper.MatToTexture(img, output);

        img.Release();
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

    private void CheckIfLostMarkers(int[] ids)
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

    private void DetectMarkers()
    {
        while (true)
        {
            Debug.Log("updating thread....");
            
            if (grayedImg.IsDisposed || !updateThread)
            {
                //we skip updating the thread when not needed and also avoids memory exceptions when we disable the 
                //mono behaviour or we haven't updated the main thread yet!
                
                // Debug.Log("grayed img was disposed");
                continue;
            }
            
            CvAruco.DetectMarkers(grayedImg, dictionary, out corners, out ids, detectorParameters,
                out rejectedImgPoints);

            if (ids == null) return;
            //Debug.Log(ids.Length);
            //CvAruco.DrawDetectedMarkers(img, corners, ids);

            CheckIfLostMarkers(ids);

            //Debug.Log(ids.Length);

            //NOTE: sometimes it seems that there are markers detected even though they are not on screen?!
            if (ids.Length > 0 && OnMarkersDetected != null)
            {
                OnMarkersDetected.Invoke(ids);
            }

            for (int i = 0; i < ids.Length; i++)
            {
                Cv2.CornerSubPix(grayedImg, corners[i], new Size(5, 5), new Size(-1, -1), TermCriteria.Both(30, 0.1));

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
    }
}