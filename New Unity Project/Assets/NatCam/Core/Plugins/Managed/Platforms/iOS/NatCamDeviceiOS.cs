/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using Native = NatCamDeviceNative;

    public class NatCamDeviceiOS : INatCamDevice {

        #region --Properties--
        public bool IsRearFacing (int camera) {
            return Native.IsRearFacing(camera);
        }

        public bool IsFlashSupported (int camera) {
            return Native.IsFlashSupported(camera);
        }

        public bool IsTorchSupported (int camera) {
            return Native.IsTorchSupported(camera);
        }

        public float HorizontalFOV (int camera) {
            return Native.HorizontalFOV(camera);
        }

        public float VerticalFOV (int camera) {
            return Native.VerticalFOV(camera);
        }

        public float MinExposureBias (int camera) {
            return Native.MinExposureBias(camera);
        }

        public float MaxExposureBias (int camera) {
            return Native.MaxExposureBias(camera);
        }

        public float MaxZoomRatio (int camera) {
            return Native.MaxZoomRatio(camera);
        }
        #endregion


        #region --Getters--
        public void GetPreviewResolution (int camera, out int width, out int height) {
            Native.GetPreviewResolution(camera, out width, out height);
        }

        public void GetPhotoResolution (int camera, out int width, out int height) {
            Native.GetPhotoResolution(camera, out width, out height);
        }

        public float GetFramerate (int camera) {
            return Native.GetFramerate(camera);
        }
        
        public float GetExposure (int camera) {
            return Native.GetExposure(camera);
        }
        public int GetExposureMode (int camera) {
            return Native.GetExposureMode(camera);
        }
        public int GetFocusMode (int camera) {
            return Native.GetFocusMode(camera);
        }
        public int GetFlash (int camera) {
            return Native.GetFlash(camera);
        }
        public int GetTorch (int camera) {
            return Native.GetTorch(camera);
        }
        public float GetZoom (int camera) {
            return Native.GetZoom(camera);
        }
        #endregion


        #region --Setters--
        
        public void SetPreviewResolution (int camera, int width, int height) {
            Native.SetPreviewResolution(camera, width, height);
        }

        public void SetPhotoResolution (int camera, int width, int height) {
            Native.SetPhotoResolution(camera, width, height);
        }

        public void SetFramerate (int camera, float framerate) {
            Native.SetFramerate(camera, framerate);
        }

        public void SetFocus (int camera, float x, float y) {
            Native.SetFocus(camera, x, y);
        }

        public void SetExposure (int camera, float bias) {
            Native.SetExposure(camera, bias);
        }

        public void SetExposureMode (int camera, int state) {
            Native.SetExposureMode(camera, state);
        }

        public void SetFocusMode (int camera, int state) {
            Native.SetFocusMode(camera, state);
        }

        public void SetFlash (int camera, int state) {
            Native.SetFlash(camera, state);
        }

        public void SetTorch (int camera, int state) {
            Native.SetTorch(camera, state);
        }
        public void SetZoom (int camera, float ratio) {
            Native.SetZoom(camera, ratio);
        }
        #endregion
    }
}