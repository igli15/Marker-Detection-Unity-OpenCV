using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionARCamera : MonoBehaviour
{
    public Camera arCamera;
    public MarkerBehaviour targetMarker;

    private bool shouldReposition = false;
    
    private void Start()
    {
        targetMarker.OnMarkerDetected.AddListener(RepositionCamera);
        

        targetMarker.OnMarkerDetected.AddListener(delegate { shouldReposition = true;});
        targetMarker.OnMarkerLost.AddListener(delegate {
            shouldReposition = false;
        });
    }

    public void Update()
    {
        if(shouldReposition)
            RepositionCamera();
    }

    private void RepositionCamera()
    {
        Matrix4x4 m = targetMarker.GetMatrix();
        Matrix4x4 inverseMat = m.inverse;

        arCamera.transform.position = transform.position;
        arCamera.transform.position += ARucoUnityHelper.GetPosition(inverseMat);
        arCamera.transform.rotation = ARucoUnityHelper.GetQuaternion(inverseMat);
        transform.localScale = ARucoUnityHelper.GetScale(m);

    }
}
