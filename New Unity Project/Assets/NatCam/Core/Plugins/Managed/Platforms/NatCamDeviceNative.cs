/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using System.Runtime.InteropServices;

    public static class NatCamDeviceNative {

        private const string Assembly =
        #if UNITY_IOS
        "__Internal";
        #else
        "NatCam";
        #endif

        #if INATCAM_C

        #region --Properties--
        [DllImport(Assembly)]
        public static extern bool IsRearFacing (int camera);
        [DllImport(Assembly)]
        public static extern bool IsFlashSupported (int camera);
        [DllImport(Assembly)]
        public static extern bool IsTorchSupported (int camera);
        [DllImport(Assembly)]
        public static extern float HorizontalFOV (int camera);
        [DllImport(Assembly)]
        public static extern float VerticalFOV (int camera);
        [DllImport(Assembly)]
        public static extern float MinExposureBias (int camera);
        [DllImport(Assembly)]
        public static extern float MaxExposureBias (int camera);
        [DllImport(Assembly)]
        public static extern float MaxZoomRatio (int camera);
        #endregion


        #region --Getters--

        [DllImport(Assembly)]
        public static extern void GetPreviewResolution (int camera, out int width, out int height);
        [DllImport(Assembly)]
        public static extern void GetPhotoResolution (int camera, out int width, out int height);
        [DllImport(Assembly)]
        public static extern float GetFramerate (int camera);
        [DllImport(Assembly)]
        public static extern float GetExposure (int camera);
        [DllImport(Assembly)]
        public static extern int GetExposureMode (int camera);
        [DllImport(Assembly)]
        public static extern int GetFocusMode (int camera);
        [DllImport(Assembly)]
        public static extern int GetFlash (int camera);
        [DllImport(Assembly)]
        public static extern int GetTorch (int camera);
        [DllImport(Assembly)]
        public static extern float GetZoom (int camera);
        #endregion


        #region --Setters--
        [DllImport(Assembly)]
        public static extern void SetPreviewResolution (int camera, int width, int height);
        [DllImport(Assembly)]
        public static extern void SetPhotoResolution (int camera, int width, int height);
        [DllImport(Assembly)]
        public static extern void SetFramerate (int camera, float framerate);
        [DllImport(Assembly)]
        public static extern bool SetFocus (int camera, float x, float y);
        [DllImport(Assembly)]
        public static extern float SetExposure (int camera, float bias);
        [DllImport(Assembly)]
        public static extern bool SetFocusMode (int camera, int state);
        [DllImport(Assembly)]
        public static extern bool SetExposureMode (int camera, int state);
        [DllImport(Assembly)]
        public static extern bool SetFlash (int camera, int state);
        [DllImport(Assembly)]
        public static extern bool SetTorch (int camera, int state);
        [DllImport(Assembly)]
        public static extern bool SetZoom (int camera, float ratio);
        #endregion


        #else
        public static bool IsRearFacing (int camera) {return true;}
        public static bool IsFlashSupported (int camera) {return false;}
        public static bool IsTorchSupported (int camera) {return false;}
        public static float HorizontalFOV (int camera) {return 0;}
        public static float VerticalFOV (int camera) {return 0;}
        public static float MinExposureBias (int camera) {return 0;}
        public static float MaxExposureBias (int camera) {return 0;}
        public static float MaxZoomRatio (int camera) {return 1;}
        public static void GetPreviewResolution (int camera, out int width, out int height) {width = height = 0;}
        public static void GetPhotoResolution (int camera, out int width, out int height) {width = height = 0;}
        public static float GetFramerate (int camera) {return 0;}
        public static float GetExposure (int camera) {return 0;}
        public static int GetExposureMode (int camera) {return 0;}
        public static int GetFocusMode (int camera) {return 0;}
        public static int GetFlash (int camera) {return 0;}
        public static int GetTorch (int camera) {return 0;}
        public static float GetZoom (int camera) {return 0;}
        public static void SetPreviewResolution (int camera, int width, int height) {}
        public static void SetPhotoResolution (int camera, int width, int height) {}
        public static void SetFramerate (int camera, float framerate) {}
        public static bool SetFocus (int camera, float x, float y) {return false;}
        public static float SetExposure (int camera, float bias) {return 0;}
        public static bool SetFocusMode (int camera, int state) {return false;}
        public static bool SetExposureMode (int camera, int state) {return false;}
        public static bool SetFlash (int camera, int state) {return false;}
        public static bool SetTorch (int camera, int state) {return false;}
        public static bool SetZoom (int camera, float ratio) {return false;}
        #endif
    }
}