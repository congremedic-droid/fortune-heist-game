using System;
using System.IO;
using UnityEngine;

namespace FortuneHeist
{
    public class ThemeConfigService : MonoBehaviour
    {
        [SerializeField] private string localThemeFileName = "theme_config.json";

        public ThemeConfigData CurrentTheme { get; private set; } = ThemeConfigData.Default();
        public event Action<ThemeConfigData> OnThemeUpdated;

        private void Awake()
        {
            LoadLocalTheme();
        }

        public void LoadLocalTheme()
        {
            string path = Path.Combine(Application.streamingAssetsPath, localThemeFileName);
            if (!File.Exists(path))
            {
                CurrentTheme = ThemeConfigData.Default();
                OnThemeUpdated?.Invoke(CurrentTheme);
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                ThemeConfigData parsed = JsonUtility.FromJson<ThemeConfigData>(json);
                CurrentTheme = parsed ?? ThemeConfigData.Default();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ThemeConfigService] Failed to read theme: {ex.Message}");
                CurrentTheme = ThemeConfigData.Default();
            }

            OnThemeUpdated?.Invoke(CurrentTheme);
        }
    }
}
