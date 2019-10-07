using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Create a plane by using 4 markers representing it's corners.
public class ARMarkerPlane : MonoBehaviour
{
    public MarkerBehaviour topLeftMarker;
    public MarkerBehaviour topRightMarker;
    public MarkerBehaviour bottomLeftMarker;
    public MarkerBehaviour bottomRightMarker;
    
    public float planeSizeInMeters = 3;

    [HideInInspector] public int arenaID;
    
    private void Awake()
    {
        arenaID = topLeftMarker.GetMarkerID();
        MarkerManager.RegisterMarkerPlane(this);
    }
    
    public Vector3 GetCenterBasedOnACorner(MarkerBehaviour cornerMarker)
    {
        if (MarkerManager.CompareMarkers(topLeftMarker,cornerMarker))
        {
            //top left to center:
            return cornerMarker.GetCurrentPose().position +
                   new Vector3(planeSizeInMeters / 2, -planeSizeInMeters / 2, 0);
        }
        else if (MarkerManager.CompareMarkers(topRightMarker,cornerMarker))
        {
            return cornerMarker.GetCurrentPose().position +
                   new Vector3(-planeSizeInMeters / 2, -planeSizeInMeters / 2, 0);
        }
        else if (MarkerManager.CompareMarkers(bottomLeftMarker,cornerMarker))
        {
            return cornerMarker.GetCurrentPose().position +
                   new Vector3(planeSizeInMeters / 2, planeSizeInMeters / 2, 0);
        }
        else if (MarkerManager.CompareMarkers(bottomRightMarker,cornerMarker))
        {
            return cornerMarker.GetCurrentPose().position +
                   new Vector3(-planeSizeInMeters / 2, planeSizeInMeters / 2, 0);
        }
        else
        {
            return Vector3.negativeInfinity;
        }
    }

    public bool ContainsMarkerID(int id)
    {
        if (id == topLeftMarker.GetMarkerID())
        {
            return true;
        }
        else if (id == topRightMarker.GetMarkerID())
        {
            return true;
        }
        else if (id == bottomLeftMarker.GetMarkerID())
        {
            return true;
        }
        else if (id == bottomRightMarker.GetMarkerID())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
