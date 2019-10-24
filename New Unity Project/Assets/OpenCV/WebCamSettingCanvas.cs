using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WebCamSettingCanvas : MonoBehaviour
{
    public WebCamera webCam;
    public TextMeshProUGUI textComponent;
    public Button applyButton;

    public InputField widthInputField;
    public InputField heightInputField;
    
    private void Awake()
    {
        webCam.OnTextureResolutionChanged += OnWebCamResolutionChanged;
        applyButton.onClick.AddListener(OnApply);
    }

    private void OnWebCamResolutionChanged(int width,int height)
    {
        textComponent.text = "Resolution: " + width + " x " + height;
    }
    
    private void OnApply()
    {
        int width = 0;
        int height = 0;
        
        if (int.TryParse(widthInputField.text,out width) && int.TryParse(heightInputField.text,out height))
        {
            webCam.AssignNewCameraTextureResolution(width, height,true);
        }
        else
        {
            Debug.LogWarning("You have NOT passed a int to the width or height input fields!");
        }
    }
}
