/* 
*   NatCam Professional
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Professional.Utilities {

    using System;
    using System.Runtime.InteropServices;
    using Utils = NatCamU.Core.Utilities;

    [Utils::ProDoc(177)]
    public static class Utilities {

        /// <summary>
        /// Strip preview data of padding (area with no information)
        /// </summary>
        /// <param name="buffer">Managed buffer containing the stripped data. Do NOT initialize this.</param>
        /// <param name="outWidth">Desired with of the buffer (without padding bytes)</param>
        /// <param name="handle">Handle to the source preview data</param>
        /// <param name="width">Width of the source preview data</param>
        /// <param name="height">Height of the source preview data</param>
        /// <param name="size">Size of the source preview data</param>
        public static void StripPreviewData (ref byte[] buffer, int outWidth, IntPtr handle, int width, int height, int size) { // NCDOC
            int bpp = size / width / height, newSize = outWidth * height * bpp, inStride = width * bpp, outStride = outWidth * bpp;
            buffer = buffer ?? new byte[newSize];
            if (outWidth < width) for (int i = 0; i < height; i++) Marshal.Copy(new IntPtr(handle.ToInt64() + i * inStride), buffer, i * outStride, outStride);
            else if (outWidth == width) Marshal.Copy(handle, buffer, 0, size);
            else Utils::Utilities.LogError("Cannot strip preview data because desired with is bigger than width");
        }
    }
}