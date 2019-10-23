using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CalibrateCamera : MonoBehaviour
{
    [Title("Calibration Settings",null,TitleAlignments.Centered)]
    public int boardWidth;
    public int boardHeight;
    public float squareSizeMilimeters;

    public CalibrationData calibrationData;
    
    [Title("Variables",null,TitleAlignments.Centered)]
    public StringVariable patternSizeString;
    
    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    private Mat mat;
    private Mat grayMat = new Mat();
    private float imageWidth;
    private float imageHeight;

    private Size boardSize;
    private List<Point2f> corners = new List<Point2f>();
    private List<Point3f> obj = new List<Point3f>();
    
    private List<List<Point2f>>  imagePoints = new List<List<Point2f>>();
    private List<List<Point3f>> objPoints = new List<List<Point3f>>();

    private Thread calibrationThread = null;

    private bool reset = false;

    private bool captureFrame = false;

    private bool calibrate = false;
    
    //public static Mutex calibrationMutex = new Mutex();
    
    public static Action<CalibrationData> OnCalibrationFinished;
    public static Action OnCalibrationStarted;
    public static Action OnCalibrationReset;
    
    protected void Start()
    {
        calibrationData.LoadData();
        
        // Create default parameres for detection
        detectorParameters = DetectorParameters.Create();

        // Dictionary holds set of all available markers
        dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_1000);

        boardSize = new Size(boardWidth,boardHeight);
        
       //OnCalibrationFinished += delegate(CalibrationData data) {calibrationMutex.Dispose();  };

    }

    private void OnEnable()
    {
        WebCamera.OnProcessTexture += OnProcessTexture;
    }

    public void StartCalibrateAsync()
    {
        if (objPoints.Count > 0 && calibrationThread == null)
        {
            //boardWidth,boardHeight,ref objPoints,ref imagePoints,mat,calibrationData
            calibrationThread = new Thread(Calibrate);
            calibrationThread.Start();
        }
    }
    
    private void Calibrate()
    {
        Debug.Log("Calibrating Async.....");
       // calibrationMutex.WaitOne();
        
        if (OnCalibrationStarted != null) CalibrateCamera.OnCalibrationStarted();

        int maxSize = (int)Mathf.Max(imageWidth, imageHeight);
        double fx = maxSize;
        double fy = maxSize;

        double cx = (double)imageWidth / 2;
        double cy = (double)imageHeight / 2;

        double[,] k = new double[3, 3]
        {
            {fx, 0d, cx},
            {0d, fy, cy},
            {0d, 0d, 1d}
        };
        
        double[] d = new double[5];
        double projectionError = -1;
        
        Vec3d[] rvec = new Vec3d[boardWidth *  boardHeight];
        Vec3d[] tvec = new Vec3d[boardWidth *  boardHeight];

        Size boardSize= new Size(boardWidth,boardHeight);
        try
        {//mat.Size()
            projectionError = Cv2.CalibrateCamera(objPoints, imagePoints, new Size(imageWidth, imageHeight), k, d,
                out rvec, out tvec,
                CalibrationFlags.FixAspectRatio,  TermCriteria.Both(30, 0.1));
            Debug.Log("Error: " + projectionError);
        }
        catch (Exception e)
        {
            ResetCalibrationImmediate();
            Debug.Log("restarting...");
        }

        
        calibrationData.RegisterMatrix(k);
        //calibrationData.RegisterDistortionCoefficients(d);
        calibrationData.projectionError = projectionError;

        string s = "";
        for (int i = 0; i < d.Length; i++)
        {
            s += d[i] + " ";
        }
        
        Debug.Log(s);
        Debug.Log("Finished!!");

        if (OnCalibrationFinished != null)
        {
            OnCalibrationFinished(calibrationData);
        }
        //calibrationMutex.ReleaseMutex();
    }

    public void ResetCalibrationImmediate()
    {
        
        objPoints.Clear();
        imagePoints.Clear();

        if(CalibrateCamera.OnCalibrationReset != null) CalibrateCamera.OnCalibrationReset();
        
        Debug.Log("Reseting....");
    }
    public void RegisterCurrentCalib()
    {
        corners.Clear();
        obj.Clear();
        //imagePoints.Clear();
        //objPoints.Clear();
        
        bool b = false;

        b = Cv2.FindChessboardCorners(mat, boardSize, OutputArray.Create(corners),ChessboardFlags.AdaptiveThresh | ChessboardFlags.NormalizeImage);

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

    protected bool OnProcessTexture(WebCamTexture input, ref Texture2D output,ARucoUnityHelper.TextureConversionParams textureParameters)
    {
        
        textureParameters.FlipHorizontally = false;
     if (!float.TryParse(patternSizeString.value, out squareSizeMilimeters))
     {
         return false;
     }
     squareSizeMilimeters = float.Parse(patternSizeString.value);
     
     mat = ARucoUnityHelper.TextureToMat(input, textureParameters);

     imageWidth = mat.Width;
     imageHeight = mat.Height;
     
     
     Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

     if (reset)
     {
         ResetCalibrationImmediate();
         reset = false;
     }

     if (captureFrame)
     {
         RegisterCurrentCalib();
         captureFrame = false;
     }

     if (calibrate)
     {
         StartCalibrateAsync();
         calibrate = false;
     }
     
     
     output = ARucoUnityHelper.MatToTexture(mat,output);
     
     mat.Release();
     return true;
 }

    public void CaptureFrame()
    {
        captureFrame = true;
    }
    
    public void StartCalibration()
    {
        calibrate = true;
    }
    
    public void Reset()
    {
        reset = true;
    }

    protected void OnDisable()
 {
     if(mat != null && !mat.IsDisposed) mat.Release();
     
     if(grayMat != null && !grayMat.IsDisposed) grayMat.Release();
     
     WebCamera.OnProcessTexture -= OnProcessTexture;

 }
}