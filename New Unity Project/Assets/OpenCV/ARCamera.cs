using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARCamera : MonoBehaviour
{
    public Camera cam;
    public GameObject outputImage;

    private RectTransform imageRectTransform;

    private void Awake()
    {
        imageRectTransform = outputImage.GetComponent<RectTransform>();
        Calibrate();
    }

    
    
    void Calibrate()
    {
        float width = 1280;
        float height = 720;
        
    }
    
}
