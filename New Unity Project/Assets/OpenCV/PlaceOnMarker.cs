using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnMarker : MonoBehaviour
{
    public MarkerBehaviour marker;
    public GameObject obj;

    private bool active = false;
    
    private void Start()
    {
        obj.SetActive(false);

        marker.OnMarkerDetected.AddListener(delegate { active = true;});
        marker.OnMarkerLost.AddListener(delegate { active = false; });
    }
    private void Update()
    {
        obj.SetActive(active);
        UpdatePose();
    }

    private void UpdatePose()
    {
        transform.position = marker.GetCurrentPose().position;
        transform.rotation = marker.GetCurrentPose().rotation;
        transform.localScale = marker.GetCurrentPose().scale;
    }
}
