using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static CalibrationThread;

public class CalibrationThread
{
    public static void CalibrateAsync(int boardWidth, int boardHeight,ref List<List<Point3f>> objPoints,ref List<List<Point2f>> imagePoints, Mat mat,CalibrationData calibrationData)
    {
        Debug.Log("Calibrating Async.....");

        if (CalibrateCamera.OnCalibrationStarted != null) CalibrateCamera.OnCalibrationStarted();
        
        double[,] k = new double[3, 3];
        double[] d = new double[4];

        Vec3d[] rvec = new Vec3d[boardWidth *  boardHeight];
        Vec3d[] tvec = new Vec3d[boardWidth *  boardHeight];
        
        
        Debug.Log("Error: " + Cv2.CalibrateCamera(objPoints, imagePoints, mat.Size(), k, d, out rvec, out tvec,
                      CalibrationFlags.FixK4 | CalibrationFlags.FixK5,TermCriteria.Both(30,1)));
        
        calibrationData.RegisterMatrix(k);
        Debug.Log(d[0] + " "+  d[1] + " " + d[2] + " "+  d[3]);
        Debug.Log("Finished!!");

        if (CalibrateCamera.OnCalibrationFinished != null)
        {
            CalibrateCamera.OnCalibrationFinished(calibrationData);
        }
    }
}


public class CalibrateCamera : WebCamera
{
    [Title("Calibration Settings",null,TitleAlignments.Centered)]
    public int boardWidth;
    public int boardHeight;
    public int squareSizeMilimeters;

    public CalibrationData calibrationData;
    
    [Title("Bool Variables",null,TitleAlignments.Centered)]
    public BoolVariable startCalibration;
    public BoolVariable registerCalibFrame;
    
    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    private Mat mat;
    private Mat grayMat = new Mat();

    private Size boardSize;
    private List<Point2f> corners = new List<Point2f>();
    private List<Point3f> obj = new List<Point3f>();
    
    private List<List<Point2f>>  imagePoints = new List<List<Point2f>>();
    private List<List<Point3f>> objPoints = new List<List<Point3f>>();

    public static Action<CalibrationData> OnCalibrationFinished;
    public static Action OnCalibrationStarted;
    
    protected override void Start()
    {
        base.Start();
        // Create default parameres for detection
        detectorParameters = DetectorParameters.Create();

        // Dictionary holds set of all available markers
        dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_1000);

        boardSize = new Size(boardWidth,boardHeight);
        
    }

    private void RegisterCurrentCalib()
    {
        //corners.Clear();
        obj.Clear();
        //imagePoints.Clear();
        //objPoints.Clear();
        
        bool b = false;

        b = Cv2.FindChessboardCorners(mat, boardSize, OutputArray.Create(corners));

        if(!b) return;

        Cv2.CornerSubPix(grayMat, corners, new Size(5, 5), new Size(-1, -1), TermCriteria.Both(30, 0.1));
        Debug.Log(b);

        Cv2.DrawChessboardCorners(mat, boardSize, corners, b);
        
        for (int i = 0; i < boardSize.Height; i++)
        {
            for (int j = 0; j < boardSize.Width; j++)
            {
                obj.Add(new Point3f((float)j * squareSizeMilimeters, (float)i * squareSizeMilimeters, 0));
                if (b)
                {
                    imagePoints.Add(corners);
                    objPoints.Add(obj);
                }
            }
        }

    }

    private void Calibrate()
    {
        double[,] k = new double[3, 3];
        double[] d = new double[4];

        Vec3d[] rvec = new Vec3d[boardWidth *  boardHeight];
        Vec3d[] tvec = new Vec3d[boardWidth *  boardHeight];
        
        
        Debug.Log("Error: " + Cv2.CalibrateCamera(objPoints, imagePoints, mat.Size(), k, d, out rvec, out tvec,
            CalibrationFlags.FixK4 | CalibrationFlags.FixK5,TermCriteria.Both(30,1)));
        
        calibrationData.RegisterMatrix(k);
        Debug.Log(d[0] + " "+  d[1] + " " + d[2] + " "+  d[3]);
       Debug.Log(DebugMatrix(k));
    }

    private string DebugMatrix( double[,] m)
    {
        return m[0,0] + " " + m[0,1] + " " + m[0,2] + "\n" +
               m[1,0] + " " + m[1,1] + " " + m[1,2] + "\n" +
                m[2,0] + " " + m[2,1] + " " + m[2,2];
    }

 protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
 {
        TextureParameters.FlipHorizontally = false;
        mat = ARucoUnityHelper.TextureToMat(input, TextureParameters);

        Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

        if(registerCalibFrame.value)
        {
            RegisterCurrentCalib();
            registerCalibFrame.value = false;
        }

        if(startCalibration.value)
        {
            //Calibrate();
            //boardWidth,boardHeight,ref objPoints,ref imagePoints,mat,calibrationData
            Thread t = new Thread(() => CalibrateAsync(boardWidth,boardHeight,ref objPoints,ref imagePoints,mat,calibrationData));
            t.Start();
            startCalibration.value = false;
        }
     
            
        output = ARucoUnityHelper.MatToTexture(mat,output);

        mat.Release();
       // grayMat.Release();
        
        return true;
        //rawImage.texture = outputTexture;
 }

 private void OnDisable()
 {
     mat.Release();
     grayMat.Release();
     
 }
}