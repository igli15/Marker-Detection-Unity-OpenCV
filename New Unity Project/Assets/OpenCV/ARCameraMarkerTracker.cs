using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.ARFoundation;

public class ARCameraMarkerTracker : ARCameraTracker
{
    private MarkerBehaviour trackingTarget;
    public Camera arCamera;
    public Transform currentTrackedMarkerTransform;
    public float cameraMaxSpeed = 1;
   

    protected override void RepositionCamera()
    {
        if (arCamera.velocity.magnitude > cameraMaxSpeed || trackingTarget == null)
        {
            Debug.Log("Returning");
            return;
        }
        
        Matrix4x4 m = trackingTarget.GetMatrix();
        Matrix4x4 inverseMat = m.inverse;

        currentTrackedMarkerTransform.transform.rotation = ARucoUnityHelper.GetQuaternion(inverseMat);
        currentTrackedMarkerTransform.transform.position = trackingTarget.transform.position;
        currentTrackedMarkerTransform.transform.position += ARucoUnityHelper.GetPosition(inverseMat);

        Matrix4x4 camMat = Matrix4x4.TRS(arCamera.transform.localPosition, arCamera.transform.localRotation,
            arCamera.transform.localScale);
        
        Matrix4x4 newHolder =
            currentTrackedMarkerTransform.localToWorldMatrix * camMat.inverse;
        transform.SetPositionAndRotation(newHolder.GetColumn(3), newHolder.rotation);
    }


    protected override void ConcentrateOnTheClosestMarker(int[] markerIds)
    {
        //if(trackingTarget != null)Debug.Log(trackingTarget.GetMarkerID());

        MarkerBehaviour closestMarker = null;
        float closestDistance = Mathf.Infinity;

        foreach (int i in markerIds)
        {
            MarkerBehaviour m = MarkerManager.GetMarker(i);

            if (m == null) continue;

            //Debug.Log("IDD: " + i);
            if (GetMarkerDistanceFromCamera(m) < closestDistance)
            {
                closestDistance = GetMarkerDistanceFromCamera(m);
                closestMarker = m;
            }

            // Debug.Log("ID: " + i + " " + "Distance: " + GetMarkerDistanceFromCamera(m));
        }

        if (closestMarker == null) return;

        if (trackingTarget == null)
        {
            trackingTarget = closestMarker;
            return;
        }

        float d1 = GetMarkerDistanceFromCamera(closestMarker);
        float d2 = GetMarkerDistanceFromCamera(trackingTarget);

        float difference = Mathf.Abs(d1 - d2);
        //Debug.Log(difference);
        if (difference > 0.1) trackingTarget = closestMarker;
    }

    protected override void UpdateCameraPose(int[] ids)
    {
        base.UpdateCameraPose(ids);
    }

    protected override void StopUpdatingCameraPose(int[] ids)
    {
        base.StopUpdatingCameraPose(ids);

        if (trackingTarget == null) return;

        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] == trackingTarget.GetMarkerID())
            {
                trackingTarget = null;
                return;
            }
        }
    }
}