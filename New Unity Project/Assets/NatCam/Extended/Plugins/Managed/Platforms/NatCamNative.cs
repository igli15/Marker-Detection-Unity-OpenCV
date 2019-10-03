/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using System;
    using System.Runtime.InteropServices;

    public static partial class NatCamNative {

        private const string ExtAssembly =
        #if UNITY_IOS
        "__Internal";
        #else
        "NatCamExtended";
        #endif

        #region ---Delegates---
        public delegate void BarcodeCallback (float x, float y, float w, float h, long timestamp, IntPtr supplement, IntPtr data, int len, int format);
        public delegate void FaceCallback (float x, float y, float w, float h, long timestamp, IntPtr supplement, int id, float roll, float yaw);
        public delegate void TextCallback (float x, float y, float w, float h, long timestamp, IntPtr supplement, IntPtr text, int len);
        public delegate void SaveCallback (int mode, string path, int callback);
        #endregion

        #region --Initialization--
        [DllImport(ExtAssembly)]
        public static extern void RegisterExtendedCallbacks (BarcodeCallback barcodeCallback, FaceCallback faceCallback, TextCallback textCallback, SaveCallback saveCallback);
        #endregion
    }
}