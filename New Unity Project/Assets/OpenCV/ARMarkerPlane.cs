using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Create a plane by using 4 markers representing it's corners.
public class ARMarkerPlane : MonoBehaviour
{
    public int topLeftMarkerID;
    public int topRightMarkerID;
    public int bottomLeftMarkerID;
    public int bottomRightMarkerID;

    public float markerSizeInMeters;
    public float planeSizeInMeters = 3;
    
    public static int idcounter = 0;
    
    [HideInInspector]
    public int arenaId;
    
    private void Awake()
    {
        arenaId = idcounter;
        idcounter++;
        
        CreateMarkerPlane();
    }

    public Vector3 GetCenterBasedOnACorner(MarkerBehaviour cornerMarker)
    {
        if (topLeftMarkerID == cornerMarker.GetMarkerID())
        {
            //top left to center:
            return cornerMarker.GetCurrentPose().position +
                   new Vector3(planeSizeInMeters / 2, -planeSizeInMeters / 2, 0);
        }
        else if (topRightMarkerID == cornerMarker.GetMarkerID())
        {
            return cornerMarker.GetCurrentPose().position +
                   new Vector3(-planeSizeInMeters / 2, -planeSizeInMeters / 2, 0);
        }
        else if (bottomLeftMarkerID == cornerMarker.GetMarkerID())
        {
            return cornerMarker.GetCurrentPose().position +
                   new Vector3(planeSizeInMeters / 2, planeSizeInMeters / 2, 0);
        }
        else if (bottomRightMarkerID == cornerMarker.GetMarkerID())
        {
            return cornerMarker.GetCurrentPose().position +
                   new Vector3(-planeSizeInMeters / 2, planeSizeInMeters / 2, 0);
        }
        else
        {
            return Vector3.negativeInfinity;
        }
    }

    private void CreateMarkerPlane()
    {
        gameObject.AddComponent<MarkerBehaviour>().SetMarkerIdAndSize(topLeftMarkerID,markerSizeInMeters);
        gameObject.AddComponent<MarkerBehaviour>().SetMarkerIdAndSize(topRightMarkerID,markerSizeInMeters);
        gameObject.AddComponent<MarkerBehaviour>().SetMarkerIdAndSize(bottomLeftMarkerID,markerSizeInMeters);
        gameObject.AddComponent<MarkerBehaviour>().SetMarkerIdAndSize(bottomRightMarkerID,markerSizeInMeters);
        
    }
}
