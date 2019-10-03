/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    public interface INatCamDevice {
        
        #region --Properties--
        bool IsRearFacing (int camera);
        bool IsFlashSupported (int camera);
        bool IsTorchSupported (int camera);
        float HorizontalFOV (int camera);
        float VerticalFOV (int camera);
        float MinExposureBias (int camera);
        float MaxExposureBias (int camera);
        float MaxZoomRatio (int camera);
        #endregion

        #region --Getters--
        void GetPreviewResolution (int camera, out int width, out int height);
        void GetPhotoResolution (int camera, out int width, out int height);
        float GetFramerate (int camera);
        float GetExposure (int camera);
        int GetExposureMode (int camera);
        int GetFocusMode (int camera);
        int GetFlash (int camera);
        int GetTorch (int camera);
        float GetZoom (int camera);
        #endregion

        #region --Setters--
        void SetPreviewResolution (int camera, int width, int height);
        void SetPhotoResolution (int camera, int pWidth, int pHeight);
        void SetFramerate (int camera, float framerate);
        void SetFocus (int camera, float x, float y);
        void SetExposure (int camera, float bias);
        void SetExposureMode (int camera, int state);
        void SetFocusMode (int camera, int state);
        void SetFlash (int camera, int state);
        void SetTorch (int camera, int state);
        void SetZoom (int camera, float ratio);
        #endregion
    }
}