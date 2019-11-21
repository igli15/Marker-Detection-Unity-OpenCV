using System;
using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARCameraFOVAdjuster : MonoBehaviour
{
    public ARCameraManager cameraManager;
    public Camera camera;
    private XRCameraIntrinsics cameraIntrinsics;
    
    private bool init = false;
    private void Awake()
    {
        CameraConfigDropdown.OnConfigChanged += ChangeFOV;
    }

    private void Update()
    {
        if (cameraManager.GetConfigurations(Allocator.Temp).Length > 0 && init == false)
        {
            ChangeFOV(cameraManager.currentConfiguration.GetValueOrDefault());
            init = true;
        }
    }


    public void ChangeFOV(XRCameraConfiguration config)
    {
        cameraManager.TryGetIntrinsics(out cameraIntrinsics);

        double[,] k = new double[3, 3]
        {
            {cameraIntrinsics.focalLength.x, 0d, cameraIntrinsics.principalPoint.x},
            {0d, cameraIntrinsics.focalLength.y, cameraIntrinsics.principalPoint.y},
            {0d, 0d, 1d}
        };

        double fovx = 0;
        double fovy = 0;
        double focalLength = 0;
        Point2d principalPoint = new Point(0, 0);
        double aspectratio = -1;

        Size imgSize = new Size(config.width, config.height);

        Cv2.CalibrationMatrixValues(k, imgSize, 0, 0,
            out fovx, out fovy, out focalLength, out principalPoint, out aspectratio);

        double fovYScale = (2.0 * Mathf.Atan((float) (imgSize.Height / (2.0 * cameraIntrinsics.focalLength.y)))) /
                           (Mathf.Atan2((float) cameraIntrinsics.principalPoint.y,
                                (float) cameraIntrinsics.focalLength.y) + Mathf.Atan2(
                                (float) (imgSize.Height - cameraIntrinsics.principalPoint.y),
                                (float) cameraIntrinsics.focalLength.y));

        //Debug.Log("oldFOV: " + camera.fieldOfView);
        camera.fieldOfView = (float) fovy * (float)fovYScale;
        //Debug.Log("newFOV: " + camera.fieldOfView);
    }

    private void OnDestroy()
    {
        CameraConfigDropdown.OnConfigChanged -= ChangeFOV;
    }
}