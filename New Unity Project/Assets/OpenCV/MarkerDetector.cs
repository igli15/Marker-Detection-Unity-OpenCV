using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Aruco;
using System;

public class MarkerDetector : WebCamera
{
    
    //Events thrown each frame holding all ids found/lost
    public static event Action<int[]> OnMarkersDetected;
    public static event Action<int[]> OnMarkersLost;
    
    public Camera cam;
    public PredefinedDictionaryName markerDictionaryType;
    [SerializeField] private bool doCornerRefinement = true;
    
    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    private Mat grayedImg = new Mat();
    private Mat img;

    private Dictionary<int, MarkerBehaviour> allDetectedMarkers = new Dictionary<int, MarkerBehaviour>();
    private List<int> lostIds = new List<int>();
    
    private Point2f[][] corners;
    private int[] ids;
    private Point2f[][] rejectedImgPoints;
    
    protected override void Start()
    {
        base.Start();  
        Init();
    }

    void Init()
    {
        detectorParameters = DetectorParameters.Create();
       
        detectorParameters.DoCornerRefinement = doCornerRefinement;

        dictionary = CvAruco.GetPredefinedDictionary(markerDictionaryType);
    }

    // Our sketch generation function
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        TextureParameters.FlipHorizontally = false;

        img = ARucoUnityHelper.TextureToMat(input, TextureParameters);

        DetectMarkers(img);

        output = ARucoUnityHelper.MatToTexture(img, output);

        img.Release();
        return true;
    }


    private void DetectMarkers(Mat img)
    {
        Cv2.CvtColor(img, grayedImg, ColorConversionCodes.BGR2GRAY);

        CvAruco.DetectMarkers(grayedImg, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);

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
            m.UpdateMarker(img.Cols, img.Rows, corners[i]);
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
    
}
