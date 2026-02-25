#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FortuneHeist.Editor
{
    public static class SetupBootstrapEditor
    {
        [MenuItem("FortuneHeist/Setup/Bootstrap Everything")]
        public static void BootstrapEverything()
        {
            SceneSetupEditor.EnsureGameSceneReady();
            EditorSceneManager.OpenScene(SceneSetupEditor.GameSceneAssetPath, OpenSceneMode.Single);

            SceneUiAutoWireEditor.AutoWireSceneUi();

            List<string> issues = ValidateCurrentScene();
            if (issues.Count == 0)
            {
                Debug.Log("[FortuneHeist] Bootstrap Everything completed. Scene is ready to press Play.");
            }
            else
            {
                Debug.LogWarning("[FortuneHeist] Bootstrap completed with pending issues:\n- " + string.Join("\n- ", issues));
            }
        }

        [MenuItem("FortuneHeist/Setup/Validate Current Scene Wiring")]
        public static void ValidateSceneMenu()
        {
            List<string> issues = ValidateCurrentScene();
            if (issues.Count == 0)
            {
                Debug.Log("[FortuneHeist] Scene wiring looks good.");
                return;
            }

            Debug.LogWarning("[FortuneHeist] Missing references detected:\n- " + string.Join("\n- ", issues));
        }

        private static List<string> ValidateCurrentScene()
        {
            List<string> issues = new List<string>();

            GameplayController gameplayController = Object.FindObjectOfType<GameplayController>();
            BuildingSystem buildingSystem = Object.FindObjectOfType<BuildingSystem>();
            GameplayHudPresenter hudPresenter = Object.FindObjectOfType<GameplayHudPresenter>();
            FtueOverlayPresenter ftueOverlayPresenter = Object.FindObjectOfType<FtueOverlayPresenter>();
            DebugMenu debugMenu = Object.FindObjectOfType<DebugMenu>();

            if (gameplayController == null) issues.Add("GameplayController component not found in scene.");
            if (buildingSystem == null) issues.Add("BuildingSystem component not found in scene.");
            if (hudPresenter == null) issues.Add("GameplayHudPresenter component not found in scene.");
            if (ftueOverlayPresenter == null) issues.Add("FtueOverlayPresenter component not found in scene.");
            if (debugMenu == null) issues.Add("DebugMenu component not found in scene.");

            if (gameplayController != null)
            {
                SerializedObject so = new SerializedObject(gameplayController);
                ValidateObjectRef(so, "spinText", "GameplayController.spinText", issues);
                ValidateObjectRef(so, "targetListRoot", "GameplayController.targetListRoot", issues);
                ValidateObjectRef(so, "targetButtonPrefab", "GameplayController.targetButtonPrefab", issues);
                ValidateObjectRef(so, "spinButton", "GameplayController.spinButton", issues);
            }

            if (buildingSystem != null)
            {
                SerializedObject so = new SerializedObject(buildingSystem);
                ValidateObjectRef(so, "buildingListRoot", "BuildingSystem.buildingListRoot", issues);
                ValidateObjectRef(so, "buildingRowButtonPrefab", "BuildingSystem.buildingRowButtonPrefab", issues);
                ValidateObjectRef(so, "goldText", "BuildingSystem.goldText", issues);
            }

            if (hudPresenter != null)
            {
                SerializedObject so = new SerializedObject(hudPresenter);
                ValidateObjectRef(so, "goldText", "GameplayHudPresenter.goldText", issues);
                ValidateObjectRef(so, "spinsText", "GameplayHudPresenter.spinsText", issues);
                ValidateObjectRef(so, "selectedTargetText", "GameplayHudPresenter.selectedTargetText", issues);
                ValidateObjectRef(so, "ftueText", "GameplayHudPresenter.ftueText", issues);
                ValidateObjectRef(so, "dailyRewardText", "GameplayHudPresenter.dailyRewardText", issues);
                ValidateObjectRef(so, "resultText", "GameplayHudPresenter.resultText", issues);
            }

            if (ftueOverlayPresenter != null)
            {
                SerializedObject so = new SerializedObject(ftueOverlayPresenter);
                ValidateObjectRef(so, "overlayRoot", "FtueOverlayPresenter.overlayRoot", issues);
                ValidateObjectRef(so, "titleText", "FtueOverlayPresenter.titleText", issues);
                ValidateObjectRef(so, "bodyText", "FtueOverlayPresenter.bodyText", issues);
                ValidateObjectRef(so, "blockerHintText", "FtueOverlayPresenter.blockerHintText", issues);
            }

            if (debugMenu != null)
            {
                SerializedObject so = new SerializedObject(debugMenu);
                ValidateObjectRef(so, "debugOutput", "DebugMenu.debugOutput", issues);
            }

            return issues;
        }

        private static void ValidateObjectRef(SerializedObject so, string propertyName, string label, List<string> issues)
        {
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop == null)
            {
                issues.Add(label + " property not found (script changed?).");
                return;
            }

            if (prop.objectReferenceValue == null)
            {
                issues.Add(label + " is not assigned.");
            }
        }
    }
}
#endif
