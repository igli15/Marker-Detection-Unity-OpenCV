using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Aruco;
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

    public float maxPositionChangePerFrame = 0.5f;
    public float maxRotationChangePerFrameDegrees = 1.5f;
    
    private Texture2D texture;
    
    ARucoUnityHelper.TextureConversionParams texParam;
    
    private XRCameraIntrinsics cameraIntrinsics;

    public bool UseCustomCalibration = false;
    
    public CalibrationData calibrationData;

    public bool showOpenCvTexture = false;
    
    public RawImage openCvTexture;


    private PositionAndRotationTracker cameraPoseTracker;

    // Start is called before the first frame update
    void Start()
    {
        cameraPoseTracker = arCamera.GetComponent<PositionAndRotationTracker>();
        texParam = new ARucoUnityHelper.TextureConversionParams();
        cameraManager.frameReceived += OnCameraFrameReceived;
        Init();
        
        if(showOpenCvTexture) openCvTexture.gameObject.SetActive(true);
    }
    

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        //Get the latest image
        XRCameraImage image;
        if (!cameraManager.TryGetLatestImage(out image))
        {
            return;
        }
        
        timeCount += Time.deltaTime;
        
        //select the format of the texture
        var format = TextureFormat.RGBA32;

        //check if the texture changed, and only if so create a new one with the new changes
        if (texture == null || texture.width != image.width || texture.height != image.height)
        {
            texture = new Texture2D(image.width, image.height, format, false);
        }

        //mirror on the Y axis so that it fits open cv standarts
        var conversionParams = new XRCameraImageConversionParams(image, format, CameraImageTransformation.MirrorY);

        // try to apply raw texture data to the texture
        var rawTextureData = texture.GetRawTextureData<byte>();
        try
        {
            image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        }
        finally
        {
            //every Mat must be released before new data is assigned!
            image.Dispose();
        }

        //apply texture
        texture.Apply();
        
        texParam.FlipHorizontally = false;
        
        //create a Mat class from the texture
        imgBuffer = ARucoUnityHelper.TextureToMat(texture, texParam);
        
        // Increment thread counter 
        if (threadCounter == 0 && timeCount >= markerDetectorPauseTime && 
            arCamera.velocity.magnitude <= maxPositionChangePerFrame && cameraPoseTracker.rotationChange <= maxRotationChangePerFrameDegrees)
        {
            //copy the buffer data to the img Mat
            imgBuffer.CopyTo(img);
            Interlocked.Increment(ref threadCounter);
            timeCount = 0;
        }
        
        updateThread = true;

        //Show the texture if needed
        if (showOpenCvTexture)
        {
            openCvTexture.texture = ARucoUnityHelper.MatToTexture(imgBuffer, texture);
        }

        //release imgBuffer Mat
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