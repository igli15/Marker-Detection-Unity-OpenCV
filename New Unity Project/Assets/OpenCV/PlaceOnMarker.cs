using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnMarker : MonoBehaviour
{
    public MarkerBehaviour marker;
    public GameObject obj;
    float dtime = 0;
    private void Start()
    {
        obj.SetActive(false);

        marker.OnMarkerDetected.AddListener(delegate { obj.SetActive(true);});
        marker.OnMarkerLost.AddListener(delegate { obj.SetActive(false); });
    }
    private void Update()
    {
        transform.position = marker.GetCurrentPose().position;
        transform.rotation = marker.GetCurrentPose().rotation;
        transform.localScale = marker.GetCurrentPose().scale;
    }
}
