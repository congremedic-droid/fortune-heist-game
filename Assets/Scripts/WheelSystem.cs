using System;
using UnityEngine;

namespace FortuneHeist
{
    public enum WheelOutcome
    {
        None,
        Gold,
        Attack,
        Rob,
        Shield
    }

    public class WheelSystem : MonoBehaviour
    {
        [SerializeField] private int spins = 10;
        [SerializeField] private int baseGoldReward = 50;
        [SerializeField] private RemoteConfigService remoteConfigService;

        public int HeistStreak { get; private set; } = 1;
        public int Spins => spins;

        public event Action<WheelOutcome> OnOutcome;

        private void Awake()
        {
            if (remoteConfigService == null)
            {
                remoteConfigService = FindObjectOfType<RemoteConfigService>();
            }
        }

        public void AddSpins(int amount)
        {
            spins = Mathf.Max(0, spins + amount);
        }

        public void SetSpins(int value)
        {
            spins = Mathf.Max(0, value);
        }

        public void SetHeistStreak(int value)
        {
            HeistStreak = Mathf.Clamp(value, 1, GetStreakCap());
        }

        public WheelOutcome Spin()
        {
            if (spins <= 0)
            {
                return WheelOutcome.None;
            }

            spins -= 1;
            int roll = UnityEngine.Random.Range(0, 100);

            BalanceConfigData cfg = GetConfig();
            int goldThreshold = Mathf.Clamp(cfg.GoldOutcomeChance, 0, 100);
            int attackThreshold = goldThreshold + Mathf.Clamp(cfg.AttackOutcomeChance, 0, 100);
            int robThreshold = attackThreshold + Mathf.Clamp(cfg.RobOutcomeChance, 0, 100);

            WheelOutcome outcome;
            if (roll < goldThreshold)
            {
                outcome = WheelOutcome.Gold;
                HeistStreak = Mathf.Min(HeistStreak + 1, GetStreakCap());
            }
            else if (roll < attackThreshold)
            {
                outcome = WheelOutcome.Attack;
                HeistStreak = 1;
            }
            else if (roll < robThreshold)
            {
                outcome = WheelOutcome.Rob;
                HeistStreak = Mathf.Min(HeistStreak + 1, GetStreakCap());
            }
            else
            {
                outcome = WheelOutcome.Shield;
                HeistStreak = 1;
            }

            OnOutcome?.Invoke(outcome);
            return outcome;
        }

        public int GetBaseGoldReward()
        {
            BalanceConfigData cfg = GetConfig();
            return cfg.BaseGoldReward > 0 ? cfg.BaseGoldReward : baseGoldReward;
        }

        private int GetStreakCap()
        {
            return Mathf.Max(1, GetConfig().HeistStreakCap);
        }

        private BalanceConfigData GetConfig()
        {
            return remoteConfigService == null ? BalanceConfigData.Default() : remoteConfigService.CurrentConfig;
        }
    }
}
