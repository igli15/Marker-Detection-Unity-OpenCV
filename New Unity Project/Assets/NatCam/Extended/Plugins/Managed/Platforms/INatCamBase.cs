/* 
*   NatCam Extended
*   Copyright (c) 2017 Yusuf Olokoba
*/

#pragma warning disable 0642

namespace NatCamU.Core.Platforms {

    using AOT;
    using System;
    using System.Runtime.InteropServices;
    using Extended;
    using Util = Utilities.Utilities;
    using UtilExt = Extended.Utilities.Utilities;
    using SaveCallbacks = System.Collections.Generic.Dictionary<int, Extended.SaveCallback>;

    public partial class INatCamBase {

        #region --Events--
        protected MetadataCallback<IMetadata> onMetadata;
        protected SaveCallbacks saveCallbacks = new SaveCallbacks();
        #endregion


        #region --Client API--

        public void SavePhoto (byte[] jpg, SaveMode mode, SaveCallback callback) {
            Func<int> AddCallback = () => {
                if (callback == null) return -1;
                int callbackID = saveCallbacks.Count;
                saveCallbacks.Add(callbackID, callback);
                return callbackID;
            };
            if (((int)mode & (int)SaveMode.SaveToAppDocuments) == (int)SaveMode.SaveToAppDocuments) UtilExt.SavePhoto(jpg, SaveMode.SaveToAppDocuments, callback);
            if (((int)mode & (int)SaveMode.SaveToPhotoGallery) == (int)SaveMode.SaveToPhotoGallery) SavePhoto(jpg, SaveMode.SaveToPhotoGallery, AddCallback());
            if (((int)mode & (int)SaveMode.SaveToPhotoAlbum) == (int)SaveMode.SaveToPhotoAlbum) SavePhoto(jpg, SaveMode.SaveToPhotoAlbum, AddCallback());
        }

        protected abstract void SavePhoto (byte[] jpg, SaveMode mode, int callback);
        #endregion

        
        #region --Native Callbacks--

        [MonoPInvokeCallback(typeof(NatCamNative.BarcodeCallback))]
        protected static void OnBarcode (float x, float y, float w, float h, long timestamp, IntPtr supplement, IntPtr data, int len, int format) {
            if (data == IntPtr.Zero) {Util.LogError("Detected barcode string does not exist"); return;}
            string code = Marshal.PtrToStringUni(data, len);
            if (code == null) {Util.LogError("Detected barcode string is null"); return;}
            object[] _supplement = null;
            if (supplement != IntPtr.Zero) ; // Unimplemented
            instance.dispatch.Dispatch(() => {
                if (instance.onMetadata != null) instance.onMetadata(new Barcode(x, y, w, h, timestamp, _supplement, code, format));
            });
        }

        [MonoPInvokeCallback(typeof(NatCamNative.FaceCallback))]
        protected static void OnFace (float x, float y, float w, float h, long timestamp, IntPtr supplement, int id, float roll, float yaw) {
            byte[] landmarkData = null;
            if (supplement != IntPtr.Zero) { // DEPLOY
                int length = Marshal.ReadByte(supplement) * 9;
                landmarkData = new byte[length];
                Marshal.Copy(new IntPtr(supplement.ToInt64() + 1), landmarkData, 0, length);
            }
            instance.dispatch.Dispatch(() => {
                if (instance.onMetadata != null) instance.onMetadata(new Face(x, y, w, h, timestamp, landmarkData, id, roll, yaw));
            });
        }

        [MonoPInvokeCallback(typeof(NatCamNative.TextCallback))]
        protected static void OnText (float x, float y, float w, float h, long timestamp, IntPtr supplement, IntPtr data, int len) {
            if (data == IntPtr.Zero) {Util.LogError("Detected text string does not exist"); return;}
            string text = Marshal.PtrToStringUni(data, len);
            if (text == null) {Util.LogError("Detected text string is null"); return;}
            object[] _supplement = null;
            if (supplement != IntPtr.Zero) ; // Unimplemented
            instance.dispatch.Dispatch(() => {
                if (instance.onMetadata != null) instance.onMetadata(new Text(x, y, w, h, timestamp, _supplement, text));
            });
        }

        [MonoPInvokeCallback(typeof(NatCamNative.SaveCallback))]
        protected static void OnSave (int mode, string path, int callback) {
            instance.dispatch.Dispatch(() => {
                if (!instance.saveCallbacks.ContainsKey(callback)) return;
                SaveCallback saveCallback = instance.saveCallbacks[callback];
                instance.saveCallbacks.Remove(callback);
                saveCallback((SaveMode)mode, path);
            });
        }
        #endregion
    }
}
#pragma warning restore 0642