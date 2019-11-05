using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFoundationMarkerDetector : MonoBehaviour
{
    public ARCameraManager cameraManager;
    public RawImage dispayImage;

    private Texture2D texture;
    
    //Events thrown each frame holding all ids found/lost
    public static event Action<int[]> OnMarkersDetected;
    public static event Action<int[]> OnMarkersLost;
    
    public PredefinedDictionaryName markerDictionaryType;
    [SerializeField] private bool doCornerRefinement = true;
    public bool debugMode = false;

    public CalibrationData calibrationData;
    private DetectorParameters detectorParameters;
    private Dictionary dictionary;
    private Mat grayedImg = new Mat();
    private Mat img;

    private Dictionary<int, MarkerBehaviour> allDetectedMarkers = new Dictionary<int, MarkerBehaviour>();
    private List<int> lostIds = new List<int>();

    private Point2f[][] corners;
    private int[] ids;
    
    private Point2f[][] cornersCache;
    private int[] idsCache;
    
    private Point2f[][] rejectedImgPoints;

    private Thread detectMarkersThread;

    private int imgCols;
    private int imgRows;

    bool updateThread = false;

    // Start is called before the first frame update
    void Start()
    {
        cameraManager.frameReceived += OnCameraFrameReceived;
    }

    // Update is called once per frame
    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        XRCameraImage image;
        if (!cameraManager.TryGetLatestImage(out image))
            return;

        var conversionParams = new XRCameraImageConversionParams
        {
            // Get the entire image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Choose RGBA format
            outputFormat = TextureFormat.RGBA32,
            
            transformation = CameraImageTransformation.MirrorY

        };

        // See how many bytes we need to store the final image.
        int size = image.GetConvertedDataSize(conversionParams);

        // Allocate a buffer to store the image
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract the image data
        image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

        // The image was converted to RGBA32 format and written into the provided buffer
        // so we can dispose of the CameraImage. We must do this or it will leak resources.
        image.Dispose();

        // At this point, we could process the image, pass it to a computer vision algorithm, etc.
        // In this example, we'll just apply it to a texture to visualize it.

        if (texture == null)
        {
            // We've got the data; let's put it into a texture so we can visualize it.
            texture = new Texture2D(
                conversionParams.outputDimensions.x,
                conversionParams.outputDimensions.y,
                conversionParams.outputFormat,
                false);
        }

        texture.LoadRawTextureData(buffer);
        texture.Apply();

        dispayImage.texture = texture;
        
        // Done with our temporary data
        buffer.Dispose();
        
        //Resources.UnloadUnusedAssets();
    }
}

