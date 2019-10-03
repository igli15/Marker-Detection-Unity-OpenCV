/* 
*   NatCam Extended
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCamU.Extended.Utilities {

    using UnityEditor;
    using UnityEngine;
    using System.IO;
	#if UNITY_IOS
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	#endif
	using Core.Utilities;
    
    public static class NatCamEditor {
		
		private const bool SpecificationEnabled = true;
        private const string
		LibraryUsageKey = @"NSPhotoLibraryUsageDescription",
		LibraryUsageDescription = @"Allow access to your photo library to save photos", // Change this as necessary
        GooglePlayVersion = "9.2.1";
		#pragma warning disable 0414
		private static readonly string[] GooglePlayDependencies = {"base", "basement", "vision"};
		#pragma warning restore 0414

        [InitializeOnLoadMethod]
		static void SetCompilerFlags () {
			new [] {
				BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.Standalone,
				BuildTargetGroup.WebGL, BuildTargetGroup.WSA
			}.ForEach(target => {
				const string Extended = "NATCAM_EXTENDED";
				string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
				#pragma warning disable 0162, 0429
				if (SpecificationEnabled && !current.Contains(Extended)) current += ";" + Extended;
				else if (!SpecificationEnabled && current.Contains(Extended)) current = current.Replace(";"+Extended, string.Empty);
				#pragma warning restore 0162, 0429
				PlayerSettings.SetScriptingDefineSymbolsForGroup(target, current);
			});
        }

        #if UNITY_IOS

        [PostProcessBuild]
		static void SetPermissions (BuildTarget buildTarget, string path) {
			if (buildTarget != BuildTarget.iOS) return;
			string plistPath = path + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));
			PlistElementDict rootDictionary = plist.root;
			rootDictionary.SetString(LibraryUsageKey, LibraryUsageDescription);
			File.WriteAllText(plistPath, plist.WriteToString());
		}

        #elif UNITY_ANDROID

        [InitializeOnLoadMethod]
		static void ResolvePlayDependencies () {
            #pragma warning disable 0162
            if (GooglePlayDependencies.Length == 0) return;
            #pragma warning restore 0162
			string destinationPath = Application.dataPath + "/NatCam/Extended/Plugins/Android";
			// Check that we don't already have the library
			if (Directory.GetFiles(destinationPath, "play-services-vision-?.?.?.aar", SearchOption.TopDirectoryOnly).Length > 0) return;
			Debug.Log("NatCam Extended: Acquiring Google Play Services dependencies");
			string gmsRoot = EditorPrefs.GetString("AndroidSdkRoot") + "/extras/google/m2repository/com/google/android/gms";
            GooglePlayDependencies.ForEach(library => File.Copy(
                string.Format("{0}/play-services-{1}/{2}/play-services-{1}-{2}.aar", gmsRoot, library, GooglePlayVersion), 
                string.Format("{0}/play-services-{1}-{2}.aar", destinationPath, library, GooglePlayVersion)
            ));
		}
        #endif
    }
}