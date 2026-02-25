using System.Collections.Generic;
using UnityEngine;

namespace FortuneHeist
{
    public class AttackSystem : MonoBehaviour
    {
        [SerializeField] private List<TargetBaseData> targets = new List<TargetBaseData>();
        [SerializeField] private int attackGoldBonus = 60;

        public IReadOnlyList<TargetBaseData> Targets => targets;
        public TargetBaseData SelectedTarget { get; private set; }

        private void Awake()
        {
            if (targets.Count == 0)
            {
                targets.Add(CreateTarget("Harbor Crew", 2, 300, false));
                targets.Add(CreateTarget("Neon Syndicate", 4, 550, true));
                targets.Add(CreateTarget("Vault Runners", 3, 420, false));
                targets.Add(CreateTarget("Quantum Foxes", 5, 700, false));
            }

            if (SelectedTarget == null && targets.Count > 0)
            {
                SelectedTarget = targets[0];
            }
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
            return attackGoldBonus;
        }

        public int ResolveRob(TargetBaseData target, int heistStreak)
        {
            if (target == null)
            {
                return 0;
            }

            int min = Mathf.Max(10, target.GoldPool / 10);
            int max = Mathf.Max(min + 1, target.GoldPool / 3);
            int stolen = Random.Range(min, max);
            float multiplier = Mathf.Clamp(heistStreak, 1, 5);
            int finalStolen = Mathf.RoundToInt(stolen * multiplier);

            target.GoldPool = Mathf.Max(0, target.GoldPool - finalStolen);
            return finalStolen;
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
