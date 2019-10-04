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

        trackingTarget.OnMarkerDetected.AddListener(delegate { shouldReposition = true;});
        trackingTarget.OnMarkerLost.AddListener(delegate {
            shouldReposition = false;
        });
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
}
