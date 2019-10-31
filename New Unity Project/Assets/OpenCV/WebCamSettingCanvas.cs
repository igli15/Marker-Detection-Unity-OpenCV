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

    public WebCameraSettings WebCameraSettings;
    
    private float deltaTime = 0.0f;
    
    private void Awake()
    {
        webCam.OnTextureResolutionChanged += OnWebCamResolutionChanged;
        applyButton.onClick.AddListener(OnApply);
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
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
            WebCameraSettings.requestedWidth = width;
            WebCameraSettings.requestedHeight = height;
        }
        else
        {
            Debug.LogWarning("You have NOT passed a int to the width or height input fields!");
        }
        
    }
    
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
 
        GUIStyle style = new GUIStyle();
 
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
