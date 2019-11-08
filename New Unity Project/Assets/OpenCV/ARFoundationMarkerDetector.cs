using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ThreadPriority = System.Threading.ThreadPriority;

public class ARFoundationMarkerDetector : MonoBehaviour
{
    public ARCameraManager cameraManager;

    private Texture2D texture;

    //Events thrown each frame holding all ids found/lost
    public static event Action<int[]> OnMarkersDetected;
    public static event Action<int[]> OnMarkersLost;

    public PredefinedDictionaryName markerDictionaryType;
    [SerializeField] private bool doCornerRefinement = true;
    public bool throwMarkerCallbacks = true;
    public bool drawMarkerOutlines = false;

    public CalibrationData calibrationData;
    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    private Mat grayedImg = new Mat();

    //private Mat notRotated = new Mat();
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

    bool updateThread = false;

    private int threadCounter = 0;
    private bool outputImage = false;

    ARucoUnityHelper.TextureConversionParams texParam;
    private XRCameraIntrinsics cameraIntrinsics;
    
    public RawImage displayRawImage;

    // Start is called before the first frame update
    void Start()
    {
        Init();

        DetectMarkerAsync();

        cameraManager.frameReceived += OnCameraFrameReceived;

        //cameraManager.subsystem.currentConfiguration = config;
    }

    void Init()
    {
        detectorParameters = DetectorParameters.Create();

        detectorParameters.DoCornerRefinement = doCornerRefinement;
        //detectorParameters.CornerRefinementMinAccuracy = 0.01f;

        dictionary = CvAruco.GetPredefinedDictionary(markerDictionaryType);

        texParam = new ARucoUnityHelper.TextureConversionParams();
    }

    private void DetectMarkerAsync()
    {
        if (detectMarkersThread == null || !detectMarkersThread.IsAlive)
        {
            Debug.Log("Starting Thread");
            detectMarkersThread = new Thread(DetectMarkers);
            detectMarkersThread.Start();
        }
    }

    private void DetectMarkers()
    {
        while (true)
        {
            Debug.Log("Updating");
            
            if (!updateThread)
            {
                //we skip updating the thread when not needed and also avoids memory exceptions when we disable the 
                //mono behaviour or we haven't updated the main thread yet!
                
                Debug.Log("SKIPPED");
                continue;
            }

            if (threadCounter > 0)
            {
                Debug.Log("Detecting Markers");
                Cv2.CvtColor(img, grayedImg, ColorConversionCodes.BGR2GRAY);

                CvAruco.DetectMarkers(grayedImg, dictionary, out corners, out ids, detectorParameters,
                    out rejectedImgPoints);

                if (throwMarkerCallbacks)
                {
                    CheckIfLostMarkers();
                    CheckIfDetectedMarkers();
                }

                outputImage = true;
                Interlocked.Exchange(ref threadCounter, 0);
            }
        }
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        XRCameraImage image;
        if (!cameraManager.TryGetLatestImage(out image))
        {
            return;
        }

        var format = TextureFormat.RGBA32;

        if (texture == null || texture.width != image.width || texture.height != image.height)
        {
            texture = new Texture2D(image.width, image.height, format, false);
        }

        var conversionParams = new XRCameraImageConversionParams(image, format, CameraImageTransformation.MirrorY);

        var rawTextureData = texture.GetRawTextureData<byte>();
        try
        {
            image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        }
        finally
        {
            image.Dispose();
        }

        texture.Apply();

        texParam.FlipHorizontally = false;

        imgBuffer = ARucoUnityHelper.TextureToMat(texture, texParam);

        //rotate(ref notRotated,ref imgBuffer, 90);

        if (threadCounter == 0)
        {
            //Debug.Log("Incrementing thread counter");
            imgBuffer.CopyTo(img);
            Interlocked.Increment(ref threadCounter);
        }

        //Debug.Log("ThreadCounter: " + threadCounter);
        updateThread = true;
        
        displayRawImage.texture = ARucoUnityHelper.MatToTexture(imgBuffer,texture);
        imgBuffer.Release();
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
                Debug.Log("FOUND MARKER: " + m.GetMarkerID());
                m.OnMarkerDetected.Invoke();
                allDetectedMarkers.Add(m.GetMarkerID(), m);
            }

            cameraManager.TryGetIntrinsics(out cameraIntrinsics);

            float rotZ = 0;
            
            switch (Screen.orientation) {
                case ScreenOrientation.Portrait:
                    rotZ = 90;
                    break;
                case ScreenOrientation.LandscapeLeft:
                    rotZ = 180;
                    break;
                case ScreenOrientation.LandscapeRight:
                    rotZ = 0;
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    rotZ = -90;
                    break;
            }

            // m.UpdateMarker(img.Cols, img.Rows, corners[i], rejectedImgPoints[i]);
            m.UpdateMarker(img.Rows, img.Cols, corners[i], calibrationData.GetCameraMatrix(),
                calibrationData.GetDistortionCoefficients(), cameraIntrinsics, grayedImg, Vector3.forward * rotZ);
        }
    }

    public void ToggleDebugMode()
    {
        if (ids != null)
        {
            foreach (MarkerBehaviour lostMarker in allDetectedMarkers.Values)
            {
                lostMarker.OnMarkerLost.Invoke();
            }

            allDetectedMarkers.Clear();
        }

        throwMarkerCallbacks = !throwMarkerCallbacks;
        drawMarkerOutlines = !drawMarkerOutlines;
    }
}