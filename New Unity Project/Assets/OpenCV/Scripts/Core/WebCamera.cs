using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WebCamera : MonoBehaviour
{
    public GameObject imageGameObject;
    public WebCameraSettings webCamSettings;

    private WebCamDevice? webCamDevice = null;
    private WebCamTexture webCamTexture = null;
    private Texture2D renderedTexture = null;

    private ARucoUnityHelper.TextureConversionParams textureParameters;

    private RawImage rawImage;
    private RectTransform imgRectTransform;

    public delegate bool OnProcessTextureDelegate(WebCamTexture input, ref Texture2D output,
        ARucoUnityHelper.TextureConversionParams textureConversionParams);

    public event OnProcessTextureDelegate OnProcessTexture;

    public delegate void OnTextureResolutionChangedDelegate(int newWidth, int newHeight);
    public event OnTextureResolutionChangedDelegate OnTextureResolutionChanged;
    
    
    protected virtual void Start()
    {
        Application.targetFrameRate = 60;
        textureParameters = new ARucoUnityHelper.TextureConversionParams();
        SetUpPhysicalCamera(webCamSettings.cameraIndex);

        rawImage = imageGameObject.GetComponent<RawImage>();
        imgRectTransform = imageGameObject.GetComponent<RectTransform>();
    }

    public void SetUpPhysicalCamera(int deviceIndex)
    {
        //check for physical cameras
        if (WebCamTexture.devices.Length <= 0)
        {
            Debug.LogWarning("No Camera Device Was Found!");
            return;
        }

        //check if the provided index is correct
        if (deviceIndex >= WebCamTexture.devices.Length)
        {
            webCamDevice = null;
            Debug.LogError("There is no device with that index");
            return;
        }
        
        //Get the device
        webCamDevice = WebCamTexture.devices[deviceIndex];

        //Create a WebCamTexture
        webCamTexture = new WebCamTexture(webCamDevice.Value.name,webCamSettings.requestedWidth,
            webCamSettings.requestedHeight, webCamSettings.TextureFPS);

        
        ApplyTextureConversionParameters();

        AssignNewCameraTextureResolution(webCamSettings.requestedWidth, webCamSettings.requestedHeight, true);
    }

    public void AssignNewCameraTextureResolution(int requestedWidth, int requestedHeight, bool play = true)
    {
        if (webCamTexture.isPlaying) webCamTexture.Pause();
        
        webCamTexture.Stop();

        webCamTexture.requestedWidth = requestedWidth;
        webCamTexture.requestedHeight = requestedHeight;
        

        if (play) webCamTexture.Play();

        StartCoroutine(CallResolutionChangedEvent());
    }

    //adds a delay to the callback since the widht/height take some time till they are actually changed.
    IEnumerator CallResolutionChangedEvent()
    {
        yield return new WaitForSeconds(3);
        OnTextureResolutionChanged?.Invoke(webCamTexture.width, webCamTexture.height);
        yield return null;
    }

    private void Update()
    {
        if (webCamTexture.height < 100) return;
        
        
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            ApplyTextureConversionParameters();
            
            if (OnProcessTexture != null && OnProcessTexture(webCamTexture, ref renderedTexture, textureParameters))
            {
                RenderFrame();
            }

        }
        
    }
    
    //Applies the raw image texture and its size
    private void RenderFrame()
    {
        if (renderedTexture != null)
        {
            // apply
            rawImage.texture = renderedTexture;

            // Adjust image ration according to the texture sizes 
            imgRectTransform.sizeDelta = new Vector2(renderedTexture.width, renderedTexture.height);
        }
    }

    private void ApplyTextureConversionParameters()
    {
        //Rotate the texture properly and fix mirroring issues
        textureParameters.FlipHorizontally = false;
        
        if (0 != webCamTexture.videoRotationAngle)
            textureParameters.RotationAngle = webCamTexture.videoRotationAngle; // cw -> ccw
    }

    void OnDestroy()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            Debug.Log("Camera is still playing");
            webCamTexture.Pause();

            while (webCamTexture.isPlaying)
            {
                Debug.Log("Camera is stopping....");
            }

            Debug.Log("Camera stopped playing");
        }

        webCamTexture.Stop();
        webCamTexture = null;
        webCamDevice = null;
    }
}