using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using UnityEngine;

public abstract class AbstractMarkerDetector : MonoBehaviour
{
    public static event Action<int[]> OnMarkersDetected;
    public static event Action<int[]> OnMarkersLost;

    public PredefinedDictionaryName markerDictionaryType;
    [SerializeField] private bool doCornerRefinement = true;
    public bool throwMarkerCallbacks = true;
    public bool drawMarkerOutlines = false;

    public CalibrationData calibrationData;
    
    protected DetectorParameters detectorParameters;
    protected Dictionary dictionary;
    protected Mat grayedImg = new Mat();

    //private Mat notRotated = new Mat();
    protected Mat img = new Mat();
    protected Mat imgBuffer;

    protected Dictionary<int, MarkerBehaviour> allDetectedMarkers = new Dictionary<int, MarkerBehaviour>();
    protected List<int> lostIds = new List<int>();

    protected Point2f[][] corners;
    protected int[] ids;

    protected Point2f[][] cornersCache;
    protected int[] idsCache;

    protected Point2f[][] rejectedImgPoints;

    private Thread detectMarkersThread;

    protected bool updateThread = false;

    protected int threadCounter = 0;
    protected bool outputImage = false;
    
    protected virtual void Init()
    {
        detectorParameters = DetectorParameters.Create();

        detectorParameters.DoCornerRefinement = doCornerRefinement;

        dictionary = CvAruco.GetPredefinedDictionary(markerDictionaryType);
        
        DetectMarkerAsync();
        
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
            Debug.Log("Updating...");
            
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
        if (ids.Length > 0 && OnMarkersDetected != null)
        {
            OnMarkersDetected.Invoke(ids);
        }
    }
}
