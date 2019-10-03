/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;
using System;

namespace NatCamU.Dispatch {

    using Core;

    #if UNITY_5_4_OR_NEWER
    using UnityEngine.SceneManagement;
    #endif
    
    [AddComponentMenu("")]
    public sealed class DispatchUtility : MonoBehaviour {

        #region --Data--
        public static event Action onUpdate, onFixedUpdate, onPreRender, onPostRender, onQuit;
        public static event Action<Orientation> onOrient;
        public static event Action<bool> onPause;
        public static Orientation Orientation {get {return GetOrientation(instance.orientation);}}
        #endregion


        #region --State--
        private Camera cam;
        private DeviceOrientation orientation = 0;
        private static DispatchUtility instance;

        static DispatchUtility () {
            instance = new GameObject("NatCam").AddComponent<DispatchUtility>();
            #if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded += instance.LevelWasLoaded;
            #endif
            Camera.onPreRender += instance.PreRender;
            Camera.onPostRender += instance.PostRender;
            instance.CheckOrientation();
        }
        #endregion


        #region --Unity Messages--

        void Awake () {
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(this);
            CheckOrientation();
        }

        void Update () {
            if (onUpdate != null) onUpdate();
            CheckOrientation();
        }

        void FixedUpdate () {
            if (onFixedUpdate != null) onFixedUpdate();
        }

        void PreRender (Camera camera) {
            if (IsCamera(camera) && onPreRender != null) onPreRender();
        }

        void PostRender (Camera camera) {
            if (IsCamera(camera) && onPostRender != null) onPostRender();
        }

        #if UNITY_5_4_OR_NEWER
        void LevelWasLoaded (Scene scene, LoadSceneMode mode)
        #else
        void OnLevelWasLoaded (int level)
        #endif
        {
            cam = null;
        }
        
        void OnApplicationPause (bool paused) {
            if (onPause != null) onPause(paused);
        }
        
        void OnApplicationQuit () {
            if (onQuit != null) onQuit();
        }
        #endregion


        #region --Utility--

        bool IsCamera (Camera camera) {
            cam = cam ?? camera;
            if (cam != camera) return false;
            else return true;
        }

        void CheckOrientation () {
            DeviceOrientation reference = (DeviceOrientation)(int)Screen.orientation; // Input.deviceOrientation
            switch (reference) {
                case DeviceOrientation.FaceDown: case DeviceOrientation.FaceUp: case DeviceOrientation.Unknown: break;
                default: if (orientation != reference) {
                    if (onOrient != null) onOrient(GetOrientation(reference));
                    orientation = reference;
                }
                break;
            }
        }

        private static Orientation GetOrientation (DeviceOrientation orientation) {
            if (!Application.isMobilePlatform) return Orientation.Rotation_0;
            switch (orientation) {
                case DeviceOrientation.LandscapeLeft: return Orientation.Rotation_0;
                case DeviceOrientation.Portrait: return Orientation.Rotation_90;
                case DeviceOrientation.LandscapeRight: return Orientation.Rotation_180;
                default: return Orientation.Rotation_90;
            }
        }
        #endregion
    }
}