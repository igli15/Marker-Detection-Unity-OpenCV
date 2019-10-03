/* 
*   NatCam Professional
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using UnityEngine;
    using System;
    using Extended;
    using Util = Utilities.Utilities;

    public sealed partial class NatCamAndroid {

        #region --Op vars--
        private static AndroidJavaClass natcamprofessional;
        #pragma warning disable 0414
        private static readonly bool
        ReadablePreview = true,
        RecordAudio = true;
        #pragma warning restore 0414
        private const float VideoBitrate = 11.4f; // The final bitrate that is given to the encoder is (width * height * bitrate) // Use 4.05 for lower resolutions
        #endregion


        #region --Properties--
        
        public bool SupportsRecording {
            get {
                return true;
            }
        }

        public bool IsRecording {
            get {
                return natcamprofessional.CallStatic<bool>("isRecording");
            }
        }
        #endregion


        #region --Client API--

        public void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size) {
            NatCamNative.PreviewBuffer(out ptr, out width, out height, out size);
        }

        public void StartRecording (SaveCallback callback) {
            recordingCallback = callback;
            natcamprofessional.CallStatic("startRecording");
        }

        public void StopRecording () {
            natcamprofessional.CallStatic("stopRecording");
        }
        #endregion


        #region --Operations--
        protected override void InitializePreviewBuffer () {
            natcamprofessional.CallStatic("initializePreviewBuffer", ReadablePreview);
        }

        private void ReleasePreviewBuffer () {}

        public bool SaveVideo (string path, SaveMode mode) {
            if (mode == SaveMode.SaveToPhotoGallery) return natcamprofessional.CallStatic<bool>("saveVideo", path, (int)mode);
            Util.LogError("SaveVideo API only supports saving to gallery");
            return false;
        }
        #endregion
    }
}