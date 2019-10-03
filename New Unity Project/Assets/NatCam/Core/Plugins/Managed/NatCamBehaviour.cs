/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core {

	using UnityEngine;
	using UnityEngine.UI;
	using Utilities;
	using Util = Utilities.Utilities;
	
	[CoreDoc(70)]
	public class NatCamBehaviour : MonoBehaviour { //This class is easily subclassed and overriden
		
		[Header("Preview")]
		[CoreDoc(71)] public RawImage preview;
		
		[Header("Parameters")]
		[CoreDoc(72)] public Facing facing = Facing.Rear;
		[CoreDoc(73)] public ResolutionPreset previewResolution = ResolutionPreset.HD;
		[CoreDoc(74)] public ResolutionPreset photoResolution = ResolutionPreset.HighestResolution;
		[CoreDoc(75)] public FrameratePreset framerate = FrameratePreset.Default;
		
		[Header("Debugging")]
		[CoreDoc(76)] public Switch verbose;
		
		// Use this for initialization
		public virtual void Start () {
			// Set verbose mode
			NatCam.Verbose = verbose;
			// Set the active camera
			NatCam.Camera = facing == Facing.Front ? DeviceCamera.FrontCamera : DeviceCamera.RearCamera;
			// Null checking
			if (!NatCam.Camera) {
				// Log
				Util.LogError("Active camera is null. Consider changing the facing of the camera");
				return;
			}
			// Set the camera's preview resolution
			NatCam.Camera.SetPreviewResolution(previewResolution);
			// Set the camera's photo resolution
			NatCam.Camera.SetPhotoResolution(photoResolution);
			// Set the camera's framerate
			NatCam.Camera.SetFramerate(framerate);
			// Play
			NatCam.Play();
			// Register callback for when the preview starts //Note that this is a MUST when assigning the preview texture to anything
			NatCam.OnStart += OnStart;
			// Register for preview updates
			NatCam.OnFrame += OnFrame;
		}

		/// <summary>
		/// Method called when the camera preview starts
		/// </summary>
		[CoreDoc(77)]
		public virtual void OnStart () {
			// Set the preview RawImage texture once the preview starts
			if (preview != null) preview.texture = NatCam.Preview;
			// Log
			else Util.LogError("Preview RawImage has not been set");
			Util.LogVerbose("Preview started with dimensions: " + new Vector2(NatCam.Preview.width, NatCam.Preview.height));
		}

		/// <summary>
		/// Method called on every frame that the camera preview updates
		/// </summary>
		[CoreDoc(78), CoreCode(17)]
		public virtual void OnFrame () {}

		/// <summary>
		/// Switch the active camera of the preview
		/// </summary>
		/// <param name="newCamera">Optional. The camera to switch to. An int or a DeviceCamera can be passed here</param>
		[CoreDoc(79, 3), CoreCode(14), CoreCode(15), CoreCode(16)]
		public virtual void SwitchCamera (int newCamera = -1) {
			// Select the new camera ID // If no argument is given, switch to the next camera
			newCamera = newCamera < 0 ? (NatCam.Camera + 1) % DeviceCamera.Cameras.Count : newCamera;
			// Set the new active camera
			NatCam.Camera = (DeviceCamera)newCamera;
		}
	}
}