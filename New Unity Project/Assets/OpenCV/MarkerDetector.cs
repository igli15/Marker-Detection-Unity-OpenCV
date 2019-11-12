using OpenCvSharp;
using OpenCvSharp.Aruco;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ThreadPriority = System.Threading.ThreadPriority;

public class MarkerDetector : AbstractMarkerDetector
{
    public WebCamera webCamera;
    public Camera cam;

    public bool drawMarkerOutlines = false;

    public float markerDetectorPauseTime = 0;
    
    public CalibrationData calibrationData;

    [Sirenix.OdinInspector.ReadOnly] [SerializeField]
    private float timeCount = 0;

    protected void Start()
    {
        Init();

        timeCount = markerDetectorPauseTime;

        //detectMarkersThread.Priority = ThreadPriority.Highest;
    }

    private void OnEnable()
    {
        webCamera.OnProcessTexture += ProcessTexture;
    }

    private void OnDisable()
    {
        webCamera.OnProcessTexture -= ProcessTexture;

        if (!img.IsDisposed) img.Release();
        if (!imgBuffer.IsDisposed) imgBuffer.Release();
        if (!grayedImg.IsDisposed) grayedImg.Release();
    }

    // Our sketch generation function
    private bool ProcessTexture(WebCamTexture input, ref Texture2D output,
        ARucoUnityHelper.TextureConversionParams textureParameters)
    {
        imgBuffer = ARucoUnityHelper.TextureToMat(input, textureParameters);
        //Debug.Log("New image Assigned");

        timeCount += Time.deltaTime;

        if (threadCounter == 0 && timeCount >= markerDetectorPauseTime)
        {
            imgBuffer.CopyTo(img);
            Interlocked.Increment(ref threadCounter);
            timeCount = 0;
        }

        updateThread = true;

        if (outputImage)
        {
            if (drawMarkerOutlines)
            {
                CvAruco.DrawDetectedMarkers(img, corners, ids);
            }

            output = ARucoUnityHelper.MatToTexture(img, output);
            //Debug.Log("Marker image Rendered");
            outputImage = false;
        }
        else
        {
            output = ARucoUnityHelper.MatToTexture(imgBuffer, output);
            //Debug.Log("Camera image Rendered");
        }

        imgBuffer.Release();
        return true;
    }


    protected override void CheckIfDetectedMarkers()
    {
        base.CheckIfDetectedMarkers();
        
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
            m.UpdateMarker(corners[i], calibrationData.GetCameraMatrix(),
                calibrationData.GetDistortionCoefficients(), grayedImg);
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