using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PositionAndRotationTracker : MonoBehaviour
{
    private Quaternion oldRotation;
    private Vector3 oldPosition;

    [ReadOnly]
    public float positionChange;
    [ReadOnly]
    public float rotationChange;

    private void Start()
    {
        oldPosition = transform.position;
        oldRotation = transform.rotation;
    }

    private void Update()
    {
        positionChange = (transform.position - oldPosition).magnitude;
        rotationChange = Quaternion.Angle(transform.rotation, oldRotation);

        oldPosition = transform.position;
        oldRotation = transform.rotation;
        
        //Debug.Log(positionChange);
        //Debug.Log(rotationChange);
    }
}
