/* 
*   NatCam Professional
*   Copyright (c) 2016 Yusuf Olokoba
*/

// Remember to uncomment '#define OPENCV_API' in NatCam.cs in Assets>NatCam>Professional>Plugins>Managed
//#define OPENCV_API // Uncomment this to run this example properly

namespace NatCamU.Examples {

    using UnityEngine;
    using Core;
    
    #if OPENCV_API
    using OpenCVForUnity;
    #endif

    public class VisionCam : NatCamBehaviour {
        
        #if NATCAM_PROFESSIONAL && OPENCV_API

        Mat matrix;
        Texture2D texture;
        Color32[] colors;
        const TextureFormat format =
		#if UNITY_IOS && !UNITY_EDITOR
		TextureFormat.BGRA32;
		#else
		TextureFormat.RGBA32;
		#endif
        
        // Don't display the preview from the camera, we will use OpenCV
        public override void OnPreviewStart () {}

        public override void OnPreviewUpdate () {
            // Get the preview matrix for this frame
            if (!NatCam.PreviewMatrix(ref matrix)) return;
            // Check the color buffer
            colors = colors ?? new Color32[matrix.cols() * matrix.rows()];
            // Check the destination texture
            texture = texture ?? new Texture2D(matrix.cols(), matrix.rows(), format, false, false);
            // Size checking // Ideally, call Texture2D.Destroy and realloc
			if (texture.width != matrix.cols() || texture.height != matrix.rows()) texture.Resize(matrix.cols(), matrix.rows());
            // Draw a diagonal line on our image
            Imgproc.line(matrix, new Point(0, 0), new Point(matrix.cols(), matrix.rows()), new Scalar(255, 0, 0, 255), 4);
            // Update our destination texture with the line drawn above
            Utils.matToTexture2D(matrix, texture, colors);
            // Set our UI Panel's RawImage texture to our destination texture
            preview.texture = texture;
        }
        #endif
    }
}