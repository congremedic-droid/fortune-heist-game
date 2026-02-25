using System;
using System.Collections.Generic;

namespace FortuneHeist
{
    [Serializable]
    public class TargetBaseData
    {
        public string Name;
        public int BaseLevel;
        public int GoldPool;
        public bool HasShield;
        public List<BuildingData> Buildings = new List<BuildingData>();

        public int GetHighestBuildingLevel()
        {
            int highest = 0;
            for (int i = 0; i < Buildings.Count; i++)
            {
                if (Buildings[i].Level > highest)
                {
                    highest = Buildings[i].Level;
                }
            }

            return highest;
        }

        public bool DowngradeOneBuildingLevel()
        {
            for (int i = 0; i < Buildings.Count; i++)
            {
                if (Buildings[i].Level > 0)
                {
                    Buildings[i].Level -= 1;
                    return true;
                }
            }

            return false;
        }
    }
}
