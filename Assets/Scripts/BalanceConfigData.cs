using System;

namespace FortuneHeist
{
    [Serializable]
    public class BalanceConfigData
    {
        public int GoldOutcomeChance = 40;
        public int AttackOutcomeChance = 25;
        public int RobOutcomeChance = 25;

        public int BaseGoldReward = 50;
        public int AttackGoldBonus = 60;

        public int RobMinPercent = 10;
        public int RobMaxPercent = 33;
        public int HeistStreakCap = 5;

        public float BuildingUpgradeCostMultiplier = 1f;

        public static BalanceConfigData Default()
        {
            return new BalanceConfigData();
        }
    }
}
