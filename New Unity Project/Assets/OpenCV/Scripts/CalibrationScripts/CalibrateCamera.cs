using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ThreadPriority = System.Threading.ThreadPriority;

public class CalibrateCamera : MonoBehaviour
{
    public WebCamera webCamera;
    
    public int boardWidth;
    public int boardHeight;
    public float squareSizeMeters;
    public OpenCvSharp.CalibrationFlags calibrationFlags;
    public CalibrationData calibrationData;
    
    public StringVariable patternSizeString;
    
    private Mat mat;
    private Mat grayMat = new Mat();
    
    private float imageWidth;
    private float imageHeight;

    private Size boardSize;
    private List<Point2f> corners = new List<Point2f>();
    private List<Point3f> obj = new List<Point3f>();

    private List<List<Point2f>> CornerPoints = new List<List<Point2f>>();
    private List<List<Point3f>> objPoints = new List<List<Point3f>>();

    private Thread calibrationThread = null;

    private bool reset = false;

    private bool captureFrame = false;

    private bool calibrate = false;

    public static Action<CalibrationData> OnCalibrationFinished;
    public static Action OnCalibrationStarted;
    public static Action OnCalibrationReset;

    protected void Start()
    {
        calibrationData.LoadData();

        boardSize = new Size(boardWidth, boardHeight);
    }

    private void OnEnable()
    {
        webCamera.OnProcessTexture += OnProcessTexture;
    }

    public void StartCalibrateAsync()
    {
        if (objPoints.Count > 0 && calibrationThread == null)
        {
            calibrationThread = new Thread(Calibrate);
            calibrationThread.Start();
            calibrationThread.Priority = ThreadPriority.Highest;
        }
    }

    private void Calibrate()
    {
        Debug.Log("Calibrating Async.....");
        // calibrationMutex.WaitOne();

        if (OnCalibrationStarted != null) CalibrateCamera.OnCalibrationStarted();

        //prepare the data which the calibration process will fill up
        int maxSize = (int) Mathf.Max(imageWidth, imageHeight);
        double fx = maxSize;
        double fy = maxSize;

        double cx = (double) imageWidth / 2;
        double cy = (double) imageHeight / 2;

        double[,] k = new double[3, 3]
        {
            {fx, 0d, cx},
            {0d, fy, cy},
            {0d, 0d, 1d}
        };

        double[] d = new double[5];
        double projectionError = -1;

        Vec3d[] rvec = new Vec3d[boardWidth * boardHeight];
        Vec3d[] tvec = new Vec3d[boardWidth * boardHeight];

        Size boardSize = new Size(boardWidth, boardHeight);
        try
        {
            // calibrate the camera
            projectionError = Cv2.CalibrateCamera(objPoints, CornerPoints, new Size(imageWidth, imageHeight), k, d,
                out rvec, out tvec,
                calibrationFlags, TermCriteria.Both(30, 0.1));
            Debug.Log("Error: " + projectionError);
        }
        catch (Exception e)
        {
            ResetCalibrationImmediate();
            Debug.Log("restarting...");
        }

        //register the data and save them
        calibrationData.RegisterMatrix(k);
        calibrationData.RegisterDistortionCoefficients(d);
        
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
    
    //Stop the calibration thread and clear the data
    public void ResetCalibrationImmediate()
    {
        objPoints.Clear();
        CornerPoints.Clear();
        corners.Clear();
        obj.Clear();

        calibrationThread.Abort();
        calibrationThread = null;
        if (CalibrateCamera.OnCalibrationReset != null) CalibrateCamera.OnCalibrationReset();

        Debug.Log("Reseting....");
    }

    //Capture a rendered texture frame and register the checkerboard pattern data
    public void RegisterCurrentCalib()
    {
        corners.Clear();
        obj.Clear();
        //imagePoints.Clear();
        //objPoints.Clear();

        bool b = false;
        
        //find the corners and populate the data for one sqaure
        b = Cv2.FindChessboardCorners(mat, boardSize, OutputArray.Create(corners),
            ChessboardFlags.AdaptiveThresh | ChessboardFlags.NormalizeImage | ChessboardFlags.FastCheck);

        if (!b) return;
        
        Cv2.CornerSubPix(grayMat, corners, new Size(5, 5), new Size(-1, -1), TermCriteria.Both(30, 0.1));
        Debug.Log(b);

        // for debug draw the found squares
        Cv2.DrawChessboardCorners(mat, boardSize, corners, b);

        for (int i = 0; i < boardSize.Height; i++)
        {
            for (int j = 0; j < boardSize.Width; j++)
            {
                //add the space coordinates of the squares. Z = 0 since its  a flat plane.
                obj.Add(new Point3f((float) j * squareSizeMeters, (float) i * squareSizeMeters, 0));
                if (b)
                {
                    //register the data per square
                    CornerPoints.Add(corners);
                    objPoints.Add(obj);
                }
            }
        }
    }

    private bool OnProcessTexture(WebCamTexture input, ref Texture2D output,
        ARucoUnityHelper.TextureConversionParams textureParameters)
    {
        textureParameters.FlipHorizontally = false;
        if (!float.TryParse(patternSizeString.value, out squareSizeMeters))
        {
            return false;
        }

        squareSizeMeters = float.Parse(patternSizeString.value);

        mat = ARucoUnityHelper.TextureToMat(input, textureParameters);

        //Debug.Log("Width: " + mat.Width + " Height: " + mat.Height);
        
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


        output = ARucoUnityHelper.MatToTexture(mat, output);

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
        if(calibrationThread != null) calibrationThread.Abort();
        
        if (mat != null && !mat.IsDisposed) mat.Release();

        if (grayMat != null && !grayMat.IsDisposed) grayMat.Release();

        webCamera.OnProcessTexture -= OnProcessTexture;
    }
}