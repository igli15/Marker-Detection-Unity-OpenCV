using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ThreadPriority = System.Threading.ThreadPriority;

public class ARFoundationMarkerDetector : AbstractMarkerDetector
{
    public ARCameraManager cameraManager;

    public Camera arCamera;

    public float limitVelocityMagnitude = 0.5f;
    
    private Texture2D texture;
    
    ARucoUnityHelper.TextureConversionParams texParam;
    private XRCameraIntrinsics cameraIntrinsics;

    public bool UseCustomCalibration = false;
    
    [ShowIf("UseCustomCalibration")]
    public CalibrationData calibrationData;

    public bool showOpenCvTexture = false;

    [ShowIf("showOpenCvTexture")]
    public RawImage openCvTexture;

    // Start is called before the first frame update
    void Start()
    {
        texParam = new ARucoUnityHelper.TextureConversionParams();
        cameraManager.frameReceived += OnCameraFrameReceived;
        Init();
        
        if(showOpenCvTexture) openCvTexture.gameObject.SetActive(true);
    }
    

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        XRCameraImage image;
        if (!cameraManager.TryGetLatestImage(out image))
        {
            return;
        }
        
        timeCount += Time.deltaTime;
        
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
        
        if (threadCounter == 0 && timeCount >= markerDetectorPauseTime && arCamera.velocity.magnitude < limitVelocityMagnitude)
        {
            //Debug.Log("Incrementing thread counter");
            imgBuffer.CopyTo(img);
            Interlocked.Increment(ref threadCounter);
            timeCount = 0;
        }
        

        //Debug.Log("ThreadCounter: " + threadCounter);
        updateThread = true;

        if (showOpenCvTexture)
        {
            openCvTexture.texture = ARucoUnityHelper.MatToTexture(imgBuffer, texture);
        }

        imgBuffer.Release();
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
                Debug.Log("FOUND MARKER: " + m.GetMarkerID());
                m.OnMarkerDetected.Invoke();
                allDetectedMarkers.Add(m.GetMarkerID(), m);
            }
            
            float rotZ = 0;

            switch (Screen.orientation)
            {
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

            if (!UseCustomCalibration)
            {
                cameraManager.TryGetIntrinsics(out cameraIntrinsics);

                m.UpdateMarker(corners[i], cameraIntrinsics, grayedImg, Vector3.forward * rotZ);
            }
            else
            {
                m.UpdateMarker(corners[i], calibrationData.GetCameraMatrix(),calibrationData.GetDistortionCoefficients(), grayedImg,Vector3.forward * rotZ);
            }
        }
    }
}