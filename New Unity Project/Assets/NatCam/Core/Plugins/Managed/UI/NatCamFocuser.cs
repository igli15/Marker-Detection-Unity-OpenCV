/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.UI {

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(EventTrigger), typeof(Graphic))]
    public sealed class NatCamFocuser : MonoBehaviour, IPointerUpHandler { // NCDOC // Excuse the typo ;)

        public FocusMode focusMode = FocusMode.TapToFocus;
        public bool IsTracking {get; private set;}


        #region --Client API--

        /// <summary>
        /// Start tracking focus gestures on the UI panel that this is attached to
        /// </summary>
        public void StartTracking () {
            StartTracking(focusMode);
        }

        /// <summary>
        /// Start tracking focus gestures on the UI panel that this is attached to
        /// </summary>
        /// <param name="focusMode">Focus mode to apply to the camera. Note that this must have 
        /// the FocusMode.TapToFocus bit set for tap to focus to work</param>
        public void StartTracking (FocusMode focusMode) {
            if (!NatCam.Camera) return;
            this.focusMode = (NatCam.Camera.FocusMode = focusMode);
            this.IsTracking = true;
        }

        /// <summary>
        /// Stop tracking focus gestures
        /// </summary>
        public void StopTracking () {
            IsTracking = false;
        }
        #endregion


        #region --UI Callbacks--

        void IPointerUpHandler.OnPointerUp (PointerEventData eventData) {
            if (IsTracking && NatCam.Camera) NatCam.Camera.SetFocus(Camera.main.ScreenToViewportPoint(eventData.pressPosition));
        }
        #endregion
    }
}