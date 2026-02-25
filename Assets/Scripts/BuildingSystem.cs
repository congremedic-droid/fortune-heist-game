using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneHeist
{
    public class BuildingSystem : MonoBehaviour
    {
        [SerializeField] private List<BuildingData> buildings = new List<BuildingData>();
        [SerializeField] private RectTransform buildingListRoot;
        [SerializeField] private Button buildingRowButtonPrefab;
        [SerializeField] private Text goldText;
        [SerializeField] private GameplayController gameplayController;
        [SerializeField] private RemoteConfigService remoteConfigService;

        public int PlayerGold { get; private set; } = 500;
        public IReadOnlyList<BuildingData> Buildings => buildings;

        public event Action OnBuildingChanged;
        public event Action<BuildingData, int> OnBuildingUpgraded;

        private void Awake()
        {
            EnsureDefaultBuildings();
            if (gameplayController == null) gameplayController = FindObjectOfType<GameplayController>();
            if (remoteConfigService == null) remoteConfigService = FindObjectOfType<RemoteConfigService>();
            RefreshUI();
        }

        private void EnsureDefaultBuildings()
        {
            if (buildings.Count > 0)
            {
                return;
            }

            buildings.Add(new BuildingData { Name = "Vault", Level = 1, BaseUpgradeCost = 100, RewardBonus = 10 });
            buildings.Add(new BuildingData { Name = "Security Hub", Level = 1, BaseUpgradeCost = 120, RewardBonus = 5 });
            buildings.Add(new BuildingData { Name = "Crypto Lab", Level = 0, BaseUpgradeCost = 140, RewardBonus = 8 });
        }

        public void AddGold(int amount)
        {
            PlayerGold = Mathf.Max(0, PlayerGold + amount);
            RefreshUI();
        }

        public void SetGold(int amount)
        {
            PlayerGold = Mathf.Max(0, amount);
            RefreshUI();
        }

        public bool TryUpgradeBuilding(int index)
        {
            if (index < 0 || index >= buildings.Count)
            {
                return false;
            }

            if (gameplayController != null && !gameplayController.CanUpgradeByFtue())
            {
                gameplayController.NotifyUpgradeBlockedByFtue();
                return false;
            }

            BuildingData data = buildings[index];
            int cost = GetUpgradeCost(data);
            if (PlayerGold < cost)
            {
                return false;
            }

            PlayerGold -= cost;
            data.Upgrade();
            OnBuildingUpgraded?.Invoke(data, cost);
            RefreshUI();
            return true;
        }

        public int GetTotalRewardBonus()
        {
            int total = 0;
            for (int i = 0; i < buildings.Count; i++)
            {
                total += buildings[i].RewardBonus;
            }

            return total;
        }

        public void ResetBuildingLevels()
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                buildings[i].Level = 0;
                buildings[i].RewardBonus = 0;
            }

            RefreshUI();
        }

        public string DumpBuildingLevels()
        {
            List<string> lines = new List<string>();
            for (int i = 0; i < buildings.Count; i++)
            {
                lines.Add($"{buildings[i].Name}: L{buildings[i].Level}, Bonus {buildings[i].RewardBonus}");
            }

            return string.Join(" | ", lines);
        }

        public List<BuildingState> ExportBuildingStates()
        {
            List<BuildingState> states = new List<BuildingState>();
            for (int i = 0; i < buildings.Count; i++)
            {
                BuildingData source = buildings[i];
                states.Add(new BuildingState
                {
                    Name = source.Name,
                    Level = source.Level,
                    BaseUpgradeCost = source.BaseUpgradeCost,
                    RewardBonus = source.RewardBonus,
                });
            }

            return states;
        }

        public void ImportBuildingStates(List<BuildingState> states)
        {
            if (states == null || states.Count == 0)
            {
                return;
            }

            buildings.Clear();
            for (int i = 0; i < states.Count; i++)
            {
                BuildingState state = states[i];
                buildings.Add(new BuildingData
                {
                    Name = state.Name,
                    Level = state.Level,
                    BaseUpgradeCost = state.BaseUpgradeCost,
                    RewardBonus = state.RewardBonus,
                });
            }

            RefreshUI();
        }


        private int GetUpgradeCost(BuildingData building)
        {
            float multiplier = remoteConfigService == null ? 1f : Mathf.Max(0.5f, remoteConfigService.CurrentConfig.BuildingUpgradeCostMultiplier);
            return Mathf.RoundToInt(building.GetUpgradeCost() * multiplier);
        }

        public void RefreshUI()
        {
            if (goldText != null)
            {
                goldText.text = $"Gold: {PlayerGold}";
            }

            if (buildingListRoot != null && buildingRowButtonPrefab != null)
            {
                for (int i = buildingListRoot.childCount - 1; i >= 0; i--)
                {
                    Destroy(buildingListRoot.GetChild(i).gameObject);
                }

                for (int i = 0; i < buildings.Count; i++)
                {
                    int index = i;
                    BuildingData building = buildings[i];
                    Button row = Instantiate(buildingRowButtonPrefab, buildingListRoot);
                    Text text = row.GetComponentInChildren<Text>();
                    int cost = GetUpgradeCost(building);
                    if (text != null)
                    {
                        text.text = $"{building.Name} | Lv {building.Level} | Cost {cost} | Bonus {building.RewardBonus}";
                    }

                    row.interactable = PlayerGold >= cost;
                    row.onClick.AddListener(() => TryUpgradeBuilding(index));
                }
            }

            OnBuildingChanged?.Invoke();
        }
    }
}
