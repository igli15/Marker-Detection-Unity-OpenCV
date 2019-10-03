# NatCam Core
NatCam Core provides a much cleaner, more functional, and amazingly performant API for accessing and controlling 
device cameras on devices. NatCam is built on the INatCam and INatCamDevice interfaces. The API comes with platform-
specific implementation of the interfaces mentioned. The front-end API acts as an abstraction layer for each 
native INatCam implementation.

Using NatCam is as simple as calling:
```csharp
NatCam.Play(DeviceCamera.RearCamera);
```
The preview is started and the preview texture becomes available in the `NatCam.OnPreviewStart` event. This is why 
this event is usually used to display the preview texture on a surface:
```csharp
NatCam.OnPreviewStart += () => material.mainTexture = NatCam.Preview;
```

NatCam features a full camera control pipeline for utilizing camera functionality such as focusing, zooming, exposure, 
and so on. To use this functionality, simply access the properties in the DeviceCamera class:
```csharp
DeviceCamera.RearCamera.ExposureBias = 1.3;
```

NatCam also allows for high-resolution photo capture from the camera. To do so, simply call the `CapturePhoto` function 
with an appropriate callback and an orientation container (since the photo is not corrected for app orientation):
```csharp
NatCam.CapturePhoto(OnPhotoCapture);

void OnPhotoCapture (Texture2D photo, Orientation orientation) {
    // Do stuff...
    Texture2D.Destroy(photo); // Remember to release the texture so as to avoid memory leak
}
```

*INCOMPLETE* 
- Discuss NatCamView, NatCamU.Core.UI, Utilities.RotateImage

___

With the simplicity of NatCam Core, you have the power and speed to create interactive, responsive camera apps. Happy coding!

## Requirements
- On iOS, NatCam Core requires iOS 7 and up (it requires iOS 8 if you use `DeviceCamera.ExposureBias`).
- On Android, NatCam Core requires API Level 15 and up.

## Notes
- Documentation has not been updated since 1.5f1.
- On Android, Unity automatically requests camera permissions on app start. This cannot be changed without modifying Unity Android natively.
- On iOS, camera permissions are requested the first time the camera is opened.

## Quick Tips
- Please peruse the included scripting reference under NatCam>Scripting Reference in the Editor. You can also find the docs [here](http://docs.natcam.io).
- To discuss or report an issue, visit Unity forums [here](http://forum.unity3d.com/threads/natcam-device-camera-api.374690/).
- Check out more NatCam examples on Github [here](https://github.com/olokobayusuf?tab=repositories).
- Contact me at [olokobayusuf@gmail.com](mailto:olokobayusuf@gmail.com).
- See video tutorials [here](https://www.youtube.com/watch?v=6thfRz9vkyM&list=PL993yBWYjPgCiIkUlM3DJhOdcXNm9IVXh).

Thank you very much!