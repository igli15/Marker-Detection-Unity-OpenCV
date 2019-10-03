## NatCam Professional 1.5f3
+ Added NatCam.PreviewMatrix(..) to greatly improve memory efficiency (so as not to allocate each time it is called).
+ Added NatCam.PreviewFrame(..) to greatly improve memory efficiency (so as not to allocate each time it is called).
+ Added VisionCam example to demonstrate using NatCam with OpenCVForUnity.
+ Added MotionCam example to demonstrate using NatCam.PreviewFrame.
+ Added ReadablePreview flag on NatCamAndroid. This is a workaround for the GPU driver bug that caused lag on the S7 Edge.
+ Added bitcode support on iOS.
+ Added NatCamToMatHelper script for OpenCV/DLibFaceLandmarkDetector.
+ Exposed bitrates for configuring video encoders.
+ Fixed crash when NatCam.StopRecording is called on Android.
+ Fixed crash when NatCam.StopRecording is called on iOS.
+ Fixed NatCam.PreviewMatrix having incorrect dimensions.
+ Fixed null reference exception when NatCam.PreviewBuffer is called on NatCamLegacy.
+ Fixed incorrect colors when using VisionCam example on iOS.
+ Deprecated NatCam.PreviewMatrix property.
+ Deprecated NatCam.PreviewFrame property.
+ Deprecated Utilities.SaveVideoToGallery. Use NatCam.Implementation.SaveVideo(string, SaveMode) instead.
+ Deprecated MotionCam example.
+ *Everything below*

## NatCam Professional 1.5b4
+ Added ReadablePreview flag on NatCamAndroid. This is a workaround for the GPU driver bug that caused lag on the S7 Edge.
+ Added bitcode support on iOS.
+ Added NatCamToMatHelper script for OpenCV/DLibFaceLandmarkDetector.
+ Fixed crash when NatCam.StopRecording is called on Android.
+ Fixed null reference exception when NatCam.PreviewBuffer is called on NatCamLegacy.
+ Deprecated Utilities.SaveVideoToGallery. Use NatCam.Implementation.SaveVideo(string, SaveMode) instead.
+ Fixed incorrect colors when using VisionCam example on iOS.
+ Deprecated MotionCam example.
+ *Everything below*

## NatCam Professional 1.5b3
+ Added NatCam.PreviewMatrix(..) to greatly improve memory efficiency (so as not to allocate each time it is called).
+ Added NatCam.PreviewFrame(..) to greatly improve memory efficiency (so as not to allocate each time it is called).
+ Added VisionCam example to demonstrate using NatCam with OpenCVForUnity.
+ Added MotionCam example to demonstrate using NatCam.PreviewFrame.
+ Exposed bitrates for configuring video encoders.
+ Deprecated NatCam.PreviewMatrix property.
+ Deprecated NatCam.PreviewFrame property.
+ Fixed crash when NatCam.StopRecording is called on iOS.
+ Fixed NatCam.PreviewMatrix having incorrect dimensions.
+ *Everything below*

## NatCam Professional 1.5f2
+ Added ReplayCam video recording example with recorded video playback.
+ Added flag to specify whether audio ppermission should be requested and if audio should be recorded.
+ Fixed crash when preview starts on Android devices running on OpenGLES 3.
+ Fixed microphone hardware requirement on Android.
+ *Everything below*

## NatCam Professional 1.5f1
+ Video recording is now available on platforms that support it.
+ Added NatCam.PreviewBuffer(...) that works on all platforms, even when using WebCamTexture.
+ Added Utilities.NatCamUtilities.SaveVideoToGallery to save recorded videos to device gallery.
+ On Android, the native preview update and preview data dimensions now respects the orientation of the app.
+ On Android, NatCam.PreviewFrame is no more skewed depending on app orientation.
+ Deprecated the concept of Readable preview. Preview data is now available on demand.
+ Deprecated OnNativePreviewUpdate event.
+ Deprecated ComponentBuffer enum.
+ Reimplemented NatCam.PreviewMatrix to be on demand like NatCam.PreviewFrame.
+ Renamed OPENCV_DEVELOPER_MODE macro to OPENCV_API
