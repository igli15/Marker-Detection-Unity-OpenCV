/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core {

    using UnityEngine;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Platforms;
    using Utilities;

    [CoreDoc(96)]
    public sealed class DeviceCamera {

        #region --Getters--

        /// <summary>
        /// Get the facing of the camera
        /// </summary>
        [CoreDoc(11)]
        public Facing Facing {
            get {
                return Device.IsRearFacing(this) ? Facing.Rear : Facing.Front;
            }
        }
        /// <summary>
        /// Get the current preview resolution of the camera
        /// </summary>
        [CoreDoc(12)]
        public Vector2 PreviewResolution {
            get {
                int x, y;
                Device.GetPreviewResolution(this, out x, out y);
                return new Vector2(x, y);
            }
        }
        /// <summary>
        /// Get the current photo resolution of the camera
        /// </summary>
        [CoreDoc(13)]
        public Vector2 PhotoResolution {
            get {
                int x, y;
                Device.GetPhotoResolution(this, out x, out y);
                return new Vector2(x, y);
            }
        }
        /// <summary>
        /// Get the current framerate of the camera
        /// </summary>
        public float Framerate { // NCDOC
            get {
                return Device.GetFramerate(this);
            }
        }
        /// <summary>
        /// Does this camera support flash?
        /// </summary>
        [CoreDoc(14)]
        public bool IsFlashSupported {
            get {
                return Device.IsFlashSupported(this);
            }
        }
        /// <summary>
        /// Does this camera support torch?
        /// </summary>
        [CoreDoc(15)]
        public bool IsTorchSupported {
            get {
                return Device.IsTorchSupported(this);
            }
        }
        /// <summary>
        /// Get the camera's horizontal field-of-view
        /// </summary>
        [CoreDoc(17)]
        public float HorizontalFOV {
            get {
                return Device.HorizontalFOV(this);
            }
        }
        /// <summary>
        /// Get the camera's vertical field-of-view
        /// </summary>
        [CoreDoc(18)]
        public float VerticalFOV {
            get {
                return Device.VerticalFOV(this);
            }
        }
        /// <summary>
        /// Get the camera's minimum exposure bias
        /// </summary>
        [CoreDoc(19)]
        public float MinExposureBias {
            get {
                return Device.MinExposureBias(this);
            }
        }
        /// <summary>
        /// Get the camera's maximum exposure bias
        /// </summary>
        [CoreDoc(20)]
        public float MaxExposureBias {
            get {
               return Device.MaxExposureBias(this);
            }
        }
        /// <summary>
        /// Get the camera's maximum zoom ratio
        /// </summary>
        public float MaxZoomRatio { // NCDOC
            get {
                return Device.MaxZoomRatio(this);
            }
        }
        #endregion


        #region ---Properties---

        /// <summary>
        /// Get or set the camera's focus mode
        /// </summary>
        [CoreDoc(21), CoreCode(10)]
        public FocusMode FocusMode {
            get {
                return (FocusMode)Device.GetFocusMode(this);
            }
            set {
                Device.SetFocusMode(this, (int)value);
            }
        }
        /// <summary>
        /// Get or set the camera's exposure mode
        /// </summary>
        [CoreDoc(22), CoreCode(11)]
        public ExposureMode ExposureMode {
            get {
                return (ExposureMode)Device.GetExposureMode(this);
            }
            set {
                Device.SetExposureMode(this, (int)value);
            }
        }
        /// <summary>
        /// Get or set the camera's exposure bias
        /// </summary>
        [CoreDoc(23, 7), CoreCode(12)]
        public float ExposureBias {
            get {
                return Device.GetExposure(this);
            }
            set {
                Device.SetExposure(this, value);
            }
        }
        /// <summary>
        /// Get or set the camera's flash mode when taking a picture
        /// </summary>
        [CoreDoc(24)]
        public FlashMode FlashMode {
            get {
                return (FlashMode)Device.GetFlash(this);
            }
            set {
                Device.SetFlash(this, (int)value);
            }
        }
        /// <summary>
        /// Get or set the camera's torch mode
        /// </summary>
        [CoreDoc(25)]
        public Switch TorchMode {
            get {
                return (Switch)Device.GetTorch(this);
            }
            set {
                Device.SetTorch(this, (int)value);
            }
        }
        /// <summary>
        /// Get or set the camera's current zoom ratio. This value must be between [1, MaxZoomRatio]
        /// </summary>
        [CoreDoc(26)]
        public float ZoomRatio {
            get {
                return Device.GetZoom(this);
            }
            set {
                Device.SetZoom(this, value);
            }
        }
        #endregion


        #region --Op vars--
        private readonly int index;
        #endregion


        #region --Ops--

        /// <summary>
        /// Set the camera's focus point of interest
        /// </summary>
        [CoreDoc(27, 0), CoreCode(9)]
        public void SetFocus (Vector2 viewportPoint) {
            Device.SetFocus(this, viewportPoint.x, viewportPoint.y);
        }
        
        /// <summary>
        /// Set the camera's frame rate
        /// </summary>
        [CoreDoc(29), CoreCode(7)]
        public void SetFramerate (FrameratePreset preset) {
            SetFramerate((float)preset);
        }
        
        /// <summary>
        /// Set the camera's frame rate
        /// </summary>
        [CoreDoc(28), CoreCode(8)]
        public void SetFramerate (float framerate) {
            Device.SetFramerate(this, framerate);
        }

        /// <summary>
        /// Set the camera's preview resolution
        /// </summary>
        [CoreDoc(31), CoreCode(5)]
        public void SetPreviewResolution (ResolutionPreset preset) {
            int width;
            int height;
            preset = preset == ResolutionPreset.HighestResolution ? ResolutionPreset.FullHD : preset;
            preset.Dimensions(out width, out height);
            SetPreviewResolution(width, height);
        }
        
        /// <summary>
        /// Set the camera's preview resolution
        /// </summary>
        [CoreDoc(30), CoreCode(6)]
        public void SetPreviewResolution (int width, int height) {
            Device.SetPreviewResolution(this, width, height);
        }
        
        /// <summary>
        /// Set the camera's photo resolution
        /// </summary>
        [CoreDoc(33)]
        public void SetPhotoResolution (ResolutionPreset preset) {
            int width;
            int height;
            preset.Dimensions(out width, out height);
            SetPhotoResolution(width, height);            
        }
        
        /// <summary>
        /// Set the camera's photo resolution
        /// </summary>
        [CoreDoc(32)]
        public void SetPhotoResolution (int width, int height) {
            Device.SetPhotoResolution(this, width, height);
        }
        #endregion


        #region ---Typecasting---
        private static INatCamDevice Device {get {return NatCam.Implementation.Device;}}
        
		public static implicit operator int (DeviceCamera cam) {
			return cam ? cam.index : -1;
		}
        public static implicit operator DeviceCamera (int index) {
            return Cameras.Where(c => c.index == index).FirstOrDefault();
        }
        public static implicit operator bool (DeviceCamera cam) {
            return cam != null;
        }
        #endregion


        #region ---Intializers---

        private DeviceCamera (int i) {
            index = i;
        }

        static DeviceCamera () {
            int cameraCount = WebCamTexture.devices.Length;
            DeviceCamera[] cameras = new DeviceCamera[cameraCount];
            for (int i = 0; i < cameraCount; i++) cameras[i] = new DeviceCamera(i);
            Cameras = new ReadOnlyCollection<DeviceCamera>(cameras);
            RearCamera = Cameras.FirstOrDefault(c => c.Facing == Facing.Rear);
            FrontCamera = Cameras.FirstOrDefault(c => c.Facing == Facing.Front);
        }
        #endregion
        
        
		#region ---Statics---
        [CoreDoc(34)]
        public static readonly DeviceCamera FrontCamera;
		[CoreDoc(35)]
        public static readonly DeviceCamera RearCamera;
        [CoreDoc(36), CoreCode(4)]
        public static readonly ReadOnlyCollection<DeviceCamera> Cameras; // You shall not touch!
        #endregion
    }
}