using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class AbstractMarkerDetector : MonoBehaviour
{
    [Serializable]
    public struct DetectionParameters
    {
        public int adaptiveThreshWinSizeMin; //3
        public int adaptiveThreshWinSizeMax; //23
        public int adaptiveThreshWinSizeStep; //10
        public double adaptiveThreshConstant; //7
        public double minMarkerPerimeterRate; //0.03
        public double maxMarkerPerimeterRate; //4.0
        public double polygonalApproxAccuracyRate; //0.03
        public double minCornerDistanceRate; //0.05
        public int minDistanceToBorder; //3
        public double minMarkerDistanceRate; //0.05
        public int cornerRefinementWinSize; //5
        public int cornerRefinementMaxIterations; //30
        public double cornerRefinementMinAccuracy; //0.1
        public int markerBorderBits; //1
        public int perspectiveRemovePixelPerCell; //8
        public double perspectiveRemoveIgnoredMarginPerCell; //0.13
        public double maxErroneousBitsInBorderRate; //0.35
        public double minOtsuStdDev; //5.0
        public double errorCorrectionRate; //0.6
        
    }
    public static event Action<int[]> OnMarkersDetected;
    public static event Action<int[]> OnMarkersLost;

    public static Action<int[]> OnMarkerDetectionPaused;

    public PredefinedDictionaryName markerDictionaryType;
    [SerializeField] private bool doCornerRefinement = true;
    public bool throwMarkerCallbacks = true;
    public float markerDetectorPauseTime = 0;

    [FormerlySerializedAs("detectionParams")] public DetectionParameters detectionParamsStruct;
    
    [Sirenix.OdinInspector.ReadOnly] [SerializeField]
    protected float timeCount = 0;
    
    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    protected Mat grayedImg = new Mat();

    //private Mat notRotated = new Mat();
    protected Mat img = new Mat();
    protected Mat imgBuffer;

    protected Dictionary<int, MarkerBehaviour> allDetectedMarkers = new Dictionary<int, MarkerBehaviour>();
    private List<int> lostIds = new List<int>();

    protected Point2f[][] corners;
    protected int[] ids;
    
    protected Point2f[][] rejectedImgPoints;

    private Thread detectMarkersThread;

    protected bool updateThread = false;

    protected int threadCounter = 0;
    protected bool outputImage = false;

    protected virtual void Init()
    {
        Application.targetFrameRate = 60;
        
        detectorParameters = DetectorParameters.Create();

        detectorParameters.DoCornerRefinement = doCornerRefinement;
        //Apply different detection parameters
        AssignDetectorParameterStructValues();
        
        dictionary = CvAruco.GetPredefinedDictionary(markerDictionaryType);
        
        timeCount = markerDetectorPauseTime;
        
        DetectMarkerAsync();
        
    }

    private void OnDisable()
    {
        updateThread = false;
        detectMarkersThread.Abort();
        
        if (!img.IsDisposed) img.Release();
        //if (!imgBuffer.IsDisposed) imgBuffer.Release();
        if (!grayedImg.IsDisposed) grayedImg.Release();
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

    private void OnApplicationQuit()
    {
       if(detectMarkersThread.IsAlive) detectMarkersThread.Abort();
    }
    
    

    private void DetectMarkers()
    {
        while (true)
        {
            #if UNITY_ANDROID
            Debug.Log("Updating...");
            #endif
            
            if (!updateThread)
            {
                //we skip updating the thread when not needed and also avoids memory exceptions when we disable the 
                //mono behaviour or we haven't updated the main thread yet!
                
                //Debug.Log("SKIPPED");
                continue;
            }

            if (threadCounter > 0)
            {
                //Debug.Log("Detecting Markers");
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
    
    protected virtual void CheckIfLostMarkers()
    {
        if (ids == null) return;
        
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

    protected virtual void CheckIfDetectedMarkers()
    {
        if(ids == null) return;
        
        int count = 0;
        
        if (ids.Length > 0 && OnMarkersDetected != null)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (!MarkerManager.IsMarkerRegistered(ids[i]))
                {
                    count++;
                }
            }
            
            if(count == ids.Length) return;

            OnMarkersDetected.Invoke(ids);
        }
    }

    private void AssignDetectorParameterStructValues()
    {
        detectorParameters.AdaptiveThreshWinSizeMin = detectionParamsStruct.adaptiveThreshWinSizeMin;
        detectorParameters.AdaptiveThreshWinSizeMax = detectionParamsStruct.adaptiveThreshWinSizeMax;
        detectorParameters.AdaptiveThreshWinSizeStep = detectionParamsStruct.adaptiveThreshWinSizeStep;
        detectorParameters.AdaptiveThreshConstant = detectionParamsStruct.adaptiveThreshConstant;
        detectorParameters.MinMarkerPerimeterRate = detectionParamsStruct.minMarkerPerimeterRate;
        detectorParameters.MaxMarkerPerimeterRate = detectionParamsStruct.maxMarkerPerimeterRate;
        detectorParameters.PolygonalApproxAccuracyRate = detectionParamsStruct.polygonalApproxAccuracyRate;
        detectorParameters.MinCornerDistanceRate = detectionParamsStruct.minCornerDistanceRate;
        detectorParameters.MinDistanceToBorder = detectionParamsStruct.minDistanceToBorder;
        detectorParameters.MinMarkerDistanceRate = detectionParamsStruct.minMarkerDistanceRate;
        detectorParameters.CornerRefinementWinSize = detectionParamsStruct.cornerRefinementWinSize;
        detectorParameters.CornerRefinementMaxIterations = detectionParamsStruct.cornerRefinementMaxIterations;
        detectorParameters.CornerRefinementMinAccuracy = detectionParamsStruct.cornerRefinementMinAccuracy;
        detectorParameters.MarkerBorderBits = detectionParamsStruct.markerBorderBits;
        detectorParameters.PerspectiveRemovePixelPerCell = detectionParamsStruct.perspectiveRemovePixelPerCell;
        detectorParameters.PerspectiveRemoveIgnoredMarginPerCell =
            detectionParamsStruct.perspectiveRemoveIgnoredMarginPerCell;
        detectorParameters.MaxErroneousBitsInBorderRate = detectionParamsStruct.maxErroneousBitsInBorderRate;
        detectorParameters.MinOtsuStdDev = detectionParamsStruct.minOtsuStdDev;
        detectorParameters.ErrorCorrectionRate = detectionParamsStruct.errorCorrectionRate;
    }
}
