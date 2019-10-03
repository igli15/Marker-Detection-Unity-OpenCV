/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Extended.Utilities {

    using UnityEngine;
    using System;
    using System.IO;
    
    public static class Utilities {

        #region --Metadata--

        public static long CurrentTime {
            get {
                return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }
        }

        /// <summary>
        /// Do not use. We might add this functionality in a later NatCam Extended release
        /// </summary>
		public static bool GetLandmark (Face face, Face.Landmark landmark, ref Vector2 position) {
            byte[] landmarkData = (byte[])face.supplement;
            for (int i = 0; landmarkData != null && i < landmarkData.Length; i += 9) if (landmarkData[i] == (byte)landmark) {
                position.Set(BitConverter.ToSingle(landmarkData, i + 1), BitConverter.ToSingle(landmarkData, i + 5));
                return true;
            }
            return false;
		}
        #endregion
        

        #region --SavePhoto API--

        public static void SavePhoto (byte[] png, SaveMode mode, SaveCallback callback) {
            if (png == null) return;
            string fileName = string.Format("photo_{0}.png", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff")),
            path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(path, png);
            if (callback != null) callback((SaveMode)mode, path);
        }
        #endregion
    }
}