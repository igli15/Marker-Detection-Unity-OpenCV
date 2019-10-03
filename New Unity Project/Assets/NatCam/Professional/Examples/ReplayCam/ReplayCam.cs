/* 
*   NatCam Professional
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Examples {

    using UnityEngine;
    using Core;
    using Core.UI;

    #if NATCAM_EXTENDED && NATCAM_PROFESSIONAL
    using Extended;
    #endif

    public class ReplayCam : NatCamBehaviour {

        [Header("ReplayCam")]
        public RecordingIcon recordingIcon;
        public bool saveToGallery = false, playVideo = true;

        [Header("Preview")]
        public NatCamPreview previewPanel;

        public override void OnStart () {
            // Display and scale the preview
            previewPanel.Apply(NatCam.Preview);
        }

        #if NATCAM_EXTENDED && NATCAM_PROFESSIONAL

		void Update () {
			// Enumerate touches
			foreach (Touch t in Input.touches)
            // Check for double tap
            if (t.phase == TouchPhase.Ended && t.tapCount == 2)
            // Toggle recording
            if (NatCam.IsRecording) NatCam.StopRecording(); else NatCam.StartRecording(OnVideoSave);
            // Set IsRecording property on RecordingIcon
            if (recordingIcon) recordingIcon.IsRecording = NatCam.IsRecording;
		}

		void OnVideoSave (SaveMode mode, string path) {
            // Log
            Debug.Log("Recorded video to path: "+path);
            // Save to gallery
			if (saveToGallery && NatCam.Implementation.SaveVideo(path)) Debug.Log("Saved video to gallery");
            #if !UNITYEDITOR && (UNITY_IOS || UNITY_ANDROID)
            // Play video
            if (playVideo) Handheld.PlayFullScreenMovie(path, Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFill);
            #endif
        }
        #endif
    }
}