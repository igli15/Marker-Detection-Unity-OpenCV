/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Examples {

	using UnityEngine;
	using System.Collections;
	using Core;
	using Core.UI;
	using Extended;

	public class QRCam : NatCamBehaviour {

		#if NATCAM_EXTENDED
		
		[Header("QR Cam")]
		public FocusMode focusMode = FocusMode.MacroFocus;
		public BarcodeFormat format = BarcodeFormat.QR;
		public UnityEngine.UI.Text display;

		[Header("Preview")]
		public NatCamPreview panel;

		public override void Start () {
			// Request barcodes // NOTE: Since 1.5, you must request metadata before calling Play()
			NatCam.DetectBarcode(OnBarcode, format, false);
			// Base
			base.Start();
		}

		public override void OnStart () {
			// Set focus mode
			NatCam.Camera.FocusMode = focusMode;
			// Scale the preview panel
			panel.Apply(NatCam.Preview);
		}

		public void OnBarcode (Barcode barcode) {
			// Display the text then remove
			StartCoroutine(ShowBarcode(barcode));
		}

		private IEnumerator ShowBarcode (Barcode barcode) {
			// Show the barcode
			display.text = barcode.data;
			// Wait
			yield return new WaitForSeconds(3f);
			// Reset
			display.text = string.Empty;
		}
		#endif
	}
}