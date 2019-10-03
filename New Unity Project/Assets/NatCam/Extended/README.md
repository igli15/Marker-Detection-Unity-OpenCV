# NatCam Extended
The NatCam Extended spec builds upon the Core spec by adding crucial, time-saving functionality:
- Barcode detection
- Face tracking
- Face landmarks
- Text detection (experimental)
- SavePhoto API

## Barcode Detection
NatCam allows you to detect barcodes easily:
```csharp
void Start () {
    // Detect QR codes
    NatCam.DetectBarcode(OnDetectBarcode, BarcodeFormat.QR);
    // Play
    NatCam.Play(DeviceCamera.RearCamera);
}

void OnDetectBarcode (Barcode barcode) {
    // Log
    Debug.Log("Found barcode: "+barcode.ToString());
}
```
Note that you must call `NatCam.DetectBarcode(..)` **before** calling `NatCam.Play()`.

## Face Tracking
NatCam allows you to detect faces much like barcodes:
```csharp
void Start () {
    // Detect faces
    NatCam.DetectFace(OnDetectFace);
    // Play
    NatCam.Play(DeviceCamera.RearCamera);
}

void OnDetectFace (Face face) {
    // Log
    Debug.Log("Saw a face: "+face);
}
```
Like barcode detection, you must call `NatCam.DetectFace(..)` **before** calling `NatCam.Play()`.

## Face Landmarks
NatCam Extended now comes with a Face Landmark API. This is a highly platform-specific feature, so it is not 
integrated with the face detection pipeline, or more specifically the `Face` struct. The landmark API is contained 
within `NatCamMetadata`.
```csharp
void OnDetectFace (Face face) {
    // Create a list of face points
    List<Vector2> facePoints = new List<Vector2>();
    // Find the eyes and mouth
    if (NatCamMetadata.HasLandmark(face, Face.Landmark.LeftEye)) {
        var leftEye = NatCamMetadata.GetLandmark(face, Face.Landmark.LeftEye);
        facePoints.Add(leftEye);
    }
    if (NatCamMetadata.HasLandmark(face, Face.Landmark.RightEye)) {
        var rightEye = NatCamMetadata.GetLandmark(face, Face.Landmark.RightEye);
        facePoints.Add(rightEye);
    }
    if (NatCamMetadata.HasLandmark(face, Face.Landmark.Mouth)) {
        var mouth = NatCamMetadata.GetLandmark(face, Face.Landmark.Mouth);
        facePoints.Add(mouth);
    }
    // Draw the face points
    DrawPoints(facePoints);
}
```

## Text Detection (Experimental)
NatCam Extended now comes with experimental text detection. To use it, you must first uncomment this line 
at the top of NatCam.cs in NatCam Extended:
```csharp
#define EXPERIMENTAL_TEXT_DETECTION // Uncomment this to have access to the Text Detection API on Android
```
Now, you can detect text like so:
```csharp
void Start () {
    // Create a text request
    NatCam.DetectText(OnDetectText);
    // Start the camera
    base.Start();
}

void OnDetectText (Text text) {
    // Log
    Debug.Log("Found text: "+text);
}
```
Like barcode detection, you must call `NatCam.DetectText(..)` **before** calling `NatCam.Play()`.

## SavePhoto API
NatCam Extended allows you to save Texture2D's to the several device locations, including:
- App documents
- Photo gallery
- Album in photo gallery
Using the API is effortlessly easy:
```csharp
NatCam.SavePhoto(photo, SaveMode.SaveToAppDocuments | SaveMode.SaveToPhotoGallery);
```
You can also specify the orientation to use for saving the photo, and a callback which would receive the file path
when the texture has been saved:
```csharp
NatCam.SavePhoto(photo, SaveMode.SaveToAppAlbum, Orientation.Rotation_90, OnSavePhoto);

void OnSavePhoto (SaveMode mode, string path) {
    // Log
    Debug.Log("Photo saved with mode "+mode+" to path: "+path);
}
```
