using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneHeist
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private BuildingSystem buildingSystem;
        [SerializeField] private AttackSystem attackSystem;
        [SerializeField] private WheelSystem wheelSystem;
        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private FtueSystem ftueSystem;
        [SerializeField] private DailyRewardSystem dailyRewardSystem;
        [SerializeField] private AnalyticsSystem analyticsSystem;
        [SerializeField] private GameplayHudPresenter hudPresenter;

        [Header("UI")]
        [SerializeField] private Text resultText;
        [SerializeField] private Text spinText;
        [SerializeField] private RectTransform targetListRoot;
        [SerializeField] private Button targetButtonPrefab;

        private void Start()
        {
            if (buildingSystem == null) buildingSystem = FindObjectOfType<BuildingSystem>();
            if (attackSystem == null) attackSystem = FindObjectOfType<AttackSystem>();
            if (wheelSystem == null) wheelSystem = FindObjectOfType<WheelSystem>();
            if (saveSystem == null) saveSystem = FindObjectOfType<SaveSystem>();
            if (ftueSystem == null) ftueSystem = FindObjectOfType<FtueSystem>();
            if (dailyRewardSystem == null) dailyRewardSystem = FindObjectOfType<DailyRewardSystem>();
            if (analyticsSystem == null) analyticsSystem = FindObjectOfType<AnalyticsSystem>();
            if (hudPresenter == null) hudPresenter = FindObjectOfType<GameplayHudPresenter>();

            buildingSystem.OnBuildingUpgraded += HandleBuildingUpgraded;

            LoadProgress();
            TryClaimDailyRewardOnStart();
            BuildTargetUI();
            RefreshUiCounters();
            Track("session_start", new Dictionary<string, object> { { "gold", buildingSystem.PlayerGold }, { "spins", wheelSystem.Spins } });
        }

        private void OnDestroy()
        {
            if (buildingSystem != null)
            {
                buildingSystem.OnBuildingUpgraded -= HandleBuildingUpgraded;
            }
        }

        public void SpinWheel()
        {
            WheelOutcome outcome = wheelSystem.Spin();
            TargetBaseData target = attackSystem.GetTargetOrRandom();
            int goldDelta = 0;

            switch (outcome)
            {
                case WheelOutcome.Gold:
                    goldDelta = wheelSystem.GetBaseGoldReward() + buildingSystem.GetTotalRewardBonus();
                    buildingSystem.AddGold(goldDelta);
                    SetResult($"Gold outcome! +{goldDelta} gold.");
                    break;
                case WheelOutcome.Attack:
                    goldDelta = attackSystem.ResolveAttack(target);
                    if (target != null && target.HasShield)
                    {
                        SetResult($"Attack blocked by {target.Name} shield.");
                    }
                    else
                    {
                        buildingSystem.AddGold(goldDelta);
                        SetResult($"Attack success on {target?.Name}. +{goldDelta} gold and enemy building downgraded.");
                    }
                    break;
                case WheelOutcome.Rob:
                    goldDelta = attackSystem.ResolveRob(target, wheelSystem.HeistStreak);
                    buildingSystem.AddGold(goldDelta);
                    SetResult($"Rob success on {target?.Name}! Stole {goldDelta} gold with streak x{wheelSystem.HeistStreak}.");
                    break;
                case WheelOutcome.Shield:
                    SetResult("Shield outcome. Next incoming attack can be blocked (placeholder logic).");
                    break;
                default:
                    SetResult("No spins available.");
                    break;
            }

            ftueSystem?.OnSpinDone();
            Track("spin_result", new Dictionary<string, object>
            {
                { "outcome", outcome.ToString() },
                { "gold_delta", goldDelta },
                { "target", target == null ? "none" : target.Name },
                { "streak", wheelSystem.HeistStreak }
            });

            RefreshUiCounters();
            BuildTargetUI();
            SaveProgress();
        }

        public void SelectTarget(int index)
        {
            attackSystem.SelectTarget(index);
            ftueSystem?.OnTargetSelected();
            Track("target_selected", new Dictionary<string, object> { { "index", index }, { "name", attackSystem.SelectedTarget?.Name } });
            BuildTargetUI();
            RefreshUiCounters();
            SaveProgress();
        }

        public void SaveProgress()
        {
            if (saveSystem == null || buildingSystem == null || wheelSystem == null || attackSystem == null)
            {
                return;
            }

            GameStateData state = new GameStateData
            {
                PlayerGold = buildingSystem.PlayerGold,
                Spins = wheelSystem.Spins,
                HeistStreak = wheelSystem.HeistStreak,
                SelectedTargetIndex = attackSystem.SelectedTargetIndex,
                PlayerBuildings = buildingSystem.ExportBuildingStates(),
                Targets = attackSystem.ExportTargetStates(),
                FtueStep = ftueSystem == null ? 0 : ftueSystem.CurrentStep,
                FtueCompleted = ftueSystem != null && ftueSystem.IsCompleted,
                LastDailyRewardDate = dailyRewardSystem == null ? null : dailyRewardSystem.LastClaimDate,
                DailyRewardStreak = dailyRewardSystem == null ? 0 : dailyRewardSystem.Streak,
            };

            saveSystem.Save(state);
        }

        public void LoadProgress()
        {
            if (saveSystem == null || !saveSystem.HasSave())
            {
                return;
            }

            GameStateData state = saveSystem.Load();
            if (state == null)
            {
                return;
            }

            buildingSystem.SetGold(state.PlayerGold);
            buildingSystem.ImportBuildingStates(state.PlayerBuildings);
            wheelSystem.SetSpins(state.Spins);
            wheelSystem.SetHeistStreak(state.HeistStreak);
            attackSystem.ImportTargetStates(state.Targets, state.SelectedTargetIndex);
            ftueSystem?.Restore(state.FtueStep, state.FtueCompleted);
            dailyRewardSystem?.Restore(state.LastDailyRewardDate, state.DailyRewardStreak);

            SetResult($"Progress loaded (save unix: {state.LastSaveUnix}).");
        }

        private void TryClaimDailyRewardOnStart()
        {
            if (dailyRewardSystem == null || !dailyRewardSystem.CanClaimToday())
            {
                return;
            }

            int reward = dailyRewardSystem.ClaimToday();
            if (reward > 0)
            {
                buildingSystem.AddGold(reward);
                SetResult($"Daily reward claimed: +{reward} gold.");
                Track("daily_reward_claimed", new Dictionary<string, object>
                {
                    { "reward", reward },
                    { "streak", dailyRewardSystem.Streak }
                });
                SaveProgress();
            }
        }

        private void HandleBuildingUpgraded(BuildingData building, int cost)
        {
            ftueSystem?.OnBuildingUpgraded();
            Track("building_upgrade", new Dictionary<string, object>
            {
                { "name", building.Name },
                { "level", building.Level },
                { "cost", cost }
            });
            SaveProgress();
            RefreshUiCounters();
        }

        private void BuildTargetUI()
        {
            if (targetListRoot == null || targetButtonPrefab == null || attackSystem == null)
            {
                return;
            }

            for (int i = targetListRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(targetListRoot.GetChild(i).gameObject);
            }

            for (int i = 0; i < attackSystem.Targets.Count; i++)
            {
                int index = i;
                TargetBaseData target = attackSystem.Targets[i];
                Button row = Instantiate(targetButtonPrefab, targetListRoot);
                Text label = row.GetComponentInChildren<Text>();
                if (label != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(target.Name)
                        .Append(" | Base Lv ").Append(target.BaseLevel)
                        .Append(" | Gold ").Append(target.GoldPool)
                        .Append(target.HasShield ? " | SHIELDED" : "");

                    if (attackSystem.SelectedTarget == target)
                    {
                        sb.Append(" | SELECTED");
                    }

                    label.text = sb.ToString();
                }

                row.onClick.AddListener(() => SelectTarget(index));
            }
        }

        private void RefreshUiCounters()
        {
            if (spinText != null && wheelSystem != null)
            {
                spinText.text = $"Spins: {wheelSystem.Spins} | Heist x{wheelSystem.HeistStreak}";
            }

            if (buildingSystem != null)
            {
                buildingSystem.RefreshUI();
            }

            if (hudPresenter != null)
            {
                string targetName = attackSystem?.SelectedTarget == null ? "None" : attackSystem.SelectedTarget.Name;
                string ftueInstruction = ftueSystem == null ? "FTUE off" : ftueSystem.CurrentInstruction;
                int dailyStreak = dailyRewardSystem == null ? 0 : dailyRewardSystem.Streak;
                hudPresenter.Refresh(buildingSystem.PlayerGold, wheelSystem.Spins, wheelSystem.HeistStreak, targetName, ftueInstruction, dailyStreak);
            }
        }

        private void SetResult(string text)
        {
            if (resultText != null)
            {
                resultText.text = text;
            }
        }

        private void Track(string eventName, Dictionary<string, object> parameters = null)
        {
            if (analyticsSystem != null)
            {
                analyticsSystem.Track(eventName, parameters);
            }
        }
    }
}
