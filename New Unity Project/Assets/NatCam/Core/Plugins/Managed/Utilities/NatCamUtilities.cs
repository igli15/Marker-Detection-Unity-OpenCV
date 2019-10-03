/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Utilities {

    using UnityEngine;
    using System;
    using System.Collections;
    using Dispatch;

    public static class Utilities {

        #region --Logging--

        private static StackTraceLogType currentTrace;
        public static bool verbose;

        public static void Log (string log) {
            EnterLog();
            Debug.Log("NatCam: "+log);
            ExitLog();
        }

        public static void LogVerbose (string log) {
            if (!verbose) return;
            EnterLog();
            Debug.Log("NatCam Logging: "+log);
            ExitLog();
        }
        
        public static void LogError (string warning) {
            EnterLog(LogType.Warning, StackTraceLogType.ScriptOnly);
            Debug.LogWarning("NatCam Error: "+warning);
            ExitLog(LogType.Warning);
        }

        private static void EnterLog (LogType type = LogType.Log, StackTraceLogType stack = StackTraceLogType.None) {
            #if UNITY_5_4_OR_NEWER
            currentTrace = Application.GetStackTraceLogType(type);
            Application.SetStackTraceLogType(type, stack);
            #else
            currentTrace = Application.stackTraceLogType;
            Application.stackTraceLogType = stack;
            #endif
        }

        private static void ExitLog (LogType type = LogType.Log) {
            #if UNITY_5_4_OR_NEWER
            Application.SetStackTraceLogType(type, currentTrace);
            #else
            Application.stackTraceLogType = currentTrace;
            #endif
        }
        #endregion


        #region --Extensions--

        public static Coroutine Invoke (this IEnumerator routine, MonoBehaviour mono) {
            return mono.StartCoroutine(routine);
        }
        
        public static void Terminate (this Coroutine routine, MonoBehaviour mono) {
            mono.StopCoroutine(routine);
        }

        public static void ForEach<T> (this T[] array, System.Action<T> action) {
            if (array == null) return;
            for (int i = 0, len = array.Length; i < len; i++) action(array[i]);
        }
        #endregion


        #region --Helpers--

        /// <summary>
        /// Rotates an image and returns it through the callback. Note that the image will be modified
        /// </summary>
        /// <param name="texture">The texture to be modified</param>
        /// <param name="rotation">Rotation to be applied to the buffer. This is equivalent to the Orientation struct</param>
        /// <param name="workerDispatch">Worker dispatch where the image will be rotated. If null, the calling thread will be used</param>
        /// <param name="mainDispatch">Main dispatch where callback will be invoked (so as to maintain Unity API thread access restrictions)</param>
        /// <param name="callback">Callback to be invoked with the rotated texture</param>
        public static void RotateImage (Texture2D texture, Orientation rotation, IDispatch workerDispatch, IDispatch mainDispatch, PhotoCallback callback) { // NCDOC
            if (callback == null) return;
            if (rotation == 0) {callback(texture, rotation); return;}
            Color32[] src = texture.GetPixels32(), dst = new Color32[texture.width * texture.height];
            Action completion = () => {
                if (((int)rotation & 7) % 2 != 0) texture.Resize(texture.height, texture.width);
                texture.SetPixels32(dst);
                texture.Apply();
                src = dst = null;
                GC.Collect(); // No guarantees, but just have it anyway
                callback(texture, (Orientation)rotation);
            };
            Action process = () => {
                RotateImage(dst, src, texture.width, texture.height, (byte)rotation);
                if (mainDispatch != null) mainDispatch.Dispatch(completion);
                else completion();
            };
            if (workerDispatch != null) workerDispatch.Dispatch(process);
            else process();
        }

        /// <summary>
        /// Rotates an image buffer. This is best offloaded to a worker thread using ConcurrentDispatch
        /// </summary>
        /// <param name="dst">The destination buffer of the rotated pixels</param>
        /// <param name="src">The source buffer</param>
        /// <param name="width">The width of the source buffer</param>
        /// <param name="height">The height of the source buffer</param>
        /// <param name="orientation">Rotation to be applied to the buffer. This is equivalent to the Orientation struct</param>
        public static void RotateImage (Color32[] dst, Color32[] src, int width, int height, byte orientation) { // NCDOC
            if (src == null || dst == null || src.Length == 0 || src.Length != width * height || dst.Length != src.Length || orientation == 0) return;
            int rotation = orientation & 7, mirror = orientation >> 3;
            Func<int, int> kernel = null;
            switch (rotation) {
                case 0: kernel = i => i; break;                                                                 // 0
                case 1: kernel = i => height * (width - 1 - i % width) + i / width; break;                      // 90
                case 2: kernel = i => src.Length - 1 - i; break;                                                // 180
                case 3: kernel = i => src.Length - 1 - (height * (width - 1 - i % width) + i / width); break;   // 270
            }
            for (int i = 0; i < src.Length; i++) dst[kernel(i)] = src[i];
            if (mirror == 1) for (int i = 0, w = rotation % 2 == 0 ? width : height, h = rotation % 2 == 0 ? height : width; i < h; i++) Array.Reverse(dst, i * w, w);
        }

        public static void Dimensions (this ResolutionPreset preset, out int width, out int height) {
            switch (preset) {
                case ResolutionPreset.FullHD: width = 1920; height = 1080; break;
                case ResolutionPreset.HD: width = 1280; height = 720; break;
                case ResolutionPreset.MediumResolution: width = 640; height = 480; break;
                case ResolutionPreset.HighestResolution: width = 9999; height = 9999; break; // NatCam will pick the resolution closest to this, hence the highest
                case ResolutionPreset.LowestResolution: width = 50; height = 50; break; // NatCam will pick the resolution closest to this, hence the lowest
                default: width = height = 0; break;
            }
        }
        #endregion
    }
}