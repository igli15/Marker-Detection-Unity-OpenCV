/* 
*   NatCam Professional
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Examples {

	using UnityEngine;
	using System;
	using System.Runtime.InteropServices;
	using Core;

	/*
	* GreyCam Example
	* Example showcasing NatCam Preview Data Pipeline
	* Make sure to run this on the lowest camera resolution as it is heavily computationally expensive
	*/
	public class GreyCam : NatCamBehaviour {

		private Texture2D texture;
		private byte[] pixels;
		const TextureFormat format =
		#if UNITY_IOS && !UNITY_EDITOR
		TextureFormat.BGRA32;
		#else
		TextureFormat.RGBA32;
		#endif
		
		// Override OnStart so that we can set our own
		// texture as the preview texture
		public override void OnStart () {}
		
		#if NATCAM_PROFESSIONAL

		public override void OnFrame () {
			// Declare buffer properties
			IntPtr handle; int width, height, size;
			// Read the preview buffer
			if (!NatCam.PreviewBuffer(out handle, out width, out height, out size)) return;
			// Convert to greyscale
			ConvertToGrey(ref pixels, handle, size);
			// Create the texture
			texture = texture ?? new Texture2D(width, height, format, false, false);
			// Size checking // Ideally, call Texture2D.Destroy and realloc
			if (texture.width != width || texture.height != height) texture.Resize(width, height);
			// Load texture data
			texture.LoadRawTextureData(pixels);
			// Upload to GPU
			texture.Apply();
			// Set RawImage texture
			preview.texture = texture;
		}
		#endif

		/// <summary>
		/// Convert a four-channel pixel buffer to greyscale
		/// </summary>
		/// <param name="buffer">The destination buffer containing the greyscale image</param>
		/// <param name="pixelBuffer">The source buffer. This buffer MUST be four bytes per pixel</param>
		/// <param name="size">The size of the pixel buffer</param>
		public static void ConvertToGrey (ref byte[] buffer, IntPtr pixelBuffer, int size) {
			// Create the managed buffer
			buffer = buffer ?? new byte[size];
			// Iterate over the native buffer
			for (int i = 0; i < size; i += 4) {
				// Calculate channels
				byte
				r = Marshal.ReadByte(pixelBuffer, i + 0), g = Marshal.ReadByte(pixelBuffer, i + 1),
				b = Marshal.ReadByte(pixelBuffer, i + 2), a = Marshal.ReadByte(pixelBuffer, i + 3),
				// Use quick approximation to save time and memory
				l = (byte)((r + r + r + b + g + g + g + g) >> 3);
				// Set pixels in the buffer
				buffer[i] = buffer[i + 1] = buffer[i + 2] = l; buffer[i + 3] = a;
			}
		}
	}
}