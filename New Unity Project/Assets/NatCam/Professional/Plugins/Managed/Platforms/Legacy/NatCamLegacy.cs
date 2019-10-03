/* 
*   NatCam Professional
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using UnityEngine;
    using System;
    using System.Runtime.InteropServices;
    using Extended;
    using Util = Utilities.Utilities;

    public sealed partial class NatCamLegacy {

        #region --Op vars--
        private CrossBuffer buffer;
        #endregion


        #region --Properties--
        public bool SupportsRecording {
            get {
                return false;
            }
        }

        public bool IsRecording {
            get {
                return false;
            }
        }
        #endregion


        #region --Client API--

        public void PreviewBuffer (out IntPtr ptr, out int width, out int height, out int size) {
            if (buffer.Size != preview.width * preview.height) InitializePreviewBuffer();
            preview.GetPixels32(buffer);
            buffer.CopyToNative();
            ptr = buffer; width = preview.width; height = preview.height; size = buffer.NativeSize;
        }

        public void StartRecording (SaveCallback callback) {
            Util.LogError("Recording is not supported on legacy");
        }

        public void StopRecording() {
            Util.LogError("Recording is not supported on legacy");
        }
        #endregion


        #region --Operations--

        private void InitializePreviewBuffer () {
            if (buffer.IsAllocated) buffer.Release();
            buffer = new CrossBuffer(preview.width, preview.height);
        }

        private void ReleasePreviewBuffer () {
            if (buffer.IsAllocated) buffer.Release();
        }

        public bool SaveVideo (string path, SaveMode mode) {
            return false;
        }
        #endregion


        #region --Utility--

        /// <summary>
        /// This is a cross buffer which maintains pixel data in both managed and native layers
        /// </summary>
        public struct CrossBuffer {
            private byte[] intermediateBuffer; // We need to use this because .NET does not have Marshal.Copy(IntPtr, IntPtr, int)
            public Color32[] ManagedBuffer {get; private set;}
            public IntPtr NativeBuffer {get; private set;} // We need to use this so that we don't disturb GC
            public int Size {get {return IsAllocated ? ManagedBuffer.Length : 0;}}
            public int NativeSize {get {return IsAllocated ? intermediateBuffer.Length : 0;}}
            public bool IsAllocated {get {return intermediateBuffer != null;}}
            public CrossBuffer (int width, int height) {
                int size = width * height * Marshal.SizeOf(typeof(Color32));
                intermediateBuffer = new byte[size];
                ManagedBuffer = new Color32[width * height];
                NativeBuffer = Marshal.AllocHGlobal(size);
                GC.AddMemoryPressure(size);
            }
            public void Release () {
                Marshal.FreeHGlobal(NativeBuffer);
                GC.RemoveMemoryPressure(intermediateBuffer.Length);
                intermediateBuffer = null;
                ManagedBuffer = null;
                NativeBuffer = IntPtr.Zero;
            }
            public void CopyToNative () {
                GCHandle handle = GCHandle.Alloc(ManagedBuffer, GCHandleType.Pinned);
                Marshal.Copy(handle.AddrOfPinnedObject(), intermediateBuffer, 0, NativeSize);
                handle.Free();
                Marshal.Copy(intermediateBuffer, 0, NativeBuffer, NativeSize);
            }
            public void CopyToManaged () { // DEPLOY
                Marshal.Copy(NativeBuffer, intermediateBuffer, 0, NativeSize);
                GCHandle handle = GCHandle.Alloc(ManagedBuffer, GCHandleType.Pinned);
                Marshal.Copy(intermediateBuffer, 0, handle.AddrOfPinnedObject(), NativeSize);
            } 
            public static implicit operator Color32[] (CrossBuffer buffer) {
                return buffer.ManagedBuffer;
            }
            public static implicit operator IntPtr (CrossBuffer buffer) {
                return buffer.NativeBuffer;
            }
        }
        #endregion
    }
}