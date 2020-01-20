using System;
using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;

public class MarkerBehaviour : MonoBehaviour
{
    [System.Serializable]
    public struct MarkerPose
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public struct MarkerData
    {
        public Point2f[] corners;
        public Point2f[] rejectedImgPoints;
    }

    [SerializeField] private float sizeInMeters = 1;
    [SerializeField] private int markerId;

    public UnityEvent OnMarkerDetected;
    public UnityEvent OnMarkerLost;

    private MarkerData currentMarkerData;
    private MarkerPose currentMarkerPose;

    private Matrix4x4 currentTransformationMatrix;

    public MarkerPose GetCurrentPose()
    {
        return currentMarkerPose;
    }

    public int GetMarkerID()
    {
        return markerId;
    }

    public Matrix4x4 GetMatrix()
    {
        return currentTransformationMatrix;
    }
    
    private void Awake()
    {
        //Register the marker 
        MarkerManager.RegisterMarker(this);
    }

    //Update Marker will create the transformation matrix and convert that to unity space 
    //"corners" is the array received from open cv array of corners, k and d are manual calibration data 
    //for more info on how they are used see UpdateMarkerPose and CreateTransformationMatrix functions below.
    public void UpdateMarker(Point2f[] corners, double[,] k, double[] d,
        Mat grayMat = null, Nullable<Vector3> additionalRotation = null)
    {
        currentMarkerData.corners = corners;

        //currentMarkerData.rejectedImgPoints = rejectedImgPoints;
        UpdateMarkerPose(CreateTransformationMatrix(k, d, grayMat),
            additionalRotation);
    }

    //Update Marker will create the transformation matrix and convert that to unity space 
    //"corners" is the array received from open cv array of corners,XRCameraIntrinsics is an AR Foundation class
    //which contains info about the camera calibration without doing it manually
    //for more info on how they are used see UpdateMarkerPose and CreateTransformationMatrix functions below.
    public void UpdateMarker(Point2f[] corners,
        XRCameraIntrinsics cameraIntrinsics, Mat grayMat = null, Nullable<Vector3> additionalRotation = null)
    {
        currentMarkerData.corners = corners;

        //currentMarkerData.rejectedImgPoints = rejectedImgPoints;
        UpdateMarkerPose(CreateTransformationMatrix(cameraIntrinsics, grayMat),
            additionalRotation);
    }

    public void SetMarkerIdAndSize(int id, float meters)
    {
        markerId = id;
        sizeInMeters = meters;
    }
    
    public void SetWorldSize(float size)
    {
        sizeInMeters = size;
    }

    //Updates Marker pose data by transforming the "transform matrix" received from open cv.
    //additional rotation can be passed so that the whole world matrix is rotated accordingly.
    //that is useful for AR-Foundation since there the camera view is on landscape mode and needs to be rotated properly
    private void UpdateMarkerPose(Matrix4x4 transformMatrix, Nullable<Vector3> additionalRotation = null)
    {
        //convert from open cv space to unity space
        Matrix4x4 matrixY = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
        Matrix4x4 matrixZ = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
        Matrix4x4 matrixX = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-90,0,0), new Vector3(1, 1, 1));
        
        currentTransformationMatrix = (matrixY * transformMatrix * matrixZ) * matrixX;

        //apply additional rotation if needed
        Matrix4x4 r = Matrix4x4.Rotate(Quaternion.Euler(additionalRotation.GetValueOrDefault(Vector3.zero)));
        currentTransformationMatrix = r * currentTransformationMatrix;

        //update the current marker pose position,rotation and scale
        currentMarkerPose.position = ARucoUnityHelper.GetPosition(currentTransformationMatrix);
        currentMarkerPose.rotation = ARucoUnityHelper.GetQuaternion(currentTransformationMatrix);
        currentMarkerPose.scale = ARucoUnityHelper.GetScale(currentTransformationMatrix);
    }

    //Creates the Transformation matrix which transforms the marker from OpenCv camera space to unity world space.
    //Here "k" is the camera matrix and "d" is the distortion coefficients received from the camera calibration
    //If a grayed version of the img is passed then "CornerSubPix" algorithm from open cv will be applied which makes the
    //marker pose better.
    private Matrix4x4 CreateTransformationMatrix(double[,] k, double[] d, Mat grayMat = null)
    {
        if (currentMarkerData.corners.Length == 0)
        {
            Debug.LogError("Marker Is Not Updated");
        }

        float markerSizeInMeters = sizeInMeters;

        //local space marker corner points
        Point3f[] markerPoints = new Point3f[]
        {
            new Point3f(-markerSizeInMeters / 2f, markerSizeInMeters / 2f, 0f),
            new Point3f(markerSizeInMeters / 2f, markerSizeInMeters / 2f, 0f),
            new Point3f(markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f),
            new Point3f(-markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f)
        };

        //create rotation/translating arrays
        double[] rvec = new double[3] {0d, 0d, 0d};
        double[] tvec = new double[3] {0d, 0d, 0d};

        //create rotation matrix
        double[,] rotMatrix = new double[3, 3] {{0d, 0d, 0d}, {0d, 0d, 0d}, {0d, 0d, 0d}};

        //apply CornerSubPix algorithm if needed
        if (grayMat != null)
        {
            Cv2.CornerSubPix(grayMat, currentMarkerData.corners, new Size(5, 5), new Size(-1, -1),
                TermCriteria.Both(30, 0.001));
        }

        //Use SolvePnP algorithm to fill the above arrays with transformation data from open cv
        Cv2.SolvePnP(markerPoints, currentMarkerData.corners, k, d, out rvec, out tvec, false, SolvePnPFlags.Iterative);

        //Use Rodrigues algorithm to fill the rotation arrays with rotation data from open cv
        Cv2.Rodrigues(rvec, out rotMatrix);

        
        //Apply all the data to an  Unity matrix for ease of use.
        Matrix4x4 matrix = new Matrix4x4();

        matrix.SetRow(0,
            new Vector4((float) rotMatrix[0, 0], (float) rotMatrix[0, 1], (float) rotMatrix[0, 2], (float) tvec[0]));
        matrix.SetRow(1,
            new Vector4((float) rotMatrix[1, 0], (float) rotMatrix[1, 1], (float) rotMatrix[1, 2], (float) tvec[1]));
        matrix.SetRow(2,
            new Vector4((float) rotMatrix[2, 0], (float) rotMatrix[2, 1], (float) rotMatrix[2, 2], (float) tvec[2]));
        matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
        return matrix;
    }

    //Creates the Transformation matrix which transforms the marker from OpenCv camera space to unity world space.
    //Here XRCameraIntrinsics is an ARFoundation which contains camera calibration info done by ARFoundation
    //If a grayed version of the img is passed then "CornerSubPix" algorithm from open cv will be applied which makes the
    //marker pose better.
    private Matrix4x4 CreateTransformationMatrix(XRCameraIntrinsics cameraIntrinsics, Mat grayMat = null)
    {
        if (currentMarkerData.corners.Length == 0)
        {
            Debug.LogError("Marker Is Not Updated");
        }

        float markerSizeInMeters = sizeInMeters;

        //local space marker corner points
        Point3f[] markerPoints = new Point3f[]
        {
            new Point3f(-markerSizeInMeters / 2f, markerSizeInMeters / 2f, 0f),
            new Point3f(markerSizeInMeters / 2f, markerSizeInMeters / 2f, 0f),
            new Point3f(markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f),
            new Point3f(-markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f)
        };


        double[,] rawCameraMatrix;

        float fx = 0;
        float fy = 0;

        float cx = 0;
        float cy = 0;
        
        //Use the Landscape left orientation for our camera camera intrinsics since 
        //the picture that goes to opencv its always oriented that way so we need to edit our principal points,
        //by rotation them 270 degrees.
        
        fx = cameraIntrinsics.focalLength.x;
        fy = cameraIntrinsics.focalLength.y;

        cx =  (float)(cameraIntrinsics.resolution.x) - cameraIntrinsics.principalPoint.x;
        cy = (float)(cameraIntrinsics.resolution.y) - cameraIntrinsics.principalPoint.y;

        rawCameraMatrix = new double[3, 3]
        {
            {fx, 0d, cx},
            {0d,fy, cy},
            {0d, 0d, 1d}
        };
        
        //create rotation/translating arrays
        double[] rvec = new double[3] {0d, 0d, 0d};
        double[] tvec = new double[3] {0d, 0d, 0d};

        //create rotation matrix
        double[,] rotMatrix = new double[3, 3] {{0d, 0d, 0d}, {0d, 0d, 0d}, {0d, 0d, 0d}};

        //apply CornerSubPix algorithm if needed
        if (grayMat != null)
        {
            Cv2.CornerSubPix(grayMat, currentMarkerData.corners, new Size(5, 5), new Size(-1, -1),
                TermCriteria.Both(30, 0.001));
        }

        //Use SolvePnP algorithm to fill the above arrays with transformation data from open cv
        Cv2.SolvePnP(markerPoints, currentMarkerData.corners, rawCameraMatrix, new double[5] {0, 0, 0, 0, 0}, out rvec,
            out tvec, false, SolvePnPFlags.Iterative);

        //Use Rodrigues algorithm to fill the rotation arrays with rotation data from open cv
        Cv2.Rodrigues(rvec, out rotMatrix);


        Matrix4x4 matrix = new Matrix4x4();


        matrix.SetRow(0,
            new Vector4((float) rotMatrix[0, 0], (float) rotMatrix[0, 1], (float) rotMatrix[0, 2], (float) tvec[0]));
        matrix.SetRow(1,
            new Vector4((float) rotMatrix[1, 0], (float) rotMatrix[1, 1], (float) rotMatrix[1, 2], (float) tvec[1]));
        matrix.SetRow(2,
            new Vector4((float) rotMatrix[2, 0], (float) rotMatrix[2, 1], (float) rotMatrix[2, 2], (float) tvec[2]));
        matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
        return matrix;
    }
}