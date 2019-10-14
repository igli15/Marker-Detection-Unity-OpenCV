using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ARDebugValues")]
public class ArDebugValues : ScriptableObject
{
    public float screenWidth;
    public float screenHeight;
    public float fovx;
    public float fovy;
    public float imageWidth;
    public float imageHeight;
    public float imageScale;
    
    public override string ToString()
    {
        return "screenWidth: " + screenWidth + "\n" +
               "screenHeight: " + screenHeight + "\n" +
               "fovx: " + fovx + "\n" +
               "fovy: " + fovy + "\n" +
               "imageWidth: " + imageWidth + "\n" +
               "imageHeight: " + imageHeight + "\n" +
               "imageScale: " + imageScale + "\n";
    }
}
