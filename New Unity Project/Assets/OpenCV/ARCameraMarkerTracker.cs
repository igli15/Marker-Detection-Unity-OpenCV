using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraMarkerTracker : ARCameraTracker
{
    private MarkerBehaviour trackingTarget;

    protected override void RepositionCamera()
    {
        if (trackingTarget == null) return;
        
        Matrix4x4 m = trackingTarget.GetMatrix();
        Matrix4x4 inverseMat = m.inverse;

            /*
        Matrix4x4 markerWorldMatrix = Matrix4x4.TRS(trackingTarget.transform.position,
            trackingTarget.transform.rotation, trackingTarget.transform.localScale);

        Matrix4x4 finalMat = markerWorldMatrix * inverseMat;
        transform.SetPositionAndRotation(ARucoUnityHelper.GetPosition(finalMat),ARucoUnityHelper.GetQuaternion(finalMat));
        */
        
        transform.rotation = ARucoUnityHelper.GetQuaternion(inverseMat);
        transform.position = trackingTarget.transform.position;
        transform.position += ARucoUnityHelper.GetPosition(inverseMat);
        //Debug.Log(ARucoUnityHelper.GetScale(m));
        //trackingTarget.transform.localScale = ARucoUnityHelper.GetScale(m);

    }

    protected override void ConcentrateOnTheClosestMarker(int[] markerIds)
    {
        MarkerBehaviour closestMarker = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (int i in markerIds)
        {
            MarkerBehaviour m = MarkerManager.GetMarker(i);
            
            if (m == null) continue;
            
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

        if (difference > 2) trackingTarget = closestMarker;
    }
}
