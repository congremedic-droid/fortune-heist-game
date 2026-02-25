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

        public int HeistStreak { get; private set; } = 1;
        public int Spins => spins;

        public event Action<WheelOutcome> OnOutcome;

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
            HeistStreak = Mathf.Clamp(value, 1, 5);
        }

        public WheelOutcome Spin()
        {
            if (spins <= 0)
            {
                return WheelOutcome.None;
            }

            spins -= 1;
            int roll = UnityEngine.Random.Range(0, 100);

            WheelOutcome outcome;
            if (roll < 40)
            {
                outcome = WheelOutcome.Gold;
                HeistStreak = Mathf.Min(HeistStreak + 1, 5);
            }
            else if (roll < 65)
            {
                outcome = WheelOutcome.Attack;
                HeistStreak = 1;
            }
            else if (roll < 90)
            {
                outcome = WheelOutcome.Rob;
                HeistStreak = Mathf.Min(HeistStreak + 1, 5);
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
            return baseGoldReward;
        }
    }
}
