/* 
*   NatCam Extended
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Extended {

	using UnityEngine;
	using System;
	using Core;
	using Core.Utilities;
	using Util = Utilities.Utilities;


    #region --Delegates--
    /// <summary>
    /// A delegate type used to pass information about detected metadata to subscribers.
    /// </summary>
	[ExtDoc(102)]
	public delegate void MetadataCallback<Metadata> (Metadata metadata) where Metadata : IMetadata;
	/// <summary>
    /// A delegate type used to pass the path of a saved photo to subscribers.
    /// </summary>
	[ExtDoc(104)]
	public delegate void SaveCallback (SaveMode mode, string path);
    #endregion


    #region --Enumerations--
    [ExtDoc(105), Flags] public enum BarcodeFormat : int { // Update native mappings
		[ExtDoc(106)] QR = 1,
		[ExtDoc(107)] EAN_13 = 2,
		[ExtDoc(108)] EAN_8 = 4,
		[ExtDoc(109)] DATA_MATRIX = 8,
		[ExtDoc(110)] PDF_417 = 16,
		[ExtDoc(111)] CODE_128 = 32,
		[ExtDoc(112)] CODE_93 = 64,
		[ExtDoc(113)] CODE_39 = 128,
		[ExtDoc(114)] ITF = 256,
		[ExtDoc(115)] ALL = 511
	}

	[ExtDoc(119), Flags] public enum SaveMode : byte { // Update native mappings
		[ExtDoc(120)] SaveToAppDocuments = 1,
		[ExtDoc(121)] SaveToPhotoGallery = 2,
		[ExtDoc(122)] SaveToPhotoAlbum = 4
	}
    #endregion


    #region --Metadata--

	public interface IMetadata {
		Rect rect {get;}
		long timestamp {get;}
		object supplement {get;}

		string ToString();
	}

	[ExtDoc(140)] public struct Barcode : IMetadata {
		[ExtDoc(188)] public Rect rect {get; private set;}
		[ExtDoc(143)] public long timestamp {get {return Util.CurrentTime - NatCam.Implementation.MetadataTime(time);}}
		[ExtDoc(189)] public object supplement {get; private set;} // Unused
		[ExtDoc(141)] public readonly string data;
		[ExtDoc(142)] public readonly BarcodeFormat format;
		private readonly long time;
		[ExtDoc(144)] public Barcode (float x, float y, float w, float h, long timestamp, object supplement, string data, int format) { // Must be constructed on the main thread
			this.rect = new Rect(x, y, w, h);
			this.time = timestamp <= 0 ? Time.frameCount : timestamp;
			this.supplement = supplement;
			this.data = data;
			this.format = (BarcodeFormat)format;
		}
		[ExtDoc(145), ExtCode(18)] public override string ToString () {
			return "["+format+"]:"+data;
		}
	}

	public struct Text : IMetadata { // NCDOC
		[ExtDoc(188)] public Rect rect {get; private set;}
		[ExtDoc(143)] public long timestamp {get {return Util.CurrentTime - NatCam.Implementation.MetadataTime(time);}}
		[ExtDoc(189)] public object supplement {get; private set;} // Unused
		public string text {get; private set;}
		private readonly long time;
		public Text (float x, float y, float w, float h, long timestamp, object supplement, string text) {
			this.rect = new Rect(x, y, w, h);
			this.time = timestamp <= 0 ? Time.frameCount : timestamp;
			this.supplement = supplement;
			this.text = text;
		}
		public override string ToString () {
			return text;
		}
	}

	[ExtDoc(146)] public struct Face : IMetadata, IEquatable<Face> {
		public enum Landmark : byte {LeftEye = 1, RightEye, Mouth, LeftCheek, RightCheek, LeftEar, RightEar, NoseBase}
		[ExtDoc(188)] public Rect rect {get; private set;}
		[ExtDoc(143)] public long timestamp {get {return Util.CurrentTime - NatCam.Implementation.MetadataTime(time);}}
		[ExtDoc(189)] public object supplement {get; private set;}
		[ExtDoc(147)] public readonly int id;
		[ExtDoc(190)] public readonly Vector2 rotation; // (roll, yaw)
		private readonly long time;
		[ExtDoc(153)] public Face (float x, float y, float w, float h, long timestamp, object supplement, int id, float roll, float yaw) {
			this.rect = new Rect(x, y, w, h);
			this.time = timestamp <= 0 ? Time.frameCount : timestamp;
			this.supplement = supplement;
			this.id = id;
			this.rotation = new Vector2(roll, yaw);
		}
		[ExtDoc(154), ExtCode(19)] public override string ToString () {
			return "["+id+"]:"+rect;
		}
		public bool Equals (Face other) {
			return other == this;
		}
		public override bool Equals (object other) {
			return other is Face && Equals((Face)other);
		}
		public override int GetHashCode () {
			return id;
		}
		public static bool operator == (Face a, Face b) {
			return a.GetHashCode() == b.GetHashCode();
		}
		public static bool operator != (Face a, Face b) {
			return a.GetHashCode() != b.GetHashCode();
		}
	}
    #endregion
}