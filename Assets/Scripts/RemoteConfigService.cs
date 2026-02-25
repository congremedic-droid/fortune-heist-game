using System;
using System.IO;
using UnityEngine;

namespace FortuneHeist
{
    public class RemoteConfigService : MonoBehaviour
    {
        [SerializeField] private string localConfigFileName = "remote_balance.json";

        public BalanceConfigData CurrentConfig { get; private set; } = BalanceConfigData.Default();
        public event Action<BalanceConfigData> OnConfigUpdated;

        private void Awake()
        {
            LoadLocalConfig();
        }

        public void LoadLocalConfig()
        {
            string path = Path.Combine(Application.streamingAssetsPath, localConfigFileName);
            if (!File.Exists(path))
            {
                CurrentConfig = BalanceConfigData.Default();
                OnConfigUpdated?.Invoke(CurrentConfig);
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                BalanceConfigData parsed = JsonUtility.FromJson<BalanceConfigData>(json);
                CurrentConfig = parsed ?? BalanceConfigData.Default();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[RemoteConfigService] Failed to read config: {ex.Message}");
                CurrentConfig = BalanceConfigData.Default();
            }

            OnConfigUpdated?.Invoke(CurrentConfig);
        }
    }
}
