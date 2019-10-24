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


    private float dtCounter = 0;
    private int callCount = 0;
    private int maxCallCount = 0;
    
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

        webCamTexture = new WebCamTexture(webCamDevice.Value.name,webCamSettings.requestedWidth,
            webCamSettings.requestedHeight, webCamSettings.TextureFPS);

        ReadTextureConversionParameters();

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


        dtCounter += Time.deltaTime;
        if (dtCounter > 1)
        {
            maxCallCount = callCount;
            dtCounter = 0;
            callCount = 0;
        }
        //Debug.Log(dtCounter);
        
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            ReadTextureConversionParameters();
            
            if (OnProcessTexture != null && OnProcessTexture(webCamTexture, ref renderedTexture, textureParameters))
            {
                if (dtCounter <= 1) callCount += 1;
                RenderFrame();
            }

            /*
            if (ProcessTexture(webCamTexture,ref renderedTexture))
            {
                RenderFrame();
            }
            */
        }
        
        Debug.Log(callCount);
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
    
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
 
        GUIStyle style = new GUIStyle();
 
        Rect rect = new Rect(0, 40, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);

        string text = "Texture Update Rate: " + maxCallCount.ToString() + "\n" + "Texture Requested FPS: " + webCamSettings.TextureFPS.ToString() ;
        GUI.Label(rect, text, style);
        
    }
}