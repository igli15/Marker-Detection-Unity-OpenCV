using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class ARCameraController : MonoBehaviour
{
    public TrackedPoseDriver trackedPoseDriver;

    private void Update()
    {
        Debug.Log(trackedPoseDriver.originPose.position);
    }
}