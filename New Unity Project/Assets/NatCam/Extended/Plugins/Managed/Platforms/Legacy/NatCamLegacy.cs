/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

// Barcode detection built upon ZXing.NET 0.14.0.0
#define ZXING_API

#if ZXING_API
using ZXing;
#endif

namespace NatCamU.Core.Platforms {

    using UnityEngine;
    using System;
    using Extended;
    using Dispatch;
    using Utilities;
    using Util = NatCamU.Core.Utilities.Utilities;
    using UtilExt = NatCamU.Extended.Utilities.Utilities;

    public sealed partial class NatCamLegacy {

        #region --Op vars--
        public event MetadataCallback<IMetadata> OnMetadata;
        private Color32[] metadataBuffer, dispatchBuffer;
        private IDispatch metadataDispatch;
        private readonly object bufferFence = new object();
        #endregion
        

        #region --Properties--
        public bool SupportsMetadata {
            get {
                return true;
            }
        }
        #endregion


        #region --Client API--

        public void SavePhoto (byte[] png, SaveMode mode, SaveCallback callback) {
            if (((int)mode & (int)SaveMode.SaveToAppDocuments) == (int)SaveMode.SaveToAppDocuments) UtilExt.SavePhoto(png, SaveMode.SaveToAppDocuments, callback);
            if (((int)mode & (int)SaveMode.SaveToPhotoGallery) == (int)SaveMode.SaveToPhotoGallery) Util.LogError("Legacy only supports saving to app documents");
            if (((int)mode & (int)SaveMode.SaveToPhotoAlbum) == (int)SaveMode.SaveToPhotoAlbum) Util.LogError("Legacy only supports saving to app documents");
        }

        public long MetadataTime (long timestamp) {
            return UtilExt.CurrentTime - timestamp;
        }
        #endregion


        #region --State Management--

        private void MetadataUpdate () {
            int width = preview.width, height = preview.height, size = width * height;
            lock (bufferFence) {
                if (metadataBuffer != null && metadataBuffer.Length != size) metadataBuffer = null;
                metadataBuffer = metadataBuffer ?? new Color32[size];
                preview.GetPixels32(metadataBuffer);
            }
            metadataDispatch.Dispatch(() => {
                if (dispatchBuffer != null && dispatchBuffer.Length != size) dispatchBuffer = null;
                dispatchBuffer = dispatchBuffer ?? new Color32[size];
                lock (bufferFence) Array.Copy(metadataBuffer, dispatchBuffer, size);
                DetectBarcode(dispatchBuffer, width, height, dispatch, OnMetadata);
            });
        }

        private void SetDetection (bool state) {
            if (state) {
                OnFrame -= MetadataUpdate;
                OnFrame += MetadataUpdate;
                metadataDispatch = metadataDispatch ?? new ConcurrentDispatch();
            }
            else {
                OnFrame -= MetadataUpdate;
                OnMetadata = null;
                if (metadataDispatch != null) metadataDispatch.Release(); metadataDispatch = null;
            }
        }
        #endregion


        #region --Metadata--

        /// <summary>
        /// Detect barcodes in a preview frame
        /// </summary>
        /// <param name="frame">The preview frame</param>
        /// <param name="width">Width of the preview frame</param>
        /// <param name="height">Height of the preview frame</param>
        /// <param name="dispatch">Dispatcher which will be used to invoke callback</param>
        /// <param name="callback">Callback to be invoked for each barcode detected</param>
        [ExtDoc(161, 4), ExtCode(22)]
        public static void DetectBarcode (Color32[] frame, int width, int height, IDispatch dispatch, MetadataCallback<IMetadata> callback) {
            if (frame == null || dispatch == null || callback == null) return;
            #if ZXING_API
            var results = reader.DecodeMultiple(frame, width, height);
            dispatch.Dispatch(() => results.ForEach(result => callback(NatCamBarcode(result))));
            #endif
        }

        #if ZXING_API

        private static BarcodeReader reader {
            get {
                if (_reader != null) return _reader;
                _reader = new BarcodeReader();
                //_reader.Options.TryHarder =
                _reader.TryInverted =
                _reader.AutoRotate = true;
                return _reader;
            }
        }
        private static BarcodeReader _reader;

        private static Barcode NatCamBarcode (Result result) {
            Func<ZXing.BarcodeFormat, BarcodeFormat> Format = format => {
                switch (format) {
                    case ZXing.BarcodeFormat.QR_CODE : return BarcodeFormat.QR;
                    case ZXing.BarcodeFormat.EAN_13 : return BarcodeFormat.EAN_13;
                    case ZXing.BarcodeFormat.EAN_8 : return BarcodeFormat.EAN_8;
                    case ZXing.BarcodeFormat.DATA_MATRIX : return BarcodeFormat.DATA_MATRIX;
                    case ZXing.BarcodeFormat.PDF_417 : return BarcodeFormat.PDF_417;
                    case ZXing.BarcodeFormat.CODE_128 : return BarcodeFormat.CODE_128;
                    case ZXing.BarcodeFormat.CODE_93 : return BarcodeFormat.CODE_93;
                    case ZXing.BarcodeFormat.CODE_39 : return BarcodeFormat.CODE_39;
                    case ZXing.BarcodeFormat.ITF : return BarcodeFormat.ITF;
                    default : return BarcodeFormat.ALL;
                }
            };
            ResultPoint[] points = result.ResultPoints;
            float
            x = points[1].X, y = points[1].Y,
            w = points[2].X - x, h = y - points[0].Y;
            return new Barcode(x, y, w, h, result.Timestamp / TimeSpan.TicksPerMillisecond, null, result.Text, (int)Format(result.BarcodeFormat));
        }
        #endif
        #endregion
    }
}