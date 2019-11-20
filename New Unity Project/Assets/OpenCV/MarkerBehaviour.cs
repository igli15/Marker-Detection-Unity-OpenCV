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

    private void Awake()
    {
        MarkerManager.RegisterMarker(this);
    }

    public void UpdateMarker(Point2f[] corners, double[,] k, double[] d,
        Mat grayMat = null, Nullable<Vector3> additionalRotation = null)
    {
        currentMarkerData.corners = corners;

        //currentMarkerData.rejectedImgPoints = rejectedImgPoints;
        UpdateMarkerPose(CreateTransformationMatrix(k, d, grayMat),
            additionalRotation);
    }

    public void UpdateMarker(Point2f[] corners,
        XRCameraIntrinsics cameraIntrinsics, Mat grayMat = null, Nullable<Vector3> additionalRotation = null)
    {
        currentMarkerData.corners = corners;

        //currentMarkerData.rejectedImgPoints = rejectedImgPoints;
        UpdateMarkerPose(CreateTransformationMatrix(cameraIntrinsics, grayMat),
            additionalRotation);
    }


    //corners 0 is at top left and moves CW
    //returns the local center of the marker
    public Vector2 GetMarkerLocalCenter()
    {
        if (currentMarkerData.corners.Length == 0)
        {
            Debug.LogError("Marker Is Not Updated");
        }

        Vector2 c = Vector2.zero;

        for (int i = 0; i < 4; i++)
        {
            c.x += currentMarkerData.corners[i].X;
            c.y += currentMarkerData.corners[i].Y;
        }

        c.x /= 4;
        c.y /= 4;
        return c;
    }

    public Vector3 GetMarkerScreenSpacePos(float depth)
    {
        Vector2 localCenter = GetMarkerLocalCenter();
        return new Vector3(localCenter.x, localCenter.y, depth);
    }

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

    public void SetMarkerIdAndSize(int id, float meters)
    {
        markerId = id;
        sizeInMeters = meters;
    }

    private void UpdateMarkerPose(Matrix4x4 transformMatrix, Nullable<Vector3> additionalRotation = null)
    {
        Matrix4x4 matrixY = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
        Matrix4x4 matrixZ = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
        currentTransformationMatrix = matrixY * transformMatrix * matrixZ;


        Matrix4x4 r = Matrix4x4.Rotate(Quaternion.Euler(additionalRotation.GetValueOrDefault(Vector3.zero)));
        currentTransformationMatrix = r * currentTransformationMatrix;


        currentMarkerPose.position = ARucoUnityHelper.GetPosition(currentTransformationMatrix);
        currentMarkerPose.rotation = ARucoUnityHelper.GetQuaternion(currentTransformationMatrix);
        currentMarkerPose.scale = ARucoUnityHelper.GetScale(currentTransformationMatrix);
    }

    private Matrix4x4 CreateTransformationMatrix(double[,] k, double[] d, Mat grayMat = null)
    {
        if (currentMarkerData.corners.Length == 0)
        {
            Debug.LogError("Marker Is Not Updated");
        }

        float markerSizeInMeters = sizeInMeters;

        Point3f[] markerPoints = new Point3f[]
        {
            new Point3f(-markerSizeInMeters / 2f, markerSizeInMeters / 2f, 0f),
            new Point3f(markerSizeInMeters / 2f, markerSizeInMeters / 2f, 0f),
            new Point3f(markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f),
            new Point3f(-markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f)
        };

        double[] rvec = new double[3] {0d, 0d, 0d};
        double[] tvec = new double[3] {0d, 0d, 0d};

        double[,] rotMatrix = new double[3, 3] {{0d, 0d, 0d}, {0d, 0d, 0d}, {0d, 0d, 0d}};

        if (grayMat != null)
        {
            Cv2.CornerSubPix(grayMat, currentMarkerData.corners, new Size(5, 5), new Size(-1, -1),
                TermCriteria.Both(30, 0.001));
        }

        Cv2.SolvePnP(markerPoints, currentMarkerData.corners, k, d, out rvec, out tvec, false, SolvePnPFlags.Iterative);

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

    private Matrix4x4 CreateTransformationMatrix(XRCameraIntrinsics cameraIntrinsics, Mat grayMat = null)
    {
        if (currentMarkerData.corners.Length == 0)
        {
            Debug.LogError("Marker Is Not Updated");
        }

        float markerSizeInMeters = sizeInMeters;

        Point3f[] markerPoints = new Point3f[]
        {
            new Point3f(-markerSizeInMeters / 2f, markerSizeInMeters / 2f, 0f),
            new Point3f(markerSizeInMeters / 2f, markerSizeInMeters / 2f, 0f),
            new Point3f(markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f),
            new Point3f(-markerSizeInMeters / 2f, -markerSizeInMeters / 2f, 0f)
        };


        double[,] rawCameraMatrix;

        rawCameraMatrix = new double[3, 3]
        {
            {cameraIntrinsics.focalLength.x, 0d, cameraIntrinsics.principalPoint.x},
            {0d,cameraIntrinsics.focalLength.y, cameraIntrinsics.principalPoint.y},
            {0d, 0d, 1d}
        };

        double[] rvec = new double[3] {0d, 0d, 0d};
        double[] tvec = new double[3] {0d, 0d, 0d};

        double[,] rotMatrix = new double[3, 3] {{0d, 0d, 0d}, {0d, 0d, 0d}, {0d, 0d, 0d}};

        if (grayMat != null)
        {
            Cv2.CornerSubPix(grayMat, currentMarkerData.corners, new Size(5, 5), new Size(-1, -1),
                TermCriteria.Both(30, 0.001));
        }

        Cv2.SolvePnP(markerPoints, currentMarkerData.corners, rawCameraMatrix, new double[5] {0, 0, 0, 0, 0}, out rvec,
            out tvec, false, SolvePnPFlags.Iterative);

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

    public void SetWorldSize(float size)
    {
        sizeInMeters = size;
    }
}