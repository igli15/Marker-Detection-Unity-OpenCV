using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraPlaneTracker : ARCameraTracker
{
    [SerializeField] private ARMarkerPlane arPlaneTarget;

    private MarkerBehaviour closestCorner;

    protected override void RepositionCamera()
    {
        if (arPlaneTarget == null || closestCorner == null) return;
        
        
        Matrix4x4 m = closestCorner.GetMatrix();
        Matrix4x4 inverseMat = m.inverse;

        transform.position = closestCorner.transform.position;
        transform.position += ARucoUnityHelper.GetPosition(inverseMat);
        transform.rotation = ARucoUnityHelper.GetQuaternion(inverseMat);
        
        
       // trackingTarget.transform.localScale = ARucoUnityHelper.GetScale(m);

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
        
        if (closestCorner == null)
        {
            closestCorner = closestMarker;
            return;
        }

        float d1 = GetMarkerDistanceFromCamera(closestMarker);
        float d2 = GetMarkerDistanceFromCamera(closestCorner);

        float difference = Mathf.Abs(d1 - d2);

        if (difference > 2) closestCorner = closestMarker;
    }
}
