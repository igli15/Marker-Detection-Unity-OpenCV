/* 
*   NatCam Professional
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCamU.Professional.Utilities {

    using UnityEditor;
	#if UNITY_IOS
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using System.IO;
	#endif
	using Core.Utilities;

    public static class NatCamEditor {

		private const bool SpecificationEnabled = true;
        private const string
		MicrophoneUsageKey = @"NSMicrophoneUsageDescription",
        MicrophoneUsageDescription = @"Allow microphone input for video recording."; // Change this as necessary

        [InitializeOnLoadMethod]
		static void SetCompilerFlags () {
			new [] {
				BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.Standalone,
				BuildTargetGroup.WebGL, BuildTargetGroup.WSA
			}.ForEach(target => {
				const string Professional = "NATCAM_PROFESSIONAL";
				string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
				#pragma warning disable 0162, 0429
				if (SpecificationEnabled && !current.Contains(Professional)) current += ";" + Professional;
				else if (!SpecificationEnabled && current.Contains(Professional)) current = current.Replace(";"+Professional, string.Empty);
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
			rootDictionary.SetString(MicrophoneUsageKey, MicrophoneUsageDescription);
			File.WriteAllText(plistPath, plist.WriteToString());
		}
        #endif
    }
}