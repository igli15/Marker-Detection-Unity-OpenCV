using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraTracker : MonoBehaviour
{
    private Camera arCam;
    
    public MarkerBehaviour trackingTarget;
    
    private bool shouldReposition = false;
    
    // Start is called before the first frame update
    void Start()
    {
        arCam = GetComponent<Camera>();

        MarkerDetector.OnMarkersDetected += UpdateCameraPose;
        MarkerDetector.OnMarkersDetected += ConcentrateOnTheClosestMarker;
        MarkerDetector.OnMarkersLost += StopUpdatingCameraPose;
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldReposition)
            RepositionCamera();
    }
    
    private void RepositionCamera()
    {
        if (trackingTarget == null) return;
        
        Matrix4x4 m = trackingTarget.GetMatrix();
        Matrix4x4 inverseMat = m.inverse;

        transform.position = trackingTarget.transform.position;
        transform.position += ARucoUnityHelper.GetPosition(inverseMat);
        transform.rotation = ARucoUnityHelper.GetQuaternion(inverseMat);
        
        trackingTarget.transform.localScale = ARucoUnityHelper.GetScale(m);

    }
    
    public float GetMarkerDistanceFromCamera(MarkerBehaviour m)
    {
        return ARucoUnityHelper.GetPosition(m.GetMatrix()).magnitude;
    }
    
    private void ConcentrateOnTheClosestMarker(int[] markerIds)
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

    private void UpdateCameraPose(int[] ids)
    {
        shouldReposition = true;
    }

    private void StopUpdatingCameraPose(int[] ids)
    {
        shouldReposition = false;
    }

    //Unsubscribe from events
    //Static events are persistent so we need to manually unsubscribe from them.
    private void OnDestroy()
    {
        MarkerDetector.OnMarkersDetected -= UpdateCameraPose;
        MarkerDetector.OnMarkersDetected -= ConcentrateOnTheClosestMarker;
        MarkerDetector.OnMarkersLost -= StopUpdatingCameraPose;
    }
}
