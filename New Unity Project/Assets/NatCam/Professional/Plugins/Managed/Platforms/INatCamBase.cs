/* 
*   NatCam Professional
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using AOT;
    using Extended;

    public partial class INatCamBase {

        #region --Op vars--
        protected SaveCallback recordingCallback;
        #endregion


        #region --Operations--
        protected abstract void InitializePreviewBuffer ();
        #endregion


        #region --Native Callbacks--

        [MonoPInvokeCallback(typeof(NatCamNative.SaveCallback))]
        protected static void OnVideo (int mode, string path, int callback) {
            instance.dispatch.Dispatch(() => {
                if (instance.recordingCallback == null) return;
                instance.recordingCallback((SaveMode)mode, path);
                instance.recordingCallback = null;
            });
        }
        #endregion
    }
}