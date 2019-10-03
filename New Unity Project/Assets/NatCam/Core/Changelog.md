## NatCam Core 1.5f3
+ NatCam now supports autorotation.
+ Made preview rendering faster on Android.
+ Made PhotoCallback return orientation information.
+ NatCam.OnStart is now called when cameras are changed and when the orientation is changed.
+ Added a unified orientation pipeline for captured photos.
+ Added NatCamPreview component for properly displaying textures on UI panels with scaling and orientation.
+ Added NatCamFocuser component for detecting focus gestures.
+ Added Utilities.RotateImage(...) for rotating an image in system memory.
+ Added bitcode support on iOS.
+ Added support for zooming on a large amount of iOS and Android devices.
+ Added DeviceCamera.MaxZoomRatio.
+ Added tap-to-focus functionality to MiniCam example using NatCamFocuser.
+ Added DeviceCamera.Framerate getter.
+ Added Orientation struct for photo orientation.
+ Added NatCamTransform2D shader for GPU-accelerated image transformation.
+ Added ScaleMode.Letterbox for letterbox scaling.
+ Added NatCam.Implementation.HasPermissions boolean.
+ Refactored NatCam.OnPreviewStart, INatCam.OnPreviewStart, and NatCamBehaviour.OnPreviewStart to OnStart.
+ Refactored NatCam.OnPreviewUpdate, INatCam.OnPreviewUpdate, and NatCamBehaviour.OnPreviewUpdate to OnFrame.
+ Fixed bug where screen flickered when running on Android without multithreaded rendering.
+ Fixed bug on Android where calling Play() after Release() or pausing and suspending the app does not work.
+ Fixed bug on Android where camera does not pause when Pause() is called.
+ Fixed crash when NatCam.Release() is called on Android.
+ Fixed FocusMode.MacroFocus not working on Android.
+ Fixed DeviceCamera.FocusMode getter causing an error.
+ Fixed bug where DeviceCamera.SetFocus(..) ignored focus mode completely.
+ Fixed bug where OnStart is not called when preview is resumed on Android.
+ Fixed bug where setting high framerates rarely worked on iOS.
+ Fixed bug where preview stretched when using NatCamScaler and autorotation.
+ Fixed resource leak when NatCam.Release() is called after NatCam.Pause() on NatCamLegacy.
+ Fixed memory leak when taking photos with the MiniCam example.
+ Fixed potential namespace conflicts.
+ Fixed resolution set for one camera being used by another on NatCamLegacy.
+ Fixed IL2CPP build support on Android.
+ Fixed crash on Android when NatCam is unable to open the camera.
+ Fixed rare crash on Android when app lost focus with multithreaded rendering disabled.
+ Fixed exception on Android when NatCam.Camera getter is called before NatCam.Camera has been set.
+ Fixed DeviceCamera.SetFocus focus point being inverted on the Y axis.
+ Fixed bug on Android where switching cameras caused preview to freeze on older devices.
+ Fixed bug on Android where DeviceCamera.TorchMode getter always returned Off.
+ Fixed bug where preview was incorrectly oriented after NatCam.Release() was called.
+ Fixed bug on Android where preview would flip briefly when camera was switched.
+ Fixed bug on NatCamLegacy where preview did not update after Pause() was called.
+ Fixed bug on Android where setting a camera that did not exist caused a hard crash.
+ Fixed bug on iOS where suspending the app caused system sounds to be distorted.
+ Fixed bug on iOS where pausing the preview caused the orientation to be incorrect.
+ Fixed 'Camera is being used after Camera.release() was called' exception on Android.
+ Fixed bug on Android where z-sorting was affected by the preview.
+ Fixed build error when building Android project with NatCam and Android Ultimate Plugin.
+ Deprecated INatCamMobile.HasPermissions.
+ Deprecated INatCamMobile interface.
+ Deprecated NatCamTransformation2D shader. Use NatCamTransform2D instead.
+ Deprecated DeviceCamera.IsZoomSupported. Instead, check that DeviceCamera.MaxZoomRatio > 1.
+ Renamed Minigram example to MiniCam.
+ Renamed StartingOff example to PlainCam.
+ Refactored INatCamDispatch to IDispatch.
+ Refactored NatCamUtilities to Utilities.
+ Refactored ScaleMode.FixedHeightVariableWidth to ScaleMode.AdjustWidth.
+ Refactored ScaleMode.FixedWidthVariableHeight to ScaleMode.AdjustHeight.
+ Made NatCamLegacy call OnPreviewStart only when Preview is not 16x16 (usually encountered on macOS).
+ IDispatch has been further modularized.
+ IDispatch will no more invoke a delegate more than once per Update.
+ Implemented IDisposable interface for IDispatch.
+ *Everything below*

## NatCam Core 1.5b4
+ NatCam.OnStart is now called when cameras are changed and when the orientation is changed.
+ Added a unified orientation pipeline for captured photos.
+ Added NatCamPreview component for properly displaying textures on UI panels with scaling and orientation.
+ Added NatCamFocuser component for detecting focus gestures.
+ Added ability to mirror image in Utilities.RotateImage.
+ Added bitcode support on iOS.
+ Added support for zooming on a large amount of iOS and Android devices.
+ Added DeviceCamera.MaxZoomRatio.
+ Added tap-to-focus functionality to MiniCam example using NatCamFocuser.
+ Improved NatCamFocuser by adding StartTracking(), StartTracking(FocusMode), and StopTracking() API.
+ Refactored NatCam.OnPreviewStart, INatCam.OnPreviewStart, and NatCamBehaviour.OnPreviewStart to OnStart.
+ Refactored NatCam.OnPreviewUpdate, INatCam.OnPreviewUpdate, and NatCamBehaviour.OnPreviewUpdate to OnFrame.
+ Fixed crash on Android when NatCam is unable to open the camera.
+ Fixed rare crash on Android when app lost focus with multithreaded rendering disabled.
+ Fixed exception on Android when NatCam.Camera getter is called before NatCam.Camera has been set.
+ Fixed DeviceCamera.SetFocus focus point being inverted on the Y axis.
+ Fixed bug on Android where switching cameras caused preview to freeze on older devices.
+ Fixed bug on Android where DeviceCamera.TorchMode getter always returned Off.
+ Fixed bug where preview was incorrectly oriented after NatCam.Release() was called.
+ Fixed bug on Android where preview would flip briefly when camera was switched.
+ Fixed bug on NatCamLegacy where preview did not update after Pause() was called.
+ Fixed bug on Android where setting a camera that did not exist caused a hard crash.
+ Fixed bug on iOS where suspending the app caused system sounds to be distorted.
+ Fixed bug on iOS where pausing the preview caused the orientation to be incorrect.
+ Fixed 'Camera is being used after Camera.release() was called' exception on Android.
+ Fixed bug on Android where z-sorting was affected by the preview.
+ Fixed build error when building Android project with NatCam and Android Ultimate Plugin.
+ Deprecated NatCamScaler component.
+ Deprecated NatCamView component.
+ Deprecated NatCamZoomer component.
+ Deprecated NatCam.HasPermissions. Use NatCam.Implementation.HasPermissions instead.
+ Deprecated DeviceCamera.IsZoomSupported. Instead, check that DeviceCamera.MaxZoomRatio > 1.
+ NatCamView2D shader has been refactored to NatCamTransform2D.
+ IDispatch has been further modularized.
+ IDispatch will no more invoke a delegate more than once per Update.
+ *Everything below*

## NatCam Core 1.5b3
+ NatCam now supports autorotation.
+ Made preview rendering faster on Android.
+ Made PhotoCallback return orientation information.
+ Added NatCamScaler component for scaling the preview to avoid stretching.
+ Added NatCamZoomer component for zooming the preview with input gestures.
+ Added NatCamFocuser component for focusing the camera at touch points.
+ Added DeviceCamera.Framerate getter.
+ Added Orientation struct for photo orientation.
+ Added NatCamView component for orienting captured photos using a GPU shader.
+ Added NatCamView2D shader for GPU-accelerated image transformation.
+ Added Utilities.RotateImage(...) for rotating an image in system memory.
+ Added ScaleMode.Letterbox for letterbox scaling.
+ Added NatCam.HasPermissions boolean.
+ Fixed bug where screen flickered when running on Android without multithreaded rendering.
+ Fixed bug on Android where calling Play() after Release() or pausing and suspending the app does not work.
+ Fixed bug on Android where camera does not pause when Pause() is called.
+ Fixed crash when NatCam.Release() is called on Android.
+ Fixed FocusMode.MacroFocus not working on Android.
+ Fixed DeviceCamera.FocusMode getter causing an error.
+ Fixed bug where DeviceCamera.SetFocus(..) ignored focus mode completely.
+ Fixed bug where OnPreviewStart is not called when preview is resumed on Android.
+ Fixed bug where setting high framerates rarely worked on iOS.
+ Fixed bug where preview stretched when using NatCamScaler and autorotation.
+ Fixed resource leak when NatCam.Release() is called after NatCam.Pause() on NatCamLegacy.
+ Fixed memory leak when taking photos with the MiniCam example.
+ Fixed potential namespace conflicts.
+ Fixed resolution set for one camera being used by another on NatCamLegacy.
+ Fixed IL2CPP build support on Android.
+ Deprecated INatCamMobile.HasPermissions.
+ Deprecated INatCamMobile interface.
+ Deprecated NatCamTransformation2D shader. Use NatCamView2D instead.
+ Renamed Minigram example to MiniCam.
+ Renamed StartingOff example to PlainCam.
+ Refactored INatCamDispatch to IDispatch.
+ Refactored NatCamUtilities to Utilities.
+ Refactored ScaleMode.FixedHeightVariableWidth to ScaleMode.AdjustWidth.
+ Refactored ScaleMode.FixedWidthVariableHeight to ScaleMode.AdjustHeight.
+ Made NatCamLegacy call OnPreviewStart only when Preview is not 16x16 (usually encountered on macOS).
+ Implemented IDisposable interface for IDispatch.
+ *Everything below*

## NatCam Core 1.5f2
+ Added master switches to enable and disable the Extended and Professional spec in NatCamLinker.
+ Added support for getting DeviceCamera.PhotoResolution on NatCamLegacy.
+ Added support for more Android devices by making camera requirement optional.
+ Fixed preview not showing on a large number of Android devices.
+ Fixed memory leak when calling NatCam.Play() when preview is paused on iOS.
+ Fixed crash on start when running on some Android devices running OpenGLES 2.
+ Fixed bug where camera did not switch on Android.
+ Fixed OnPreviewStart not being called when NatCam.Play() is called after NatCam.Pause().
+ Fixed DeviceCamera.PreviewResolution returning wrong values on NatCamLegacy.
+ Fixed iOS 10 crash by adding NSMicrophoneUsageDescription.
+ Fixed endless refresh when using plugins that used custom platform macros like Cross Platform Native Plugins.
+ Fixed 'requested build target group (15) doesn't exist' error in NatCamLinker.
+ *Everything below*

## NatCam Core 1.5f1
+ Added three API specifications: Core, Extended, and Professional for developers to choose from.
+ Completely rebuilt API to prepare for supporting more platforms.
+ Greatly improved speed of NatCam and Unity on Android.
+ Greatly improved speed of capturing photos especially on Android.
+ Made API easier to use by removing extraneous functions.
+ Added NatCamBehaviour component for quickly using NatCam.
+ Added FocusMode.SoftFocus for soft autofocus especially in VR applications on Android.
+ Added FocusMode.MacroFocus for focusing on up-close objects like barcodes on Android.
+ Added NatCamView2D shader for GPU-accelerated image transformation.
+ Added NatCam.CapturePhoto(PhotoCallback).
+ Added NATCAM_CORE, NATCAM_EXTENDED, NATCAM_PROFESSIONAL macros for specification-dependent compilation.
+ Added NATCAM_[version number] macro for version-dependent compilation. In 1.5, it is NATCAM_15.
+ Added support for more tablets on the Play store by making flash and focus capabilities optional.
+ Added a blacklist to keep track of devices that don't work.
+ Added support for more Android devices by making camera requirement optional.
+ Deprecated NatCam.Initialize(...). NatCam will initialize itself however it needs to.
+ Deprecated UnitygramBase component for NatCamBehaviour component.
+ Deprecated NatCam.ExecuteOnPreviewStart(PreviewCallback). Use NatCam.OnPreviewStart instead.
+ Deprecated NatCam.CapturePhoto() and NatCam.OnPhotoCapture event.
+ Dropped captured photo correction for app orientation. Now all captured photos are in landscape left orientation.
+ Fixed Android N compatibility.
+ Fixed iOS 10 compatibility by adding NSCameraUsageDescription.
+ Fixed rare crash immediately app is suspended on Android.
+ Fixed rare crash when calling NatCam.Release() on Android.
+ Fixed EXC_BAD_ACCESS crash when switching cameras on iOS.
+ Fixed preview incorrectly rotating when app is using fixed orientation on iOS.
+ Fixed build error on Android because of targetSDKVersion.
+ Fixed OnPreviewStart not being invoked when camera was played after being paused.
+ Fixed OnPreviewStart being invoked too soon after camera was played after being paused.
+ Fixed OnPreviewUpdate being called too frequently on NatCam Legacy (WebCamTexture).
+ Fixed preview flipping momentarily when switching cameras on Android.
+ Fixed bug on Android where photo resolution could only be set before NatCam.Play() is called.
+ Fixed bug on Android where some devices fail to resume the preview after the app is suspended.
+ Fixed bug on Android where calling CapturePhoto() on cameras that did not support flash mode failed.
+ Fixed bug on iOS where setting exposure mode to Auto did not work properly.
+ Fixed bug on iOS where DeviceCamera.VerticalFOV returned horizontal FOV.
+ Fixed bug on iOS where preview orientation might be incorrect when device was face up or down.
+ Fixed rare issue where preview might freeze after some seconds.
+ Fixed camera preview stopping when camera property was set on a camera that is not active.
+ Fixed stackTraceLogType warning on Unity 5.4 and above.
+ Rebuilt documentation.
+ Refactored NatCam.ActiveCamera to NatCam.Camera.
+ Reimplemented control pipeline on Android to be cleaner and more efficient.
+ Removed FocusMode.HybridFocus. Use bitwise OR instead.
+ Reduced the amount of text that NatCam logs.
+ Changed Android API minimum requirement to API level 15.
+ Changed ResolutionPreset.HighestResolution to default to FullHD when calling DeviceCamera.SetPreviewResolution(ResolutionPreset).

## NatCam 1.4f1:
*NOT RELEASED*

## NatCam 1.3:
+ Added universal barcode detection support. Now, barcodes can be detected in the editor and all other platforms.
+ Added Exposure control with DeviceCamera.ExposureBias, DeviceCamera.MinExposureBias and DeviceCamera.MaxExposureBias.
+ Added ExposureMode enum and DeviceCamera.ExposureMode.
+ Added Face detection with Face struct and NatCam.RequestFace().
+ Added DeviceCamera.SetPhotoResolution() + overloads.
+ Added DeviceCamera.ActivePhotoResolution.
+ Added a new, low-cost rendering pipeline on Android.
+ Removed rendering pipeline on iOS. This has increased performance especially for GPU-bound applications.
+ Reduced rendering pipeline memory usage on Android.
+ Removed NATCAM_DEVELOPER_MODE and component buffer access (Y and UV buffers) from rendering pipeline.
+ Fixed hanging and crashing on Samsung Galaxy S4, Nexus 1, and other Android devices with PowerVR SGX540 family GPU's.
+ Barcode detection now supports Unicode characters.
+ Made camera switching faster on Android.
+ Reimplemented NatCamPreviewScaler to be more stable.
+ Added NatCam.SaveToPhotos for saving Texture2D to the gallery or app album.
+ Added necessary checks and error logging for google_play_services when detecting barcodes and faces on Android.
+ Fixed bug on Android where camera must be manually focused before autofocus starts.
+ Fixed bug where NatCam becomes unresponsive when there is no camera in the scene.
+ Removed Verbose switch from NatCam.Initialize(), it is now a member variable (NatCam.Verbose = ...).
+ Deprecated BarcodeDetection switch in NatCam.Initialize() for MetadataDetection (which now includes faces).
+ Changed OnDetectedBarcode delegate template to take single barcode instead of list.
+ Fixed barcodes not being detected when the preview is resumed after calling Pause on Android.
+ Fixed FormatException when scanning some barcodes.
+ Fixed bug where NatCam.ExecuteOnPreviewStart invokes immediately after switching cameras.
+ Fixed bug where NatCam.PreviewMatrix is null when the OnPreviewStart event is broadcast.
+ Fixed bug where NatCamPreviewScaler will incorrectly stretch UI panel on iOS.
+ Added NatCam.IsInitialized property.
+ Renamed Unitygram example to Minigram.
+ Everything below.

## NatCan 1.2:
+ Completely rebuilt API to have a more object-oriented programming pattern, and to be much cleaner.
+ Immediate native-to-managed callbacks, deprecated UnitySendMessage for MonoPInvokeCallback with function pointers.
+ Added NativePreviewCallback and NatCamNativeInterface.EnableComponentUpdate() to get access to the raw luma (Y) and chroma (UV) buffers from the camera.
+ Added the NatCam rendering pipeline to NatCam iOS.
+ Added NatCamNativeInterface.DisableRenderingPipeline() to disable NatCam's native rendering pipeline.
+ Added NatCamPreviewGestures component for easy detection of focusing and zooming gestures.
+ Added UnitygramBase component for quickly implementing NatCam.
+ Removed NatCam.AutoTapToFocus, NatCam.AutoPinchToZoom.
+ Camera preview is now accessible through NatCam.Preview and returns a Texture (not Texture2D).
+ Camera switching on iOS and Android is now stable and responsive.
+ Added SetFramerate() in the DeviceCamera class and FrameratePreset enum.
+ Added HorizontalFOV, VerticalFOV, IsTorchSupported, IsFlashSupported, and IsZoomSupported in the DeviceCamera class.
+ Added DeviceCamera.Cameras list of cameras on the device, not just DeviceCamera.Front/RearCamera.
+ Added ability to specify NatCam interface (native or fallback) and NatCamInterface enum.
+ Added verboseMode switch to NatCam.Initialize() for debugging.
+ Added NatCam.RequestBarcodeDetection(), BarcodeRequest struct.
+ Added ability to use bitwise operators to request multiple barcode formats when creating barcode detection requests.
+ Added NatCam.HasPermissions to check if the app has camera permissions.
+ Deprecated DeviceCamera.SupportedResolutions and Resolution struct. Use ResolutionPreset instead.
+ Deprecated CapturePhoto overloads for NatCam.CapturePhoto(params PhotoCaptureCallback[] callbacks).
+ Removed '#if CALLIGRAPHY_DOC_GEN_MODE' conditional directives making code much cleaner.
+ Fix camera preview lagging on iOS.
+ OpenCV PreviewMatrix now updates from the native pixel buffer. This gives some memory savings and performance increase.
+ Captured photo is now the highest resolution that the camera supports by default.
+ Captured photo is now RGBA32 format. This means you can use Get/SetPixels(s), EncodeToJPG/PNG, and Apply.
+ Preview is now RGBA32 format on both iOS and Android.
+ Fixed "Error Creating Optimization Context" when using Readable preview on Galaxy S6 and Galaxy Tab.
+ Fixed rare scan line jitter when NatCam corrects padding with Readable preview on some Android devices.
+ Added ALLOCATE_NEW_PHOTO_TEXTURES macro for optimizing memory usage.
+ Added ALLOCATE_NEW_FRAME_TEXTURES macro for optimizing memory usage.
+ Fixed Android crash on Stop().
+ Removed NatCam detectTouchesForFocus and detectPinchToZoom for NatCamPreviewGestures component.
+ Fixed error when LoadLevel is called--something along the lines of "SendMessage: NatCamHelper not found".
+ Added editor-serialized variables for Unitygram example. Now, you can set Unitygram's variables from the editor instead of code.
+ Unitygram example is now a camera app featuring photo capture with flash, switching cameras, and barcode detection.
+ Automatically link required frameworks on iOS.
+ Deprecated OPTIMIZATION_USE_NATIVE_BUFFER macro, and as a result, direct support for Unity 5.2 has been stopped.
+ NatCamPreviewUIPanelScaler and NatCamPreviewUIPanelZoomer have been renamed to NatCamPreviewScaler and NatCamPreviewZoomer respectively.
+ Deprecated NatCam.Stop() for NatCam.Release().
+ Fixed NatCam.Release() (formerly 'Stop') being ignored when NatCam.Pause() is called immediately before.
+ Fixed NatCamPreviewScaler not correctly scaling HD and FullHD preview on iOS.
+ Fixed rotation and skewing when using CapturedPhoto with OpenCV.
+ Fixed memory leak when calling Release() (formerly named 'Stop') and Initialize() several times on iOS.
+ Fixed occasional tearing when running very high resolution previews on iOS and Android.
+ Completely rebuilt the documentation.
+ Added Easter Eggs on iOS.
+ Everything below.

## NatCam 1.1:
+ FocusMode enum. Now you can specify the active camera's focus mode from one of the four FocusModes: AutoFocus, TapToFocus, HybridFocus, and Off.
+ Orientation support. NatCam now supports all orientations. The preview will always be correctly rotated on all orientations.
+ Native Sources. iOS sources and Android Java sources are now included. Instructions on how to compile both are also included. Android C++ sources will be coming in the next release.
+ Native Plugin Callback for access to the camera preview data in native C-based code (C, C++, Objective-C, and C#). A code example for how to do this is included in the documentation (under NatCam>NatCamNativePluginUpdateEvent).
+ Torch while preview is running.
+ Everything below.

## NatCam 1.0:
+ Fluid camera preview on iOS and Android.
+ OpenCV and AR support on iOS and Android.
+ Autofocus.
+ Tap-to-Focus.
+ Zoom (Camera zoom on devices that support it, but a shader-accelerated digital zoom option is available on all devices).
+ Pinch-to-Zoom.
+ Machine Readable Code Detection (e.g QR Codes, ISBN Barcodes, and so on).
+ Capture Photos.
+ Saving captured photos to user devices (and an app album).
+ Flash.
+ Torch.
+ Front Camera Support.
+ Switching Cameras.
+ Getting Supported Camera Resolutions.
+ Android IL2CPP support (and iOS IL2CPP).
+ Low memory footprint.
+ Low CPU utilization.