using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CalibrateCamera : MonoBehaviour
{
    public Texture2D[] textures;
    public RawImage rawImage;

    public int boardWidth;
    public int boardHeight;
    public int squareSizeMilimeters;

    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    private Mat mat;
    private Mat grayMat;

    private Size boardSize;
    private List<Point2f> corners = new List<Point2f>();
    private List<Point3f> obj = new List<Point3f>();
    
    private List<List<Point2f>>  imagePoints = new List<List<Point2f>>();
    private List<List<Point3f>> objPoints = new List<List<Point3f>>();
    
    void Start()
    {
        // Create default parameres for detection
        detectorParameters = DetectorParameters.Create();

        // Dictionary holds set of all available markers
        dictionary = CvAruco.GetPredefinedDictionary(PredefinedDictionaryName.Dict4X4_1000);

        boardSize = new Size(boardWidth,boardHeight);

        foreach (var t in textures)
        {
            // Create Opencv image from unity texture
            mat = ARucoUnityHelper.TextureToMat(t);

            // Convert image to grasyscale
            grayMat = new Mat();

            Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

            Calibrate();

            // Create Unity output texture awith detected markers
            Texture outputTexture = ARucoUnityHelper.MatToTexture(mat);

            rawImage.texture = outputTexture;
        }
    }
    
    private void Calibrate()
    {
        bool b = false;

        b = Cv2.FindChessboardCorners(mat, boardSize, OutputArray.Create(corners));
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

        double[,] k = new double[3, 3];
        double[] d = new double[4];

        Vec3d[] rvec = new Vec3d[boardWidth *  boardHeight];
        Vec3d[] tvec = new Vec3d[boardWidth *  boardHeight];
        
        Cv2.CalibrateCamera(objPoints, imagePoints, mat.Size(), k, d, out rvec, out tvec,
            CalibrationFlags.FixK4 | CalibrationFlags.FixK5,TermCriteria.Both(30,0));
        
        Debug.Log(d[0] + " "+  d[1] + " " + d[2] + " "+  d[3]);
    }
}