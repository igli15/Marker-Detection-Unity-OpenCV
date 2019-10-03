/* 
*   NatCam Professional
*   Copyright (c) 2016 Yusuf Olokoba
*/

// Compatible with NatCam 1.5f3+
// OPENCV_API must be uncommented below and in NatCam.cs

//#define OPENCV_API

namespace NatCamU.Professional.Utilities {

    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using Core;
    #if OPENCV_API
    using OpenCVForUnity;
    #endif

    public class NatCamToMatHelper : NatCamBehaviour, IDisposable {

        [Header("Events")]
        public UnityEvent OnInitedEvent;
        public UnityEvent OnDisposedEvent;


        #region --NatCam Callbacks--

        public override void OnStart () {
            // Invoke initialization event
			if (OnInitedEvent != null) OnInitedEvent.Invoke ();
        }
        #endregion


        #region --Client API--

        public void Init () {
            // If playing, ignore
            if (NatCam.IsPlaying) return;
            // Create initialization event if null
            OnInitedEvent = OnInitedEvent ?? new UnityEvent ();
            // Create disposed event if null
            OnDisposedEvent = OnDisposedEvent ?? new UnityEvent ();
            // Start NatCam
            base.Start();
        }

        /// <summary>
        /// Init the specified camera, requestWidth, requestHeight.
        /// </summary>
        /// <param name="facing">Facing of the camera.</param>
        public void Init (Facing facing) {
            //Set the desired facing
            this.facing = facing;
            //Initialize
            Init();
        }

        /// <summary>
        /// Has this instance been initialized?
        /// </summary>
        /// <returns><c>true</c>, if inited was ised, <c>false</c> otherwise.</returns>
        public bool isInited () {
            return true;
        }

        /// <summary>
        /// Play this instance.
        /// </summary>
        public void Play () {
            if (!NatCam.IsPlaying) NatCam.Play();
        }

        /// <summary>
        /// Pause this instance.
        /// </summary>
        public void Pause () {
            if (NatCam.IsPlaying) NatCam.Pause();
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop () {
            Pause();
        }

        /// <summary>
        /// Is the camera playing?
        /// </summary>
        /// <returns><c>true</c>, if playing was used, <c>false</c> otherwise.</returns>
        public bool isPlaying () {
            return NatCam.IsPlaying;
        }

        /// <summary>
        /// Gets the device camera.
        /// </summary>
        /// <returns>The device camera.</returns>
        public DeviceCamera GetDeviceCamera () {
            return NatCam.Camera;
        }

        /// <summary>
        /// Dids the update this frame.
        /// </summary>
        /// <returns><c>true</c>, if update this frame was dided, <c>false</c> otherwise.</returns>
        public bool didUpdateThisFrame () {
            return true; // NatCam isn't synchronized with Unity's frame updates, so just say yes
        }
        #endregion


        #region --OpenCV--

        #if OPENCV_API

        private Mat matrix;

        /// <summary>
        /// Gets the matrix.
        /// </summary>
        /// <returns>The matrix.</returns>
        public Mat GetMat () {
            NatCam.PreviewMatrix(ref matrix);
            return matrix;
        }
        #endif

        /// <summary>
        /// Releases all resource used by the <see cref="NatCamToMatHelper"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="NatCamToMatHelper"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="NatCamToMatHelper"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="NatCamToMatHelper"/> so
        /// the garbage collector can reclaim the memory that the <see cref="NatCamToMatHelper"/> was occupying.</remarks>
        public void Dispose ()  {
            // Release NatCam
            NatCam.Release();
            #if OPENCV_API
            // Dispose the matrix if it isn't null
            if (matrix != null) matrix.Dispose (); matrix = null;
            #endif
            // Invoke the OnDisposedEvent
            if (OnDisposedEvent != null) OnDisposedEvent.Invoke ();
        }
        #endregion
    }
}