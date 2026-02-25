using System;
using UnityEngine;

namespace FortuneHeist
{
    public class DailyRewardSystem : MonoBehaviour
    {
        [SerializeField] private int baseReward = 100;

        public string LastClaimDate { get; private set; }
        public int Streak { get; private set; }

        public void Restore(string lastClaimDate, int streak)
        {
            LastClaimDate = lastClaimDate;
            Streak = Mathf.Max(0, streak);
        }

        public bool CanClaimToday()
        {
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            return LastClaimDate != today;
        }

        public int ClaimToday()
        {
            if (!CanClaimToday())
            {
                return 0;
            }

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (LastClaimDate == DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"))
            {
                Streak += 1;
            }
            else
            {
                Streak = 1;
            }

            LastClaimDate = today;
            return baseReward * Mathf.Clamp(Streak, 1, 7);
        }
    }
}
