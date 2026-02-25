using System.Collections.Generic;
using UnityEngine;

namespace FortuneHeist
{
    public class AttackSystem : MonoBehaviour
    {
        [SerializeField] private List<TargetBaseData> targets = new List<TargetBaseData>();
        [SerializeField] private int attackGoldBonus = 60;
        [SerializeField] private RemoteConfigService remoteConfigService;

        public IReadOnlyList<TargetBaseData> Targets => targets;
        public TargetBaseData SelectedTarget { get; private set; }
        public int SelectedTargetIndex => SelectedTarget == null ? -1 : targets.IndexOf(SelectedTarget);

        private void Awake()
        {
            if (remoteConfigService == null) remoteConfigService = FindObjectOfType<RemoteConfigService>();
            EnsureDefaultTargets();
            if (SelectedTarget == null && targets.Count > 0)
            {
                SelectedTarget = targets[0];
            }
        }

        private void EnsureDefaultTargets()
        {
            if (targets.Count > 0)
            {
                return;
            }

            targets.Add(CreateTarget("Harbor Crew", 2, 300, false));
            targets.Add(CreateTarget("Neon Syndicate", 4, 550, true));
            targets.Add(CreateTarget("Vault Runners", 3, 420, false));
            targets.Add(CreateTarget("Quantum Foxes", 5, 700, false));
        }

        public void SelectTarget(int index)
        {
            if (index < 0 || index >= targets.Count)
            {
                return;
            }

            SelectedTarget = targets[index];
        }

        public TargetBaseData GetTargetOrRandom()
        {
            if (SelectedTarget != null)
            {
                return SelectedTarget;
            }

            if (targets.Count == 0)
            {
                return null;
            }

            return targets[Random.Range(0, targets.Count)];
        }

        public int ResolveAttack(TargetBaseData target)
        {
            if (target == null)
            {
                return 0;
            }

            if (target.HasShield)
            {
                return 0;
            }

            target.DowngradeOneBuildingLevel();
            int configured = remoteConfigService == null ? attackGoldBonus : remoteConfigService.CurrentConfig.AttackGoldBonus;
            return configured > 0 ? configured : attackGoldBonus;
        }

        public int ResolveRob(TargetBaseData target, int heistStreak)
        {
            if (target == null)
            {
                return 0;
            }

            BalanceConfigData cfg = remoteConfigService == null ? BalanceConfigData.Default() : remoteConfigService.CurrentConfig;
            int min = Mathf.Max(10, target.GoldPool * Mathf.Max(1, cfg.RobMinPercent) / 100);
            int max = Mathf.Max(min + 1, target.GoldPool * Mathf.Max(cfg.RobMinPercent + 1, cfg.RobMaxPercent) / 100);
            int stolen = Random.Range(min, max);
            int cap = remoteConfigService == null ? 5 : Mathf.Max(1, remoteConfigService.CurrentConfig.HeistStreakCap);
            float multiplier = Mathf.Clamp(heistStreak, 1, cap);
            int finalStolen = Mathf.RoundToInt(stolen * multiplier);

            target.GoldPool = Mathf.Max(0, target.GoldPool - finalStolen);
            return finalStolen;
        }

        public List<TargetState> ExportTargetStates()
        {
            List<TargetState> states = new List<TargetState>();
            for (int i = 0; i < targets.Count; i++)
            {
                TargetBaseData source = targets[i];
                TargetState state = new TargetState
                {
                    Name = source.Name,
                    BaseLevel = source.BaseLevel,
                    GoldPool = source.GoldPool,
                    HasShield = source.HasShield,
                    Buildings = new List<BuildingState>()
                };

                for (int b = 0; b < source.Buildings.Count; b++)
                {
                    BuildingData building = source.Buildings[b];
                    state.Buildings.Add(new BuildingState
                    {
                        Name = building.Name,
                        Level = building.Level,
                        BaseUpgradeCost = building.BaseUpgradeCost,
                        RewardBonus = building.RewardBonus,
                    });
                }

                states.Add(state);
            }

            return states;
        }

        public void ImportTargetStates(List<TargetState> states, int selectedIndex)
        {
            if (states == null || states.Count == 0)
            {
                return;
            }

            targets.Clear();
            for (int i = 0; i < states.Count; i++)
            {
                TargetState source = states[i];
                TargetBaseData target = new TargetBaseData
                {
                    Name = source.Name,
                    BaseLevel = source.BaseLevel,
                    GoldPool = source.GoldPool,
                    HasShield = source.HasShield,
                    Buildings = new List<BuildingData>()
                };

                for (int b = 0; b < source.Buildings.Count; b++)
                {
                    BuildingState building = source.Buildings[b];
                    target.Buildings.Add(new BuildingData
                    {
                        Name = building.Name,
                        Level = building.Level,
                        BaseUpgradeCost = building.BaseUpgradeCost,
                        RewardBonus = building.RewardBonus,
                    });
                }

                targets.Add(target);
            }

            if (selectedIndex >= 0 && selectedIndex < targets.Count)
            {
                SelectedTarget = targets[selectedIndex];
            }
            else
            {
                SelectedTarget = targets.Count > 0 ? targets[0] : null;
            }
        }

        private TargetBaseData CreateTarget(string name, int baseLevel, int gold, bool hasShield)
        {
            TargetBaseData data = new TargetBaseData
            {
                Name = name,
                BaseLevel = baseLevel,
                GoldPool = gold,
                HasShield = hasShield,
                Buildings = new List<BuildingData>
                {
                    new BuildingData { Name = "Vault", Level = baseLevel, BaseUpgradeCost = 100, RewardBonus = baseLevel * 8 },
                    new BuildingData { Name = "Walls", Level = Mathf.Max(1, baseLevel - 1), BaseUpgradeCost = 80, RewardBonus = baseLevel * 5 },
                }
            };

            return data;
        }
    }
}
