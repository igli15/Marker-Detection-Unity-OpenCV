using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class WebCamera : MonoBehaviour
{

    public GameObject Surface;
    public Vector2Int cameraMaxTextureResolution = new Vector2Int(1920,1080);
    private WebCamDevice? webCamDevice = null;
    private WebCamTexture webCamTexture = null;
    private Texture2D renderedTexture = null;
    
    public int textureFps = 60;

    protected ARucoUnityHelper.TextureConversionParams TextureParameters { get; private set; }

    private RawImage rawImage;
    private AspectRatioFitter aspectRatioFitter;
    private RectTransform imgRectTransform;

    public int cameraDeviceIndex = 0;
    
    
    private void ReadTextureConversionParameters()
    {
        ARucoUnityHelper.TextureConversionParams parameters = new ARucoUnityHelper.TextureConversionParams();

        if (0 != webCamTexture.videoRotationAngle)
            parameters.RotationAngle = webCamTexture.videoRotationAngle; // cw -> ccw
        
        TextureParameters = parameters;
    }

   
    protected virtual void Start()
    {
        rawImage = Surface.GetComponent<RawImage>();
        aspectRatioFitter = Surface.GetComponent<AspectRatioFitter>();
        imgRectTransform = Surface.GetComponent<RectTransform>();
        
        SetUpPhysicalCamera(cameraDeviceIndex);
    }

    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            if (webCamTexture.isPlaying)
            {
                webCamTexture.Stop();
            }
            webCamTexture = null;
        }
        
        webCamDevice = null;
        
    }
    
    private void Update()
    {
        if (webCamTexture.height < 100) return;
        
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            // this must be called continuously
            ReadTextureConversionParameters();

            // process texture with whatever method sub-class might have in mind
            if (ProcessTexture(webCamTexture, ref renderedTexture))
            {
                RenderFrame();
            }
        }
    }
    
    protected abstract bool ProcessTexture(WebCamTexture input, ref Texture2D output);
    
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


    private void SetUpPhysicalCamera(int deviceIndex)
    {
        if (WebCamTexture.devices.Length <= 0)
        {
            Debug.LogWarning("No Camera Device Was Found!");
            return;
        }

        if (deviceIndex >= WebCamTexture.devices.Length)
        {
            webCamDevice = null;
            Debug.LogError("There is no device with that index");
            return;
        }
        
        webCamDevice = WebCamTexture.devices[deviceIndex];
        
        webCamTexture = new WebCamTexture(webCamDevice.Value.name,(int)cameraMaxTextureResolution.x,(int)cameraMaxTextureResolution.y,textureFps);

        ReadTextureConversionParameters();
        
        AssignNewCameraTextureResolution(cameraMaxTextureResolution.x,cameraMaxTextureResolution.y,true);
    }

    public void AssignNewCameraTextureResolution(int requestedWidth,int requestedHeight,bool play = true)
    {
        if(webCamTexture.isPlaying) webCamTexture.Stop();

        webCamTexture.requestedWidth = requestedWidth;
        webCamTexture.requestedHeight = requestedHeight;
        
        if(play) webCamTexture.Play();
    }

    protected virtual void OnDisable()
    {
        webCamTexture.Stop();
    }
}