
using System;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using Rect = OpenCvSharp.Rect;

// Many ideas are taken from http://answers.unity3d.com/questions/773464/webcamtexture-correct-resolution-and-ratio.html#answer-1155328

/// <summary>
/// Base WebCamera class that takes care about video capturing.
/// Is intended to be sub-classed and partially overridden to get
/// desired behavior in the user Unity script
/// </summary>
public abstract class WebCamera : MonoBehaviour
{
    /// <summary>
    /// Target surface to render WebCam stream
    /// </summary>
    public GameObject Surface;
    public Vector2 cameraMaxTextureResolution = new Vector2(1920,1080);
    private Nullable<WebCamDevice> webCamDevice = null;
    protected WebCamTexture webCamTexture = null;
    private Texture2D renderedTexture = null;

    public bool useFrontCamera = false;
    public int textureFps = 60;

    protected bool forceFrontalCamera = false;
    
    protected ARucoUnityHelper.TextureConversionParams TextureParameters { get; private set; }

    protected RawImage rawImage;
    private AspectRatioFitter aspectRatioFitter;
    private RectTransform imgRectTransform;
    
    public string DeviceName
    {
        get
        {
            return (webCamDevice != null) ? webCamDevice.Value.name : null;
        }
        set
        {
            // quick test
            if (value == DeviceName)
                return;

            if (null != webCamTexture && webCamTexture.isPlaying)
                webCamTexture.Stop();

            // get device index
            int cameraIndex = -1;
            for (int i = 0; i < WebCamTexture.devices.Length && -1 == cameraIndex; i++)
            {
                if (WebCamTexture.devices[i].name == value)
                    cameraIndex = i;
            }

            // set device up
            if (-1 != cameraIndex)
            {
                if (useFrontCamera)
                {
                    webCamDevice = WebCamTexture.devices[cameraIndex];
                }
                else
                {
                    webCamDevice = WebCamTexture.devices[0];
                }
                webCamTexture = new WebCamTexture(webCamDevice.Value.name,(int)cameraMaxTextureResolution.x,(int)cameraMaxTextureResolution.y,textureFps);

                //Debug.Log(webCamTexture.width);
                //Debug.Log(webCamTexture.height);

                webCamTexture.requestedWidth = (int)cameraMaxTextureResolution.x;
                webCamTexture.requestedHeight = (int)cameraMaxTextureResolution.y;

               // Debug.Log(webCamTexture.width);
               // Debug.Log(webCamTexture.height);

                // read device params and make conversion map
                ReadTextureConversionParameters();

                webCamTexture.Play();

                //webCamTexture.Stop();
            }
            else
            {
                throw new ArgumentException(String.Format("{0}: provided DeviceName is not correct device identifier", this.GetType().Name));
            }
        }
    }

    /// <summary>
    /// This method scans source device params (flip, rotation, front-camera status etc.) and
    /// prepares TextureConversionParameters that will compensate all that stuff for OpenCV
    /// </summary>
    private void ReadTextureConversionParameters()
    {
        ARucoUnityHelper.TextureConversionParams parameters = new ARucoUnityHelper.TextureConversionParams();

        // frontal camera - we must flip around Y axis to make it mirror-like
        parameters.FlipHorizontally = forceFrontalCamera || webCamDevice.Value.isFrontFacing;

        // TODO:
        // actually, code below should work, however, on our devices tests every device except iPad
        // returned "false", iPad said "true" but the texture wasn't actually flipped

        // compensate vertical flip
        //parameters.FlipVertically = webCamTexture.videoVerticallyMirrored;

        // deal with rotation
        if (0 != webCamTexture.videoRotationAngle)
            parameters.RotationAngle = webCamTexture.videoRotationAngle; // cw -> ccw

        // apply
        TextureParameters = parameters;

        //UnityEngine.Debug.Log (string.Format("front = {0}, vertMirrored = {1}, angle = {2}", webCamDevice.isFrontFacing, webCamTexture.videoVerticallyMirrored, webCamTexture.videoRotationAngle));
    }

   
    protected virtual void Start()
    {
        rawImage = Surface.GetComponent<RawImage>();
        aspectRatioFitter = Surface.GetComponent<AspectRatioFitter>();
        imgRectTransform = Surface.GetComponent<RectTransform>();
        
        if (WebCamTexture.devices.Length > 0)
        {
            DeviceName = WebCamTexture.devices[WebCamTexture.devices.Length - 1].name;
        }
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

        if (webCamDevice != null)
        {
            webCamDevice = null;
        }
    }

    /// <summary>
    /// Updates web camera texture
    /// </summary>
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

            //float videoRatio = (float) cameraMaxTextureResolution.x/cameraMaxTextureResolution.y;
   
            // you'll be using an AspectRatioFitter on the Image, so simply set it
           // aspectRatioFitter.aspectRatio = videoRatio;
            
            // Adjust image ration according to the texture sizes 
            imgRectTransform.sizeDelta = new Vector2(cameraMaxTextureResolution.x, cameraMaxTextureResolution.y);
        }
    }

    protected virtual void OnDisable()
    {
        webCamTexture.Stop();
    }
}