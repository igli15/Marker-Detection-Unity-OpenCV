using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectFlipping : MonoBehaviour
{
    [SerializeField] private float maxAngle = 90;
    [SerializeField] private Transform relativeTransform;
    
    private void Update()
    {
        if (Vector3.Angle(transform.up, -relativeTransform.forward) >= maxAngle)
        {
            Vector3 eulerRot = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(eulerRot.x,0,0);
        }
    }
}
