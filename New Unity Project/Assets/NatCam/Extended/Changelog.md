## NatCam Extended 1.5f3
+ NatCam Extended now features experimental support for Text Detection on Android.
+ Greatly improved metadata detection speed on Android.
+ Enhanced support for using libraries that depend on Google Play Services by avoiding clashes.
+ NatCam.SavePhoto can now save photos with orientation on all platforms!
+ Added IMetadata interface with rect and timestamp fields.
+ Added MetadataCallback callback for use with IMetadata.
+ Added NatCam.DetectBarcode (MetadataCallback).
+ Added NatCam.DetectFace(MetadataCallback).
+ Added overloads for equality, Equals() and GetHashCode() operations to Face struct.
+ Added NatCamiOS.UseCoreImageMetadataBackend flag, allowing for more precise face tracking.
+ Added bitcode support on iOS.
+ Added FaceCam example showcasing face tracking.
+ Added FaceCam.DrawMetadataRect function for drawing metadata positioning and scaling metadata rect on UI panel.
+ Fixed crash after switching cameras while detecting metadata on Android.
+ Fixed regression where metadata was not detected after the preview was resumed.
+ Fixed bug where metadata was not detected after switching cameras.
+ Fixed crash when using NatCam.SavePhoto on iOS 10+ due to missing NSPhotoLibraryUsageDescription.
+ Fixed metadata position being mirrored on the X axis when using front camera.
+ Fixed bug in NatCam.SavePhoto where photos are not saved if no callback is supplied.
+ Deprecated Faces example.
+ Deprecated NatCamPreview component. Use NatCamU.Core.UI classes instead.
+ Deprectaed NatCam.RequestBarcode.
+ Deprecated NatCam.OnBarcodeDetect and NatCam.OnFaceDetect.
+ Deprecated BarcodeCallback and FaceCallback.
+ Deprecated BarcodeRequest struct.
+ Deprecated SaveOrientation struct. Use NatCamU.Core.Orientation instead.
+ Renamed Scanner example to QRCam.
+ *Everything below*

## NatCam Extended 1.5b4
+ Added FaceCam example showcasing face tracking.
+ NatCam.SavePhoto can now save photos with orientation on all platforms!
+ Added bitcode support on iOS.
+ Added FaceCam.DrawMetadataRect function for drawing metadata positioning and scaling metadata rect on UI panel.
+ Added NatCamiOS.UseCoreImageMetadataBackend flag, allowing for more precise face tracking.
+ Fixed metadata position being mirrored on the X axis when using front camera.
+ Fixed bug in NatCam.SavePhoto where photos are not saved if no callback is supplied.
+ Fixed crash when the text detection backend is enabled on iOS.
+ *Everything below*

## NatCam Extended 1.5b3
+ NatCam Extended now features experimental support for Text Detection on Android.
+ Greatly improved metadata detection speed on Android.
+ Enhanced support for using libraries that depend on Google Play Services by avoiding clashes.
+ Added IMetadata interface with rect and timestamp fields.
+ Added MetadataCallback callback for use with IMetadata.
+ Added NatCam.DetectBarcode (MetadataCallback).
+ Added NatCam.DetectFace(MetadataCallback).
+ Added overloads for equality, Equals() and GetHashCode() operations to Face struct.
+ Added support for mirroring when using NatCam.SavePhoto iOS (Android still does not support this).
+ Fixed crash after switching cameras while detecting metadata on Android.
+ Fixed regression where metadata was not detected after the preview was resumed.
+ Fixed bug where metadata was not detected after switching cameras.
+ Fixed crash when using NatCam.SavePhoto on iOS 10+ due to missing NSPhotoLibraryUsageDescription.
+ Deprectaed Faces example.
+ Deprecated NatCamPreview component. Use NatCamU.Core.UI classes instead.
+ Deprecated NatCam.OnBarcodeDetect and NatCam.OnFaceDetect.
+ Deprectaed NatCam.RequestBarcode.
+ Deprecated BarcodeCallback and FaceCallback.
+ Deprecated BarcodeRequest struct.
+ Deprecated SaveOrientation struct. Use NatCamU.Core.Orientation instead.
+ Renamed Scanner example to QRCam.
+ *Everything below*

## NatCam Extended 1.5f2
+ Fixed barcode detection not working on NatCamLegacy (WebCamTexture).
+ *Everything below*

## NatCam Extended 1.5f1
+ Added NatCamPreview component for preview scaling, focusing, zooming and panning.
+ Added ScaleMode.FillView for NatCamPreviewScaler.
+ Added timeStamp integer field to Barcode and Face.
+ Added limited support for SavePhoto API on NatCam Legacy.
+ Added SaveMode enum.
+ Added SaveOrientation enum for saving photos with custom orientations.
+ Deprecated specifying MetadataDetection flag when initializing NatCam.
+ Deprecated NatCamPreviewScaler component.
+ Deprecated NatCamPreviewZoomer component.
+ Deprecated NatCamPreviewGestures component.
+ Deprecated NatCamPreviewBehaviour base class.
+ Deprecated ZoomMode.ZoomSpeedOverrideOnly.
+ Deprecated NatCam.RequestFace. Use NatCam.OnFaceDetect event instead.
+ Deprecated PhotoSaveMode enum.
+ Exposed AlbumName property for creating custom album names when using SaveMode.SaveToPhotoAlbum.
+ Fixed faces not being detected on Android.
+ Fixed faces not being detected properly by front camera on Android.
+ Fixed face positions not being in viewport coordinates on Android.
+ Fixed face positions being inverted on the Y axis.
+ Fixed Time.frameCount error when detecting barcodes in the editor.
+ Fixed non-existent WRITE_INTERNAL_STORAGE permission in Android Manifest.
+ Exposed google-play-services.jar to prevent build errors on duplicate copies.