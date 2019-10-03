/* 
*   NatCam Professional
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using System;
    using System.Runtime.InteropServices;

    public static partial class NatCamNative {

        private const string ProAssembly =
        #if UNITY_IOS
        "__Internal";
        #else
        "NatCamProfessional";
        #endif

        #region --Preview Data--
        [DllImport(ProAssembly)]
        public static extern void RegisterProfessionalCallbacks (SaveCallback saveCallback);
        [DllImport(ProAssembly)]
        public static extern void InitializePreviewBuffer ();
        [DllImport(ProAssembly)]
        public static extern void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size);
        [DllImport(ProAssembly)]
        public static extern void ReleasePreviewBuffer ();
        #endregion

        #if INATCAM_C

        #region --Recording--
        [DllImport(ProAssembly)]
        public static extern bool IsRecording ();
        [DllImport(ProAssembly)]
        public static extern void StartRecording ();
        [DllImport(ProAssembly)]
        public static extern void StopRecording ();
        #endregion


        #else
        public static bool IsRecording () {return false;}
        public static void StartRecording () {}
        public static void StopRecording () {}
        #endif
    }
}