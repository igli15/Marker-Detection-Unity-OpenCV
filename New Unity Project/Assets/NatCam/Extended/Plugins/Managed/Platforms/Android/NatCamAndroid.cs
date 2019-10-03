/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using UnityEngine;
    using System;
    using Extended;

    public sealed partial class NatCamAndroid {

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
        private static AndroidJavaClass natcamextended;
        private static readonly string AlbumName = Application.productName;
        #endregion
        

        #region --Properties--
        public bool SupportsMetadata {
            get {
                return natcamextended.CallStatic<bool>("supportsMetadata");
            }
        }
        #endregion


        #region --Client API--

        protected override void SavePhoto (byte[] png, SaveMode mode, int callback) {
            if (png == null) return;
            natcamextended.CallStatic("setAlbumName", AlbumName);
            IntPtr jpeg = AndroidJNI.ToByteArray(png);
            jvalue[] wrapper = new jvalue[3];
            wrapper[0].l = jpeg; wrapper[1].i = (int)mode; wrapper[2].i = callback;
            IntPtr methodID = AndroidJNIHelper.GetMethodID(natcamextended.GetRawClass(), "savePhoto", "([BII)V", true);
            AndroidJNI.CallStaticVoidMethod(natcamextended.GetRawClass(), methodID, wrapper);
        }

        public long MetadataTime (long timestamp) {
            return natcamextended.CallStatic<long>("getMetadataTime", timestamp);
        }
        #endregion
    }
}