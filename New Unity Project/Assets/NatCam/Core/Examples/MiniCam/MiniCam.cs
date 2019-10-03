/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using Core;
    using Core.UI;

    public class MiniCam : NatCamBehaviour {
        
        [Header("UI")]
        public NatCamPreview panel;
        public NatCamFocuser focuser;
        public Text flashText;
        public Button switchCamButton, flashButton;
        public Image checkIco, flashIco;
        private Texture2D photo;


        #region --Unity Messages--

        // Use this for initialization
        public override void Start () {
            // Start base
            base.Start();
            // Set the flash icon
            SetFlashIcon();
        }
        #endregion

        
        #region --NatCam and UI Callbacks--

        public override void OnStart () {
            // Display the preview
            panel.Apply(NatCam.Preview);
            // Start tracking focus gestures
            focuser.StartTracking();
        }
        
        public void CapturePhoto () {
            // Divert control if we are checking the captured photo
            if (!checkIco.gameObject.activeInHierarchy) NatCam.CapturePhoto(OnPhoto);
            // Check captured photo
            else OnViewPhoto();
        }
        
        void OnPhoto (Texture2D photo, Orientation orientation) {
            // Cache the photo
            this.photo = photo;
            // Display the photo
            panel.Apply(photo, orientation);
            // Enable the check icon
            checkIco.gameObject.SetActive(true);
            // Disable the switch camera button
            switchCamButton.gameObject.SetActive(false);
            // Disable the flash button
            flashButton.gameObject.SetActive(false);
        }
        #endregion
        
        
        #region --UI Ops--
        
        public void SwitchCamera () {
            //Switch camera
            base.SwitchCamera();
            //Set the flash icon
            SetFlashIcon();
        }
        
        public void ToggleFlashMode () {
            //Set the active camera's flash mode
            NatCam.Camera.FlashMode = NatCam.Camera.IsFlashSupported ? NatCam.Camera.FlashMode == FlashMode.Auto ? FlashMode.On : NatCam.Camera.FlashMode == FlashMode.On ? FlashMode.Off : FlashMode.Auto : NatCam.Camera.FlashMode;
            //Set the flash icon
            SetFlashIcon();
        }

        public void ToggleTorchMode () {
            //Set the active camera's torch mode
            NatCam.Camera.TorchMode = NatCam.Camera.TorchMode == Switch.Off ? Switch.On : Switch.Off;
        }
        
        void OnViewPhoto () {
            // Disable the check icon
            checkIco.gameObject.SetActive(false);
            // Display the preview
            panel.Apply(NatCam.Preview);
            // Enable the switch camera button
            switchCamButton.gameObject.SetActive(true);
            // Enable the flash button
            flashButton.gameObject.SetActive(true);
            // Free the photo texture
            Texture2D.Destroy(photo); photo = null;
        }
        
        void SetFlashIcon () {
            //Null checking
            if (!NatCam.Camera) return;
            //Set the icon
            flashIco.color = !NatCam.Camera.IsFlashSupported || NatCam.Camera.FlashMode == FlashMode.Off ? (Color)new Color32(120, 120, 120, 255) : Color.white;
            //Set the auto text for flash
            flashText.text = NatCam.Camera.IsFlashSupported && NatCam.Camera.FlashMode == FlashMode.Auto ? "A" : "";
        }
        #endregion
    }
}