using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Aruco;
using System;

public class MarkerDetector : WebCamera
{
    public Camera cam;
    public PredefinedDictionaryName markerDictionaryType;
    
    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    private Mat grayedImg = new Mat();
    private Mat img;

    private Dictionary<int, MarkerBehaviour> allDetectedMarkers = new Dictionary<int, MarkerBehaviour>();
    private List<int> lostIds = new List<int>();

    protected override void Start()
    {
        base.Start();  
        Init();
    }

    void Init()
    {
        detectorParameters = DetectorParameters.Create();
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

        Point2f[][] corners;
        int[] ids;
        Point2f[][] rejectedImgPoints;

        CvAruco.DetectMarkers(grayedImg, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
        //CvAruco.DrawDetectedMarkers(img, corners, ids);

        CheckIfLostMarkers(ids);

        for (int i = 0; i < ids.Length; i++)
        {
            if (!MarkerManager.IsMarkerRegistered(ids[i]))
            {
                continue;
            }

            MarkerBehaviour m = MarkerManager.GetMarker(ids[i]);

            if (m == null)
            {
                continue;
            }

            if (!allDetectedMarkers.ContainsKey(ids[i]))
            {
                m.OnMarkerDetected.Invoke();
                allDetectedMarkers.Add(m.GetMarkerID(), m);
            }
            m.UpdateMarker(img.Cols, img.Rows, corners[i], rejectedImgPoints[i]);
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

        foreach (int i in lostIds)
        {
            allDetectedMarkers.Remove(i);
        }
        lostIds.Clear();
    }
}
