/* 
*   NatCam Professional
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using System;
    using System.Runtime.InteropServices;
    using Extended;
    using Util = Utilities.Utilities;

    public sealed partial class NatCamiOS {

        #region --Op vars--
        #pragma warning disable 0414
        private static readonly bool RecordAudio = false;
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
                return NatCamNative.IsRecording();
            }
        }
        #endregion


        #region --Client API--

        public void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size) {
            NatCamNative.PreviewBuffer(out ptr, out width, out height, out size);
        }

        public void StartRecording (SaveCallback callback) {
            recordingCallback = callback;
            NatCamNative.StartRecording();
        }

        public void StopRecording () {
            NatCamNative.StopRecording();
        }
        #endregion

        
        #region --Operations--

        protected override void InitializePreviewBuffer () {
            NatCamNative.InitializePreviewBuffer();
        }

        private void ReleasePreviewBuffer () {
            NatCamNative.ReleasePreviewBuffer();
        }

        public bool SaveVideo (string path, SaveMode mode) {
            if (mode == SaveMode.SaveToPhotoGallery) return SaveVideo(path, (int)mode);
            Util.LogError("SaveVideo API only supports saving to gallery");
            return false;
        }
        #endregion


        #region --Native Interop--
        #if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void SetBitrate (float rate);
        [DllImport("__Internal")]
        private static extern void SetRecordAudio (bool record);
        [DllImport("__Internal")]
        private static extern bool SaveVideo (string path, int unused);
        #else
        private static void SetBitrate (float rate) {}
        private static void SetRecordAudio (bool record) {}
        private static bool SaveVideo (string path, int unused) {return false;}
        #endif
        #endregion
    }
}