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
            ConfigureSigningFromEnvironment();
            BuildAndroid(BuildOptions.StrictMode, artifact);
            EditorUserBuildSettings.buildAppBundle = false;
        }


        private static void ConfigureSigningFromEnvironment()
        {
            string useCustomKeystore = Environment.GetEnvironmentVariable("FH_ANDROID_USE_CUSTOM_KEYSTORE");
            bool enabled = string.Equals(useCustomKeystore, "true", StringComparison.OrdinalIgnoreCase);
            if (!enabled)
            {
                return;
            }

            string keystoreName = Environment.GetEnvironmentVariable("FH_ANDROID_KEYSTORE_NAME");
            string keystorePass = Environment.GetEnvironmentVariable("FH_ANDROID_KEYSTORE_PASS");
            string aliasName = Environment.GetEnvironmentVariable("FH_ANDROID_KEYALIAS_NAME");
            string aliasPass = Environment.GetEnvironmentVariable("FH_ANDROID_KEYALIAS_PASS");

            if (string.IsNullOrWhiteSpace(keystoreName) ||
                string.IsNullOrWhiteSpace(keystorePass) ||
                string.IsNullOrWhiteSpace(aliasName) ||
                string.IsNullOrWhiteSpace(aliasPass))
            {
                throw new Exception("Custom Android signing enabled but one or more keystore variables are missing.");
            }

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = keystoreName;
            PlayerSettings.Android.keystorePass = keystorePass;
            PlayerSettings.Android.keyaliasName = aliasName;
            PlayerSettings.Android.keyaliasPass = aliasPass;
            Debug.Log("Android custom signing configured from environment variables.");
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
