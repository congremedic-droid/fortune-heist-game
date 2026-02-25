#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FortuneHeist.Editor
{
    [InitializeOnLoad]
    public static class SceneSetupEditor
    {
        private const string SceneFolder = "Assets/Scenes";
        private const string ScenePath = SceneFolder + "/GameScene.unity";

        static SceneSetupEditor()
        {
            EnsureGameSceneExists();
            EnsureBuildSettingsDefaultScene();
        }

        private static void EnsureGameSceneExists()
        {
            if (!Directory.Exists(SceneFolder))
            {
                Directory.CreateDirectory(SceneFolder);
                AssetDatabase.Refresh();
            }

            if (File.Exists(ScenePath))
            {
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            GameObject systems = new GameObject("GameplaySystems");
            systems.AddComponent<FortuneHeist.BuildingSystem>();
            systems.AddComponent<FortuneHeist.AttackSystem>();
            systems.AddComponent<FortuneHeist.WheelSystem>();
            systems.AddComponent<FortuneHeist.GameplayController>();
            systems.AddComponent<FortuneHeist.DebugMenu>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureBuildSettingsDefaultScene()
        {
            EditorBuildSettingsScene gameScene = new EditorBuildSettingsScene(ScenePath, true);
            EditorBuildSettings.scenes = new[] { gameScene };
        }
    }
}
#endif
