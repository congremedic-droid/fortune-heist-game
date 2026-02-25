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

            BuildTargetUI();
            RefreshUiCounters();
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
                    SetResult("Shield outcome. Next incoming attack can be blocked (placeholder logic). ");
                    break;
                default:
                    SetResult("No spins available.");
                    break;
            }

            RefreshUiCounters();
            BuildTargetUI();
        }

        public void SelectTarget(int index)
        {
            attackSystem.SelectTarget(index);
            BuildTargetUI();
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
        }

        private void SetResult(string text)
        {
            if (resultText != null)
            {
                resultText.text = text;
            }
        }
    }
}
