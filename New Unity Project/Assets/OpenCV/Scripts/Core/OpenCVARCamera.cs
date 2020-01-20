using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using OpenCvSharp;
using UnityEngine;
using UnityEngine.UI;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector2 = UnityEngine.Vector2;

public class OpenCVARCamera : MonoBehaviour
{
    public Camera arCam;
    public GameObject outputImage;
    public CalibrationData calibrationData;
    private RectTransform imageRectTransform;
    
    private Vector2 currentScreenSize = Vector2.zero;
    private Vector2 currentRectSize = Vector2.zero;
    private Vector2 oldRectSize = Vector2.zero;
    

    private void Awake()
    {
        imageRectTransform = outputImage.GetComponent<RectTransform>();
        Calibrate();
        
    }

    private void Update()
    {
        if (imageRectTransform.sizeDelta.x < 100) return;
        
        //Calibrate only if the size of the texture changed
        oldRectSize = imageRectTransform.sizeDelta;
        if (Screen.width != currentScreenSize.x || Screen.height != currentScreenSize.y || currentRectSize.x != oldRectSize.x || currentRectSize.y != oldRectSize.y) 
        {
            currentScreenSize = new Vector2 (Screen.width, Screen.height);
            currentRectSize = oldRectSize;

            Calibrate();
        }
    }

    //Calibrates the scene camera based on web cam calibration data
    void Calibrate()
    {
        float width = currentRectSize.x;
        float height = currentRectSize.y;

        float imgScale = 1.0f;
    
        //Do nothing if the texture is wrong
        if (width <= 0 || height <= 0) return;
        
        //Get the screen/texture ratios
        float widthScale = (float)Screen.width / width;
        float heightScale = (float) Screen.height / height;

        float aspect = 1;
        Size imgSize;
        
        //Get the aspect ratio
        if (widthScale < heightScale)
        {
            aspect = heightScale;
            imgSize = new Size(width ,height );
        } 
        else
        {
            float k = (float) Screen.height / (float)Screen.width;
            imgSize = new Size(width ,width * k );
            aspect = widthScale;
        }
        
        //scale to fit the aspect ratio
        outputImage.transform.localScale = new UnityEngine.Vector3(aspect,aspect,1);
        
        //prepare the data for the calibration
        int maxSize = (int)Mathf.Max(width, height);
        
        double fx = maxSize;
        double fy = maxSize;

        //Principal Point
        double cx = width / 2;
        double cy = height / 2;
        
        double[,] cameraMatrix = new Double[3,3];
        
        cameraMatrix = calibrationData.GetCameraMatrix(ref cameraMatrix);

        double apertureWidth = 0;
        double apertureHeight = 0;
        double fovx = -1;
        double fovy = -1;
        double focalLength = -1;
        Point2d principalPoint = new Point (0, 0);
        double aspectratio = -1;
        
        //Do the camera calibration
        Cv2.CalibrationMatrixValues (cameraMatrix, imgSize, apertureWidth, apertureHeight, 
            out fovx, out fovy, out focalLength, out principalPoint, out aspectratio);

        fx = cameraMatrix[0, 0];
        fy = cameraMatrix[1, 1];
        cx = cameraMatrix[0, 2];
        cy = cameraMatrix[1, 2];
        
        //Get the fov scale 
        double fovYScale = (2.0 * Mathf.Atan ((float)(imgSize.Height / (2.0 * fy)))) / (Mathf.Atan2 ((float)cy, (float)fy) + Mathf.Atan2 ((float)(imgSize.Height - cy), (float)fy));
        
        arCam.fieldOfView = (float)fovy * (float)fovYScale;

    }
    
}
