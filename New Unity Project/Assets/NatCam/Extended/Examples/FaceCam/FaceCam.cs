/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Examples {

	using UnityEngine;
	using UnityEngine.UI;
	using System;
	using Core;
	using Core.UI;
	using Extended;
	using Util = Extended.Utilities.Utilities;
	using Faces = System.Collections.Generic.List<Extended.Face>;
	using Rectangles = System.Collections.Generic.Dictionary<Extended.Face, UnityEngine.UI.Graphic>;
	
	/*
	*	FaceCam Example
	*	Example showcasing NatCam face detection pipeline
	*/
	public class FaceCam : NatCamBehaviour {

		#if NATCAM_EXTENDED

		[Header("Faces")]
		[Tooltip(PrefabTip)] public Graphic rectPrefab;
		
		[Header("Preview")]
		public NatCamPreview panel;
		private Faces faces = new Faces();
		private Rectangles rectangles = new Rectangles();
		private const long FaceExpirationEps = 300L; // After 300ms of not being detected, consider face expired

		public override void Start () {
			// Subscribe for face detection // NOTE: Since 1.5, you must request metadata before calling Play()
			NatCam.DetectFace(OnFace);
			// base
			base.Start();
		}

		public override void OnStart () {
			// Display and scale the preview
			panel.Apply(NatCam.Preview);
		}

		private void OnFace (Face face) {
			// Check if we are tracking this face
			if (faces.Contains(face)) faces.Remove(face);
			// Add or update the face
			faces.Add(face);
			// Add a rectangle
			if (!rectangles.ContainsKey(face)) rectangles.Add(face, CreateRectangle());
		}

		private void Update () {
			for (int i = faces.Count - 1; i >= 0; i--) {
				// Check if the face has expired
				if (Util.CurrentTime - faces[i].timestamp > FaceExpirationEps) {
					// Remove it
					RemoveFace(i);
					continue;
				}
				// Draw the rectangle
				DrawMetadataRect(faces[i], rectangles[faces[i]]);
			}
		}


		#region --Utility--

		private Graphic CreateRectangle () {
			// Instantiate the graphic
			var graphic = Instantiate(rectPrefab);
			// Enable it
			graphic.enabled = true;
			// Set its parent
			graphic.transform.SetParent(rectPrefab.canvas.transform, false);
			// Return
			return graphic;
		}

		private void RemoveFace (int i) {
			// Destroy the face's rectangle
			Destroy(rectangles[faces[i]]);
			// Remove the rectangle
			rectangles.Remove(faces[i]);
			// Remove the face
			faces.RemoveAt(i);
		}
		
		/// <summary>
		/// Draw a metadata rect with the supplied UI graphic by positioning and scaling the graphic
		/// Don't be shy to use this in your own applications (barcodes, and so on)
		/// </summary>
		/// <param name="metadata">Metadata to draw</param>
		/// <param name="graphic">Graphic to be positioned and scaled</param>
		public static void DrawMetadataRect (IMetadata metadata, Graphic graphic) {
			// Null checking
			if (!graphic) return;
			// Define the preview to canvas space transformation
			Func<Vector2, Vector2> PreviewToCanvas = preview => Vector2.Scale(
				graphic.canvas.pixelRect.size,
				// Calculate viewport-relative size
				new Vector2(
					preview.x / NatCam.Preview.width,
					preview.y / NatCam.Preview.height
				)
			) / graphic.canvas.scaleFactor;
			// Position
			graphic.rectTransform.anchoredPosition = PreviewToCanvas(metadata.rect.position);
			// Scale
			graphic.rectTransform.sizeDelta = PreviewToCanvas(metadata.rect.size);
		}
		#endregion

		private const string PrefabTip = @"For this to work properly, ensure that the graphic is parented to the scene's 
		root canvas and that the graphic is anchored to the bottom-left corner of the canvas";
		#endif
	}
}