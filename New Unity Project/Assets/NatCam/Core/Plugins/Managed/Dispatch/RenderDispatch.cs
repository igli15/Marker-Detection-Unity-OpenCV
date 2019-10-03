/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

#if UNITY_ANDROID
    #define NATCAM_RENDER_DISPATCH
#endif

namespace NatCamU.Dispatch {

    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;
    using Core.Utilities;

    [CoreDoc(88)]
    public sealed class RenderDispatch : MainDispatch {

        #region --Ctor--

        /// <summary>
        /// Creates a dispatcher that will execute on the render thread
        /// </summary>
        [CoreDoc(89)]
        public RenderDispatch () : base () {
            base.Dispatch(() => 
            #if NATCAM_RENDER_DISPATCH
            GL.IssuePluginEvent(OnRender(), 0),
            #else
            #pragma warning disable 0618
            GL.IssuePluginEvent(0),
            #pragma warning restore 0618
            #endif
            true);
            Utilities.LogVerbose("Initialized render dispatcher");
        }
        #endregion


        #region --Client API--

        /// <summary>
        /// DO NOT USE
        /// </summary>
        [CoreDoc(90)]
        public override void Dispatch (Action action, bool repeat) {}
        #endregion


        #region --Native Interop--
        #if NATCAM_RENDER_DISPATCH
        private const string Assembly =
        #if UNITY_IOS
        "__Internal";
        #else
        "NatCamRenderDispatch";
        #endif

        [DllImport(Assembly)]
        private static extern IntPtr OnRender ();
        #endif
        #endregion
    }
}