#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace FortuneHeist.Editor
{
    public static class AndroidBuildPipeline
    {
        private const string BuildOutputFolder = "Builds/Android";

        [MenuItem("FortuneHeist/Build/Android APK")]
        public static void BuildApkFromMenu()
        {
            BuildAndroid(BuildOptions.None, "fortune-heist.apk");
        }

        [MenuItem("FortuneHeist/Build/Android AAB")]
        public static void BuildAabFromMenu()
        {
            EditorUserBuildSettings.buildAppBundle = true;
            BuildAndroid(BuildOptions.None, "fortune-heist.aab");
            EditorUserBuildSettings.buildAppBundle = false;
        }

        public static void BuildAndroidForCI()
        {
            string artifact = Environment.GetEnvironmentVariable("FH_ANDROID_ARTIFACT");
            bool bundle = string.Equals(Environment.GetEnvironmentVariable("FH_ANDROID_BUNDLE"), "true", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(artifact))
            {
                artifact = bundle ? "fortune-heist-ci.aab" : "fortune-heist-ci.apk";
            }

            EditorUserBuildSettings.buildAppBundle = bundle;
            BuildAndroid(BuildOptions.StrictMode, artifact);
            EditorUserBuildSettings.buildAppBundle = false;
        }

        private static void BuildAndroid(BuildOptions options, string artifactName)
        {
            if (!Directory.Exists(BuildOutputFolder))
            {
                Directory.CreateDirectory(BuildOutputFolder);
            }

            string location = Path.Combine(BuildOutputFolder, artifactName);
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                target = BuildTarget.Android,
                locationPathName = location,
                options = options
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception($"Android build failed: {report.summary.result}");
            }

            Debug.Log($"Android build completed: {location}");
        }

        private static string[] GetEnabledScenes()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            string[] enabled = Array.FindAll(scenes, s => s.enabled).Select(s => s.path).ToArray();
            return enabled;
        }
    }
}
#endif
