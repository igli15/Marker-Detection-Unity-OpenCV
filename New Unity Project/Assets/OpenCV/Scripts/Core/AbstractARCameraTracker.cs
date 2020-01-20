using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractARCameraTracker : MonoBehaviour
{
    //protected Camera arCam;
    
    protected bool shouldReposition = false;

    protected virtual void Start()
    {
        AbstractMarkerDetector.OnMarkersDetected += UpdateCameraPose;
        AbstractMarkerDetector.OnMarkersDetected += ConcentrateOnTheClosestMarker;
        AbstractMarkerDetector.OnMarkersLost += StopUpdatingCameraPose;
    }
    
    protected virtual void Update()
    {
        if (shouldReposition)
        {
            RepositionCamera();
            shouldReposition = false;
        }
    }
    
    protected virtual  void RepositionCamera()
    {
        
    }
    
    protected virtual void ConcentrateOnTheClosestMarker(int[] markerIds)
    {
        
    }
    
    protected float GetMarkerDistanceFromCamera(MarkerBehaviour m)
    {
        return ARucoUnityHelper.GetPosition(m.GetMatrix()).magnitude;
    }

    protected virtual void UpdateCameraPose(int[] ids)
    {
        shouldReposition = true;
    }

    protected virtual void StopUpdatingCameraPose(int[] ids)
    {
        shouldReposition = false;
    }

    //Unsubscribe from events
    //Static events are persistent so we need to manually unsubscribe from them.
    protected virtual void OnDestroy()
    {
        AbstractMarkerDetector.OnMarkersDetected -= UpdateCameraPose;
        AbstractMarkerDetector.OnMarkersDetected -= ConcentrateOnTheClosestMarker;
        AbstractMarkerDetector.OnMarkersLost -= StopUpdatingCameraPose;
    }
}
