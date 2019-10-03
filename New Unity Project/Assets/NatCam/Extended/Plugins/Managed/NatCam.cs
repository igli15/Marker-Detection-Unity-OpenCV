/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define EXPERIMENTAL_TEXT_DETECTION // Uncomment this to have access to the Text Detection API on Android

namespace NatCamU.Core {

    using UnityEngine;
    using Dispatch;
    using Extended;
    using Utilities;
    using Util = Utilities.Utilities;

    public static partial class NatCam {

        #if NATCAM_EXTENDED

        #region --Properties--

        /// <summary>
        /// Does this platform support detecting metadata?
        /// </summary>
        [ExtDoc(99)]
        public static bool SupportsMetadata {
            get {
                return Implementation.SupportsMetadata;
            }
        }
        #endregion
        

        #region --Operations--

        /// <summary>
        /// Detect a barcode
        /// </summary>
        /// <param name="callback">Callback to be invoked with the detected barcode</param>
        /// <param name="format">Desired format of the barcode</param>
        /// <param name="detectOnce">If true, at most one barcode will be detected</param>
        [ExtDoc(100), ExtCode(20)]
        public static void DetectBarcode (MetadataCallback<Barcode> callback, BarcodeFormat format = BarcodeFormat.ALL, bool detectOnce = true) {
            if (!Implementation.SupportsMetadata) {
                Util.LogError("Current platform does not support metadata detection");
                return;
            }
            if (callback == null) {
                Util.LogError("Cannot detect barcode with no callback");
                return;
            }
            MetadataCallback<IMetadata> callbackWrapper = null;
            callbackWrapper = barcode => {
                if (!(barcode is Barcode)) return;
                if ((((Barcode)barcode).format & format) == 0) return;
                if (detectOnce) Implementation.OnMetadata -= callbackWrapper;
                callback ((Barcode)barcode);
            };
            Implementation.OnMetadata += callbackWrapper;
        }

        /// <summary>
        /// Detect a face
        /// </summary>
        /// <param name="callback">Callback to be invoked with the detected face</param>
        public static void DetectFace (MetadataCallback<Face> callback) { // NCDOC
            if (!Implementation.SupportsMetadata) {
                Util.LogError("Current platform does not support metadata detection");
                return;
            }
            if (callback == null) {
                Util.LogError("Cannot detect face with no callback");
                return;
            }
            MetadataCallback<IMetadata> callbackWrapper = null;
            callbackWrapper = face => {
                if (face is Face) callback ((Face)face);
            };
            Implementation.OnMetadata += callbackWrapper;
        }

        #if EXPERIMENTAL_TEXT_DETECTION
        /// <summary>
        /// Detect a text
        /// </summary>
        /// <param name="callback">Callback to be invoked with the detected text</param>
        /// <param name="detectOnce">If true, at most one text will be detected</param>
        public static void DetectText (MetadataCallback<Text> callback, bool detectOnce = true) { // NCDOC
            if (!Implementation.SupportsMetadata) {
                Util.LogError("Current platform does not support metadata detection");
                return;
            }
            if (callback == null) {
                Util.LogError("Cannot detect text with no callback");
                return;
            }
            MetadataCallback<IMetadata> callbackWrapper = null;
            callbackWrapper = text => {
                if (!(text is Text)) return;
                if (detectOnce) Implementation.OnMetadata -= callbackWrapper;
                callback ((Text)text);
            };
            Implementation.OnMetadata += callbackWrapper;
        }
        #endif

        /// <summary>
        /// Save a photo to the device
        /// </summary>
        /// <param name="photo">The photo to be saved</param>
        /// <param name="mode">Where the photo should be saved</param>
        /// <param name="orientation">The orientation the photo should be saved with</param>
        /// <param name="callback">The callback to be invoked once the photo has been saved</param>
        [ExtDoc(101), ExtCode(21), ExtCode(23)]
        public static void SavePhoto (Texture2D photo, SaveMode mode = SaveMode.SaveToAppDocuments, Orientation orientation = 0, SaveCallback callback = null) {
            if (photo == null) return;
            if (orientation != 0) {
                IDispatch main = new MainDispatch(), worker = new ConcurrentDispatch();
                Util.RotateImage(Texture2D.Instantiate(photo), orientation, worker, main, (rotated, unused) => {
                    Implementation.SavePhoto(rotated.EncodeToPNG(), mode, callback);
                    Texture2D.Destroy(rotated);
                    main.Release(); worker.Release();
                });
            } else Implementation.SavePhoto(photo.EncodeToPNG(), mode, callback);
        }
        #endregion

        #endif
    }
}