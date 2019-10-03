/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

#pragma warning disable 0675

namespace NatCamU.Core.Platforms {

    using UnityEngine;
    using System.Collections.Generic;
    using Util = Utilities.Utilities;

    public class NatCamDeviceLegacy : INatCamDevice {

        #region --Op vars--
        Dictionary<int, long> formats;
        #endregion


        #region --Properties--
        public bool IsRearFacing (int camera) {
            return !WebCamTexture.devices[camera].isFrontFacing;
        }

        public bool IsFlashSupported (int camera) {
            Util.LogError("Flash is not supported on legacy");
            return false;
        }

        public bool IsTorchSupported (int camera) {
            Util.LogError("Torch is not supported on legacy");
            return false;
        }

        public float HorizontalFOV (int camera) {
            Util.LogError("Field of view is not supported on legacy");
            return 0f;
        }

        public float VerticalFOV (int camera) {
            Util.LogError("Field of view is not supported on legacy");
            return 0f;
        }

        public float MinExposureBias (int camera) {
            Util.LogError("Exposure is not supported on legacy");
            return 0f;
        }

        public float MaxExposureBias (int camera) {
            Util.LogError("Exposure is not supported on legacy");
            return 0f;
        }

        public float MaxZoomRatio (int camera) {
            Util.LogError("Zoom is not supported on legacy");
            return 1f;
        }
        #endregion


        #region --Getters--
        public void GetPreviewResolution (int camera, out int width, out int height) {
            bool playing = NatCam.Camera == camera && ((NatCamLegacy)NatCam.Implementation).IsPlaying;
            width = playing ? NatCam.Preview.width : (int)(formats[camera] & short.MaxValue);
            height = playing ? NatCam.Preview.height : (int)((formats[camera] >> 16) & short.MaxValue);
        }

        public void GetPhotoResolution (int camera, out int width, out int height) {
            GetPreviewResolution(camera, out width, out height);
        }

        public float GetFramerate (int camera) {
            return formats[camera] >> 32;
        }
        
        public float GetExposure (int camera) {
            Util.LogError("Exposure is not supported on legacy");
            return 0f;
        }
        public int GetExposureMode (int camera) {
            Util.LogError("Exposure mode is not supported on legacy");
            return 0;
        }
        public int GetFocusMode (int camera) {
            Util.LogError("Focus mode is not supported on legacy");
            return 0;
        }
        public int GetFlash (int camera) {
            Util.LogError("Flash is not supported on legacy");
            return 0;
        }
        public int GetTorch (int camera) {
            Util.LogError("Torch is not supported on legacy");
            return 0;
        }
        public float GetZoom (int camera) {
            Util.LogError("Zoom is not supported on legacy");
            return 0f;
        }
        #endregion


        #region --Setters--
        public void SetPreviewResolution (int camera, int width, int height) {
            formats[camera] = formats[camera] & ~int.MaxValue | (long)width | (long)height << 16;
        }

        public void SetPhotoResolution (int camera, int width, int height) {
            Util.LogError("Photo resolution is not supported on legacy");
        }

        public void SetFramerate (int camera, float framerate) {
            formats[camera] = formats[camera] & int.MaxValue | (long)framerate << 32;
        }

        public void SetFocus (int camera, float x, float y) {
            Util.LogError("Focus is not supported on legacy");
        }

        public void SetExposure (int camera, float bias) {
            Util.LogError("Exposure is not supported on legacy");
        }

        public void SetExposureMode (int camera, int state) {
            Util.LogError("Exposure mode is not supported on legacy");
        }

        public void SetFocusMode (int camera, int state) {
            Util.LogError("Focus mode is not supported on legacy");
        }

        public void SetFlash (int camera, int state) {
            Util.LogError("Flash is not supported on legacy");
        }

        public void SetTorch (int camera, int state) {
            Util.LogError("Torch is not supported on legacy");
        }
        public void SetZoom (int camera, float ratio) {
            Util.LogError("Zoom is not supported on legacy");
        }
        #endregion


        #region --Ctor--
        public NatCamDeviceLegacy () {
            formats = new Dictionary<int, long>();
            for (int i = 0; i < WebCamTexture.devices.Length; i++) formats.Add(i, 0L);
        }
        #endregion
    }
}
#pragma warning restore 0675