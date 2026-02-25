#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FortuneHeist.Editor
{
    public static class SceneUiAutoWireEditor
    {
        private const string PrefabFolder = "Assets/Prefabs";
        private const string RowButtonPrefabPath = PrefabFolder + "/RowButton.prefab";

        [MenuItem("FortuneHeist/Setup/Auto Wire Scene UI")]
        public static void AutoWireSceneUi()
        {
            GameplayController gameplayController = Object.FindObjectOfType<GameplayController>();
            if (gameplayController == null)
            {
                Debug.LogError("[FortuneHeist] GameplayController not found in scene. Open GameScene first.");
                return;
            }

            BuildingSystem buildingSystem = Object.FindObjectOfType<BuildingSystem>();
            AttackSystem attackSystem = Object.FindObjectOfType<AttackSystem>();
            WheelSystem wheelSystem = Object.FindObjectOfType<WheelSystem>();
            SaveSystem saveSystem = Object.FindObjectOfType<SaveSystem>();
            FtueSystem ftueSystem = Object.FindObjectOfType<FtueSystem>();
            FtueOverlayPresenter ftueOverlayPresenter = Object.FindObjectOfType<FtueOverlayPresenter>();
            DailyRewardSystem dailyRewardSystem = Object.FindObjectOfType<DailyRewardSystem>();
            RemoteConfigService remoteConfigService = Object.FindObjectOfType<RemoteConfigService>();
            ThemeConfigService themeConfigService = Object.FindObjectOfType<ThemeConfigService>();
            AnalyticsSystem analyticsSystem = Object.FindObjectOfType<AnalyticsSystem>();
            GameplayHudPresenter hudPresenter = Object.FindObjectOfType<GameplayHudPresenter>();
            DebugMenu debugMenu = Object.FindObjectOfType<DebugMenu>();

            Canvas canvas = EnsureCanvas();
            EnsureEventSystem();

            Text goldText = CreateText(canvas.transform, "GoldText", new Vector2(15, -20));
            Text spinsText = CreateText(canvas.transform, "SpinsText", new Vector2(15, -50));
            Text targetText = CreateText(canvas.transform, "SelectedTargetText", new Vector2(15, -80));
            Text ftueText = CreateText(canvas.transform, "FtueText", new Vector2(15, -110));
            Text dailyText = CreateText(canvas.transform, "DailyRewardText", new Vector2(15, -140));
            Text resultText = CreateText(canvas.transform, "ResultText", new Vector2(15, -175));
            resultText.rectTransform.sizeDelta = new Vector2(780, 28);

            Image accentBar = CreateImage(canvas.transform, "AccentBar", new Vector2(15, -205), new Vector2(800, 8));

            Button spinButton = CreateButton(canvas.transform, "SpinButton", "SPIN", new Vector2(15, -230), new Vector2(170, 40));
            RectTransform targetListRoot = CreateUiRoot(canvas.transform, "TargetListRoot", new Vector2(15, -280), new Vector2(520, 220));
            RectTransform buildingListRoot = CreateUiRoot(canvas.transform, "BuildingListRoot", new Vector2(560, -20), new Vector2(380, 480));

            GameObject overlayRoot = CreateOverlayRoot(canvas.transform, "FtueOverlayRoot");
            Text overlayTitle = CreateText(overlayRoot.transform, "FtueTitleText", new Vector2(20, -20));
            overlayTitle.fontSize = 22;
            Text overlayBody = CreateText(overlayRoot.transform, "FtueBodyText", new Vector2(20, -55));
            overlayBody.rectTransform.sizeDelta = new Vector2(750, 60);
            Text overlayHint = CreateText(overlayRoot.transform, "FtueBlockerHintText", new Vector2(20, -120));
            overlayHint.color = new Color(1f, 0.75f, 0.25f);

            Text debugOutput = CreateText(canvas.transform, "DebugOutputText", new Vector2(15, -520));
            debugOutput.rectTransform.sizeDelta = new Vector2(950, 30);

            UIFeedbackAnimator feedbackAnimator = resultText.GetComponent<UIFeedbackAnimator>();
            if (feedbackAnimator == null)
            {
                feedbackAnimator = resultText.gameObject.AddComponent<UIFeedbackAnimator>();
            }

            Button rowButtonPrefab = EnsureRowButtonPrefab();

            WireGameplayController(
                gameplayController,
                buildingSystem,
                attackSystem,
                wheelSystem,
                saveSystem,
                ftueSystem,
                ftueOverlayPresenter,
                dailyRewardSystem,
                remoteConfigService,
                themeConfigService,
                analyticsSystem,
                hudPresenter,
                spinsText,
                targetListRoot,
                rowButtonPrefab,
                spinButton);

            WireBuildingSystem(buildingSystem, buildingListRoot, rowButtonPrefab, goldText, gameplayController, remoteConfigService);
            WireHudPresenter(hudPresenter, goldText, spinsText, targetText, ftueText, dailyText, resultText, accentBar, feedbackAnimator);
            WireFtueOverlay(ftueOverlayPresenter, overlayRoot, overlayTitle, overlayBody, overlayHint);
            WireDebugMenu(debugMenu, buildingSystem, wheelSystem, gameplayController, saveSystem, debugOutput);

            SetSpinButtonListener(spinButton, gameplayController);

            EditorUtility.SetDirty(gameplayController);
            EditorUtility.SetDirty(buildingSystem);
            EditorUtility.SetDirty(hudPresenter);
            EditorUtility.SetDirty(ftueOverlayPresenter);
            EditorUtility.SetDirty(debugMenu);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            Debug.Log("[FortuneHeist] Auto Wire Scene UI completed. Press Play.");
        }

        private static Canvas EnsureCanvas()
        {
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                return canvas;
            }

            GameObject canvasGo = new GameObject("Canvas");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static Font GetUiFont()
        {
            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.GetComponent<Text>();
            }

            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = new Vector2(500, 24);

            Text text = go.GetComponent<Text>();
            text.font = GetUiFont();
            text.fontSize = 18;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = Color.white;
            text.text = name;
            return text;
        }

        private static Image CreateImage(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.GetComponent<Image>();
            }

            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = size;

            Image image = go.GetComponent<Image>();
            image.color = new Color(0.35f, 0.85f, 1f, 0.8f);
            return image;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, Vector2 size)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.GetComponent<Button>();
            }

            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = size;

            Image bg = go.GetComponent<Image>();
            bg.color = new Color(0.2f, 0.45f, 0.9f, 1f);

            Text text = CreateText(go.transform, "Text", new Vector2(0, 0));
            RectTransform textRt = text.rectTransform;
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.pivot = new Vector2(0.5f, 0.5f);
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = label;

            return go.GetComponent<Button>();
        }

        private static RectTransform CreateUiRoot(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.GetComponent<RectTransform>();
            }

            GameObject go = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = size;

            VerticalLayoutGroup layout = go.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 6;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            ContentSizeFitter fitter = go.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return rt;
        }

        private static GameObject CreateOverlayRoot(Transform parent, string name)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0, -20);
            rt.sizeDelta = new Vector2(820, 170);

            Image image = go.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.5f);
            return go;
        }

        private static Button EnsureRowButtonPrefab()
        {
            Button prefab = AssetDatabase.LoadAssetAtPath<Button>(RowButtonPrefabPath);
            if (prefab != null)
            {
                return prefab;
            }

            if (!AssetDatabase.IsValidFolder(PrefabFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            GameObject row = new GameObject("RowButton", typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rowRt = row.GetComponent<RectTransform>();
            rowRt.sizeDelta = new Vector2(380, 34);
            row.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

            Text label = CreateText(row.transform, "Text", new Vector2(8, -4));
            RectTransform labelRt = label.rectTransform;
            labelRt.anchorMin = new Vector2(0, 0);
            labelRt.anchorMax = new Vector2(1, 1);
            labelRt.pivot = new Vector2(0.5f, 0.5f);
            labelRt.offsetMin = new Vector2(8, 4);
            labelRt.offsetMax = new Vector2(-8, -4);
            label.alignment = TextAnchor.MiddleLeft;
            label.text = "Row";

            GameObject prefabGo = PrefabUtility.SaveAsPrefabAsset(row, RowButtonPrefabPath);
            Object.DestroyImmediate(row);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return prefabGo.GetComponent<Button>();
        }

        private static void WireGameplayController(
            GameplayController controller,
            BuildingSystem buildingSystem,
            AttackSystem attackSystem,
            WheelSystem wheelSystem,
            SaveSystem saveSystem,
            FtueSystem ftueSystem,
            FtueOverlayPresenter ftueOverlayPresenter,
            DailyRewardSystem dailyRewardSystem,
            RemoteConfigService remoteConfigService,
            ThemeConfigService themeConfigService,
            AnalyticsSystem analyticsSystem,
            GameplayHudPresenter hudPresenter,
            Text spinsText,
            RectTransform targetListRoot,
            Button targetButtonPrefab,
            Button spinButton)
        {
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("buildingSystem").objectReferenceValue = buildingSystem;
            so.FindProperty("attackSystem").objectReferenceValue = attackSystem;
            so.FindProperty("wheelSystem").objectReferenceValue = wheelSystem;
            so.FindProperty("saveSystem").objectReferenceValue = saveSystem;
            so.FindProperty("ftueSystem").objectReferenceValue = ftueSystem;
            so.FindProperty("ftueOverlayPresenter").objectReferenceValue = ftueOverlayPresenter;
            so.FindProperty("dailyRewardSystem").objectReferenceValue = dailyRewardSystem;
            so.FindProperty("remoteConfigService").objectReferenceValue = remoteConfigService;
            so.FindProperty("themeConfigService").objectReferenceValue = themeConfigService;
            so.FindProperty("analyticsSystem").objectReferenceValue = analyticsSystem;
            so.FindProperty("hudPresenter").objectReferenceValue = hudPresenter;

            so.FindProperty("spinText").objectReferenceValue = spinsText;
            so.FindProperty("targetListRoot").objectReferenceValue = targetListRoot;
            so.FindProperty("targetButtonPrefab").objectReferenceValue = targetButtonPrefab;
            so.FindProperty("spinButton").objectReferenceValue = spinButton;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireBuildingSystem(
            BuildingSystem buildingSystem,
            RectTransform buildingListRoot,
            Button buildingRowButtonPrefab,
            Text goldText,
            GameplayController gameplayController,
            RemoteConfigService remoteConfigService)
        {
            SerializedObject so = new SerializedObject(buildingSystem);
            so.FindProperty("buildingListRoot").objectReferenceValue = buildingListRoot;
            so.FindProperty("buildingRowButtonPrefab").objectReferenceValue = buildingRowButtonPrefab;
            so.FindProperty("goldText").objectReferenceValue = goldText;
            so.FindProperty("gameplayController").objectReferenceValue = gameplayController;
            so.FindProperty("remoteConfigService").objectReferenceValue = remoteConfigService;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireHudPresenter(
            GameplayHudPresenter hudPresenter,
            Text goldText,
            Text spinsText,
            Text selectedTarget,
            Text ftueText,
            Text dailyText,
            Text resultText,
            Image accentBar,
            UIFeedbackAnimator animator)
        {
            SerializedObject so = new SerializedObject(hudPresenter);
            so.FindProperty("goldText").objectReferenceValue = goldText;
            so.FindProperty("spinsText").objectReferenceValue = spinsText;
            so.FindProperty("selectedTargetText").objectReferenceValue = selectedTarget;
            so.FindProperty("ftueText").objectReferenceValue = ftueText;
            so.FindProperty("dailyRewardText").objectReferenceValue = dailyText;
            so.FindProperty("resultText").objectReferenceValue = resultText;
            so.FindProperty("accentBar").objectReferenceValue = accentBar;
            so.FindProperty("resultAnimator").objectReferenceValue = animator;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireFtueOverlay(FtueOverlayPresenter presenter, GameObject root, Text title, Text body, Text hint)
        {
            SerializedObject so = new SerializedObject(presenter);
            so.FindProperty("overlayRoot").objectReferenceValue = root;
            so.FindProperty("titleText").objectReferenceValue = title;
            so.FindProperty("bodyText").objectReferenceValue = body;
            so.FindProperty("blockerHintText").objectReferenceValue = hint;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireDebugMenu(
            DebugMenu debugMenu,
            BuildingSystem buildingSystem,
            WheelSystem wheelSystem,
            GameplayController gameplayController,
            SaveSystem saveSystem,
            Text debugOutput)
        {
            SerializedObject so = new SerializedObject(debugMenu);
            so.FindProperty("buildingSystem").objectReferenceValue = buildingSystem;
            so.FindProperty("wheelSystem").objectReferenceValue = wheelSystem;
            so.FindProperty("gameplayController").objectReferenceValue = gameplayController;
            so.FindProperty("saveSystem").objectReferenceValue = saveSystem;
            so.FindProperty("debugOutput").objectReferenceValue = debugOutput;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetSpinButtonListener(Button spinButton, GameplayController gameplayController)
        {
            if (spinButton == null || gameplayController == null)
            {
                return;
            }

            spinButton.onClick.RemoveAllListeners();
            UnityEventTools.AddPersistentListener(spinButton.onClick, gameplayController.SpinWheel);
            EditorUtility.SetDirty(spinButton);
        }
    }
}
#endif
