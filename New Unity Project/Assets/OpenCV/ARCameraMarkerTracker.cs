using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraMarkerTracker : ARCameraTracker
{
    private Camera arCam;
    
    public MarkerBehaviour trackingTarget;
    
    private bool shouldReposition = false;


    protected override void RepositionCamera()
    {
        if (trackingTarget == null) return;
        
        Matrix4x4 m = trackingTarget.GetMatrix();
        Matrix4x4 inverseMat = m.inverse;

        transform.position = trackingTarget.transform.position;
        transform.position += ARucoUnityHelper.GetPosition(inverseMat);
        transform.rotation = ARucoUnityHelper.GetQuaternion(inverseMat);
        
        trackingTarget.transform.localScale = ARucoUnityHelper.GetScale(m);

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

        trackingTarget = closestMarker;
    }
}
