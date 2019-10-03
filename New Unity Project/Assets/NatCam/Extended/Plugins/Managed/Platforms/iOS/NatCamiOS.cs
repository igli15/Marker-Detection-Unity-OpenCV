/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;
    using Extended;
    using Util = Utilities.Utilities;

    public sealed partial class NatCamiOS {

        #region --Events--
        public event MetadataCallback<IMetadata> OnMetadata {
            add {
                onMetadata += value;
            }
            remove {
                onMetadata -= value;
            }
        }
        #endregion


        #region --Op vars--
        private static readonly string AlbumName = Application.productName;
        /*
        * This flag determines whether AVFoundation or CoreImage will be used for detecting faces and texts (where applicable).
        * CoreImage will enable facial landmark detection and is much more accurate than AVFoundation. On the other hand,
        * CoreImage might introduce significant lag on some devices as it is more computationally expensive.
        */
        private const bool UseCoreImageMetadataBackend = false;
        #endregion
        

        #region --Properties--
        public bool SupportsMetadata {
            get {
                return true;
            }
        }
        #endregion


        #region --Client API--

        protected override void SavePhoto (byte[] png, SaveMode mode, int callback) {
            if (png == null) return;
            SetAlbumName(AlbumName);
            int size = Marshal.SizeOf(png[0]) * png.Length;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try {
                Marshal.Copy(png, 0, ptr, png.Length);
                SavePhoto(ptr, unchecked((UIntPtr)(uint)size), (int)mode, callback);
            }
            catch (Exception e) {Util.LogError("Failed to save photo with exception: "+e.Message);}
            finally {Marshal.FreeHGlobal(ptr);}
        }

        public long MetadataTime (long timestamp) {
            return GetMetadataTime(timestamp);
        }
        #endregion


        #region --Native Interop--

        #if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void SetDetection (bool state, bool useCoreImage);
        [DllImport("__Internal")]
        private static extern void SavePhoto (IntPtr buffer, UIntPtr size, int mode, int callback);
        [DllImport("__Internal")]
        private static extern void SetAlbumName (string name);
        [DllImport("__Internal")]
        private static extern long GetMetadataTime (long timestamp);
        
        #else
        private static void SetDetection (bool state, bool useCoreImage) {}
        private static void SavePhoto (IntPtr buffer, UIntPtr size, int mode, int callback) {}
        private static void SetAlbumName (string name) {}
        long GetMetadataTime (long timestamp) {return 0L;}
        #endif
        #endregion
    }
}