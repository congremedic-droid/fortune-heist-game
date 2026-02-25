using System;

namespace FortuneHeist
{
    [Serializable]
    public class BuildingData
    {
        public string Name;
        public int Level;
        public int BaseUpgradeCost;
        public int RewardBonus;

        public int GetUpgradeCost()
        {
            return BaseUpgradeCost * Math.Max(1, Level + 1);
        }

        public int GetRewardBonusForNextLevel()
        {
            return RewardBonus + (Level * 5);
        }

        public void Upgrade()
        {
            Level += 1;
            RewardBonus = GetRewardBonusForNextLevel();
        }
    }
}
