using System;
using System.Collections.Generic;

namespace FortuneHeist
{
    [Serializable]
    public class BuildingState
    {
        public string Name;
        public int Level;
        public int BaseUpgradeCost;
        public int RewardBonus;
    }

    [Serializable]
    public class TargetState
    {
        public string Name;
        public int BaseLevel;
        public int GoldPool;
        public bool HasShield;
        public List<BuildingState> Buildings = new List<BuildingState>();
    }

    [Serializable]
    public class GameStateData
    {
        public int PlayerGold;
        public int Spins;
        public int HeistStreak;
        public int SelectedTargetIndex;
        public List<BuildingState> PlayerBuildings = new List<BuildingState>();
        public List<TargetState> Targets = new List<TargetState>();
        public long LastSaveUnix;
    }
}
