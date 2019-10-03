/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;

namespace NatCamU.Core.Platforms {

    public class NatCamDeviceAndroid : INatCamDevice {

        #region --Properties--
        public bool IsRearFacing (int camera) {
            return natcam.CallStatic<bool>("isRearFacing", camera);
        }

        public bool IsFlashSupported (int camera) {
            return natcam.CallStatic<bool>("isFlashSupported", camera);
        }

        public bool IsTorchSupported (int camera) {
            return natcam.CallStatic<bool>("isTorchSupported", camera);
        }

        public float HorizontalFOV (int camera) {
            return natcam.CallStatic<float>("horizontalFOV", camera);
        }

        public float VerticalFOV (int camera) {
            return natcam.CallStatic<float>("verticalFOV", camera);
        }

        public float MinExposureBias (int camera) {
            return natcam.CallStatic<float>("minExposureBias", camera);
        }

        public float MaxExposureBias (int camera) {
            return natcam.CallStatic<float>("maxExposureBias", camera);
        }

        public float MaxZoomRatio (int camera) {
            return natcam.CallStatic<float>("maxZoomRatio", camera);
        }
        #endregion


        #region --Getters--
        public void GetPreviewResolution (int camera, out int width, out int height) {
            width = height = 0;
            AndroidJavaObject jRet = natcam.CallStatic<AndroidJavaObject>("getPreviewResolution", camera);
            if (jRet.GetRawObject().ToInt32() != 0) {
                int[] res = AndroidJNIHelper.ConvertFromJNIArray<int[]>(jRet.GetRawObject());
                width = res[0]; height = res[1];
            }
        }

        public void GetPhotoResolution (int camera, out int width, out int height) {
            width = height = 0;
            AndroidJavaObject jRet = natcam.CallStatic<AndroidJavaObject>("getPhotoResolution", camera);
            if (jRet.GetRawObject().ToInt32() != 0) {
                int[] res = AndroidJNIHelper.ConvertFromJNIArray<int[]>(jRet.GetRawObject());
                width = res[0]; height = res[1];
            }
        }

        public float GetFramerate (int camera) {
            return natcam.CallStatic<float>("getFramerate", camera);
        }
        
        public float GetExposure (int camera) {
            return natcam.CallStatic<float>("getExposure", camera);
        }

        public int GetExposureMode (int camera) {
            return natcam.CallStatic<int>("getExposureMode", camera);
        }

        public int GetFocusMode (int camera) {
            return natcam.CallStatic<int>("getFocusMode", camera);
        }

        public int GetFlash (int camera) {
            return natcam.CallStatic<int>("getFlash", camera);
        }

        public int GetTorch (int camera) {
            return natcam.CallStatic<int>("getTorch", camera);
        }
        
        public float GetZoom (int camera) {
            return natcam.CallStatic<float>("getZoom", camera);
        }
        #endregion


        #region --Setters--
        public void SetPreviewResolution (int camera, int width, int height) {
            natcam.CallStatic("setPreviewResolution", camera, width, height);
        }

        public void SetPhotoResolution (int camera, int width, int height) {
            natcam.CallStatic("setPhotoResolution", camera, width, height);
        }

        public void SetFramerate (int camera, float framerate) {
            natcam.CallStatic("setFramerate", camera, framerate);
        }

        public void SetFocus (int camera, float x, float y) {
            natcam.CallStatic("setFocus", camera, x, y);
        }

        public void SetExposure (int camera, float bias) {
            natcam.CallStatic("setExposure", camera, bias);
        }

        public void SetExposureMode (int camera, int state) {
            natcam.CallStatic("setExposureMode", camera, state);
        }

        public void SetFocusMode (int camera, int state) {
            natcam.CallStatic("setFocusMode", camera, state);
        }

        public void SetFlash (int camera, int state) {
            natcam.CallStatic("setFlash", camera, state);
        }

        public void SetTorch (int camera, int state) {
            natcam.CallStatic("setTorch", camera, state);
        }
        public void SetZoom (int camera, float ratio) {
            natcam.CallStatic("setZoom", camera, ratio);
        }
        #endregion
        

        #region --Interop--

        private readonly AndroidJavaClass natcam;

        public NatCamDeviceAndroid (AndroidJavaClass natcam) {
            this.natcam = natcam;
        }
        #endregion
    }
}