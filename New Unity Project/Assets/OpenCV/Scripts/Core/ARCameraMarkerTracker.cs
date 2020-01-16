using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class ARCameraMarkerTracker : ARCameraTracker
{
    private MarkerBehaviour trackingTarget;
    public Camera arCamera;
    public Transform currentTrackedMarkerTransform;
    
    public float maxAngle = 100;
    public float minAngle = 75;
    
    private Pose oldPose = new Pose();
    
    [Range(-1,1)]
    public float minDirectionDotProductValue = -1;

    private List<MarkerBehaviour> markersCache = new List<MarkerBehaviour>();

    protected override void RepositionCamera()
    {
        if (trackingTarget == null)
        {
            Debug.Log("Returning");
            return;
        }

        oldPose.position = transform.position;
        oldPose.rotation = transform.rotation;

        Matrix4x4 newHolder = GetPoseMatrix(trackingTarget);
        
        //set position and rotation
        transform.SetPositionAndRotation(newHolder.GetColumn(3), newHolder.rotation);

        //Check if it flipped if so go back to old pose position and rotation
        float angle = Vector3.Angle(transform.up, -trackingTarget.transform.forward);
        if (angle >= maxAngle || angle < minAngle)
        {
            transform.SetPositionAndRotation(oldPose.position,oldPose.rotation);
        }
    }


    //Calculates the matrix needed to place the camera in front of the marker.
    private Matrix4x4 GetPoseMatrix(MarkerBehaviour marker)
    {
        Pose targetPose = GetTargetPose(marker);

        currentTrackedMarkerTransform.SetPositionAndRotation(targetPose.position,targetPose.rotation);
        
        Matrix4x4 camMat = Matrix4x4.TRS(arCamera.transform.localPosition, arCamera.transform.localRotation,
            arCamera.transform.localScale);
        
        Matrix4x4 newHolder =
            currentTrackedMarkerTransform.localToWorldMatrix * camMat.inverse;

        return newHolder;
    }

    //Calculates the target pose for the camera in order to appear in front of the marker.
    private Pose GetTargetPose(MarkerBehaviour marker)
    {
        Matrix4x4 m = marker.GetMatrix();
        Matrix4x4 inverseMat = m.inverse;
        Pose p = new Pose();
        p.rotation = ARucoUnityHelper.GetQuaternion(inverseMat);
        p.position = marker.transform.position;
        p.position += ARucoUnityHelper.GetPosition(inverseMat);
        
        return p;
    }


    protected override void ConcentrateOnTheClosestMarker(int[] markerIds)
    {
        MarkerBehaviour closestMarker = null;
        float closestDistance = Mathf.Infinity;

        markersCache.Clear();
        
        if (minDirectionDotProductValue >= -0.9f)
        {
            Vector3 oldPos = transform.position;
            Quaternion oldRotation = transform.rotation;

            
            foreach (int i in markerIds)
            {
                MarkerBehaviour m = MarkerManager.GetMarker(i);

                if (m == null) continue;

                Pose targetPose = GetTargetPose(m);
                
                transform.SetPositionAndRotation(targetPose.position,targetPose.rotation);

                Vector3 cameraForward = new Vector3(transform.forward.x,0,transform.forward.z);
                Vector3 markerForward = new Vector3(m.transform.forward.x,0,m.transform.forward.z);
                
                float dot = Vector3.Dot(cameraForward,markerForward);
                
               // Debug.Log("DOT: " + dot);
                
                if (dot > 0)
                {
                    markersCache.Add(m);
                }
                else
                {
                    if (trackingTarget != null && m.GetMarkerID() == trackingTarget.GetMarkerID())
                    {
                        trackingTarget = null;
                    }
                }
            }
            
            transform.SetPositionAndRotation(oldPos,oldRotation);
        }

        if (markersCache.Count == 0 || minDirectionDotProductValue < -0.9f)
        {
            foreach (int i in markerIds)
            {
                MarkerBehaviour m = MarkerManager.GetMarker(i);
                
                if (m == null) continue;

                markersCache.Add(m);
            }
        }
        
        foreach (MarkerBehaviour m in markersCache)
        {
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
            Debug.Log("here");
            return;
        }

        float d1 = GetMarkerDistanceFromCamera(closestMarker);
        float d2 = GetMarkerDistanceFromCamera(trackingTarget);

        float difference = Mathf.Abs(d1 - d2);
        //Debug.Log(difference);
        if (difference > 0.1) trackingTarget = closestMarker;
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