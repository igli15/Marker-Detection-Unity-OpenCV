using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WebCameraSettings")]
public class WebCameraSettings : ScriptableObject
{
    public int requestedWidth = 1280;
    public int requestedHeight = 720;
    public int TextureFPS = 60;
    public int cameraIndex = 0;

}
