using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WebCamera : MonoBehaviour
{
    public GameObject imageGameObject;
    public Vector2Int cameraMaxTextureResolution = new Vector2Int(1920,1080);
    
    private WebCamDevice? webCamDevice = null;
    private WebCamTexture webCamTexture = null;
    private Texture2D renderedTexture = null;
    
    public int textureFps = 60;

    private ARucoUnityHelper.TextureConversionParams textureParameters;

    private RawImage rawImage;
    private RectTransform imgRectTransform;

    public int cameraDeviceIndex = 0;
    
    public delegate bool OnProcessTextureDelegate(WebCamTexture input,ref Texture2D output,ARucoUnityHelper.TextureConversionParams textureConversionParams);
    public event OnProcessTextureDelegate OnProcessTexture;
    
    protected virtual void Start()
    {
        textureParameters = new ARucoUnityHelper.TextureConversionParams();
        SetUpPhysicalCamera(cameraDeviceIndex);
        
        rawImage = imageGameObject.GetComponent<RawImage>();
        imgRectTransform = imageGameObject.GetComponent<RectTransform>();
    }
    
    public void SetUpPhysicalCamera(int deviceIndex)
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

    private void Update()
    {
        if (webCamTexture.height < 100) return;
        
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            ReadTextureConversionParameters();

            if (OnProcessTexture != null && OnProcessTexture(webCamTexture, ref renderedTexture,textureParameters))
            {
                RenderFrame();
            }
            
            /*
            if (ProcessTexture(webCamTexture,ref renderedTexture))
            {
                RenderFrame();
            }
            */
        }
    }
    
    //protected abstract bool ProcessTexture(WebCamTexture input,ref Texture2D output);
    
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
    
    private void ReadTextureConversionParameters()
    {
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