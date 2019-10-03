/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

namespace NatCamU.Core.Utilities {

	using UnityEditor;
	using System;

	#if UNITY_IOS
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using System.IO;
	#endif

	public static class NatCamEditor {

		private const string
		CameraUsageKey = @"NSCameraUsageDescription",
		CameraUsageDescription = @"Allow this app to use the camera.", // Change this as necessary
		VersionNumber = "NATCAM_15";

		[InitializeOnLoadMethod]
		static void SetCompilerFlags () {
			// Define C bridge dependency // Needed to compile AOT (IL2CPP)
			Func<BuildTargetGroup, bool> cdependency = (grp) => {
				if (grp == BuildTargetGroup.iOS) return true;
				return false;
			};
			// Iterate through build targets
			new [] {
				BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.Standalone,
				BuildTargetGroup.WebGL, BuildTargetGroup.WSA
			}.ForEach(target => {
				const string Core = "NATCAM_CORE", Cbridge = "INATCAM_C";
				string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
				if (!current.Contains(VersionNumber)) current += (current.Equals(string.Empty) ? "" : ";") + VersionNumber;
				if (cdependency(target) && !current.Contains(Cbridge)) current += ";" + Cbridge;
				if (!current.Contains(Core)) current += ";" + Core;
				PlayerSettings.SetScriptingDefineSymbolsForGroup(target, current);
			});
		}

		#if UNITY_IOS

		[PostProcessBuild]
		static void LinkFrameworks (BuildTarget buildTarget, string path) {
			if (buildTarget != BuildTarget.iOS) return;
			string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject proj = new PBXProject();
			proj.ReadFromString(File.ReadAllText(projPath));
			string target = proj.TargetGuidByName("Unity-iPhone");
			new [] {
				"Accelerate.framework",
				"AssetsLibrary.framework",
				"CoreImage.framework"
			}.ForEach(framework => proj.AddFrameworkToProject(target, framework, true));
			File.WriteAllText(projPath, proj.WriteToString());
		}

		[PostProcessBuild]
		static void SetPermissions (BuildTarget buildTarget, string path) {
			if (buildTarget != BuildTarget.iOS) return;
			string plistPath = path + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));
			PlistElementDict rootDictionary = plist.root;
			rootDictionary.SetString(CameraUsageKey, CameraUsageDescription);
			File.WriteAllText(plistPath, plist.WriteToString());
		}
		#endif
	}
}