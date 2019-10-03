/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core {

	using UnityEngine;
	using System;
	using Utilities;

    #region --Delegates--
    /// <summary>
    /// A delegate type that NatCam uses to initialize NatCamPreviewBehaviours.
    /// </summary>
    [CoreDoc(37)]
	public delegate void PreviewCallback ();
    /// <summary>
    /// A delegate type that NatCam uses to pass a captured photo to subscribers.
    /// </summary>
	[CoreDoc(38)]
    public delegate void PhotoCallback (Texture2D photo, Orientation orientation);
    #endregion


    #region --Enumerations--
    [CoreDoc(39)] public enum ExposureMode : byte {
	    [CoreDoc(40)] AutoExpose = 0,
		[CoreDoc(41)] Locked = 1
	}
    [CoreDoc(42)] public enum Facing : byte {
		[CoreDoc(43)] Rear = 0,
		[CoreDoc(44)] Front = 1
    }
    [CoreDoc(45)] public enum FlashMode : byte {
		[CoreDoc(46)] Auto = 0,
		[CoreDoc(47)] On = 1,
		[CoreDoc(48)] Off = 2
	}
	[CoreDoc(49), Flags] public enum FocusMode : byte {
        [CoreDoc(50)] Off = 0,
        [CoreDoc(51)] TapToFocus = 1,
        [CoreDoc(52)] AutoFocus = TapToFocus << 1,
        [CoreDoc(53, 1)] SoftFocus = TapToFocus << 2 | AutoFocus,
        [CoreDoc(54, 2)] MacroFocus = TapToFocus << 3 | SoftFocus
    }
	[CoreDoc(55)] public enum FrameratePreset : byte {
		[CoreDoc(56)] Default = 30,
        [CoreDoc(57)] Smooth = 60,
        [CoreDoc(58)] SlowMotion = 120,
        [CoreDoc(59)] HighestFramerate = 240,
        [CoreDoc(60)] LowestFramerate = 15
    }
    [Flags, ExtDoc(123)] public enum Orientation : byte { // Update native mappings
		[ExtDoc(124)] Rotation_0 = 0,
		[ExtDoc(125)] Rotation_90 = 1,
		[ExtDoc(126)] Rotation_180 = 2,
		[ExtDoc(127)] Rotation_270 = 3,
        Mirror = 8, // NCDOC
	}
    [CoreDoc(61)] public enum ResolutionPreset : byte {
		[CoreDoc(62)] HD = 0,
		[CoreDoc(63)] FullHD = 1,
		[CoreDoc(64)] HighestResolution = 2,
        [CoreDoc(65)] MediumResolution = 3,
		[CoreDoc(66)] LowestResolution = 4,
	}
    [CoreDoc(179)] public enum ScaleMode : byte {
		[CoreDoc(184)] None = 0,
		[CoreDoc(181)] AdjustWidth = 1,
		[CoreDoc(180)] AdjustHeight = 2,
		Letterbox = 3, // NCDOC
		[CoreDoc(182)] FillView = 4,
		[CoreDoc(183)] FillScreen = 5,
		
	}
    [CoreDoc(67)] public enum Switch : byte {
		[CoreDoc(68)] Off = 0,
		[CoreDoc(69)] On = 1
	}

    [Flags, CoreDoc(185)] public enum ZoomMode : byte {
		[CoreDoc(186)] DigitalZoom = 1,
		[CoreDoc(187)] OpticalZoom = 2
	}
    #endregion
}