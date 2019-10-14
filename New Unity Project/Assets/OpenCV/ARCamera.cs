using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using OpenCvSharp;
using UnityEngine;
using UnityEngine.UI;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Vector2 = UnityEngine.Vector2;

public class ARCamera : MonoBehaviour
{
    public Camera arCam;
    public Camera webCam;
    public GameObject outputImage;
    public ArDebugValues arDebugValues;
    
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
        currentRectSize = imageRectTransform.sizeDelta;
        if (Screen.width != currentScreenSize.x || Screen.height != currentScreenSize.y || currentRectSize.x != oldRectSize.x || currentRectSize.y != oldRectSize.y) 
        {
            currentScreenSize = new Vector2 (Screen.width, Screen.height);
            oldRectSize = currentRectSize;
            Debug.Log("Calibrating");
			//imageRectTransform.sizeDelta = new Vector2(Screen.width,Screen.height);	
            Calibrate();
        }
    }


    void Calibrate()
    {
        float width = currentRectSize.x;
        float height = currentRectSize.y;

        float imgScale = 1.0f;

        float widthScale = (float)Screen.width / width;
        float heightScale = (float) Screen.height / height;
        
        if (widthScale < heightScale) 
        {
            webCam.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            imgScale = (float)Screen.height / (float)Screen.width;
        } else 
        {
            webCam.orthographicSize = height / 2;
        }

        int maxSize = (int)Mathf.Max(width, height);
        double fx = maxSize;
        double fy = maxSize;

        double cx = width / 2;
        double cy = height / 2;

        double[,] cameraMatrix = new double[3, 3] {
            {fx, 0d, cx},
            {0d, fy, cy},
            {0d, 0d, 1d}
        };
        
        
        Size imgSize = new Size(width * imgScale,height * imgScale);
        
        double apertureWidth = 0;
        double apertureHeight = 0;
        double fovx = -1;
        double fovy = -1;
        double focalLength = -1;
        Point2d principalPoint = new Point (0, 0);
        double aspectratio = -1;
        //Cv2.CalibrateCamera()
        Cv2.CalibrationMatrixValues (cameraMatrix, imgSize, apertureWidth, apertureHeight, 
            out fovx, out fovy, out focalLength, out principalPoint, out aspectratio);

        arDebugValues.fovy = (float)fovy;
        arDebugValues.fovx = (float)fovx;
        arDebugValues.screenWidth = Screen.width;
        arDebugValues.screenHeight = Screen.height;
        arDebugValues.imageWidth = currentRectSize.x;
        arDebugValues.imageHeight = currentRectSize.y;
        arDebugValues.imageScale = imgScale;
        
        double fovXScale = (2.0 * Mathf.Atan ((float)(imgSize.Width / (2.0 * fx)))) / (Mathf.Atan2 ((float)cx, (float)fx) + Mathf.Atan2 ((float)(imgSize.Width - cx), (float)fx));
        double fovYScale = (2.0 * Mathf.Atan ((float)(imgSize.Height / (2.0 * fy)))) / (Mathf.Atan2 ((float)cy, (float)fy) + Mathf.Atan2 ((float)(imgSize.Height - cy), (float)fy));
            
       
        
        if (widthScale < heightScale) {
            arCam.fieldOfView = (float)(fovx * fovXScale);
        } else {
            arCam.fieldOfView = (float)(fovy * fovYScale);
        }

        //arCam.aspect = (float)aspectratio;

    }
    
}
