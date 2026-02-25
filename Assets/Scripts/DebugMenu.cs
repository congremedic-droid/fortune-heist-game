using UnityEngine;
using UnityEngine.UI;

namespace FortuneHeist
{
    public class DebugMenu : MonoBehaviour
    {
        [SerializeField] private BuildingSystem buildingSystem;
        [SerializeField] private WheelSystem wheelSystem;
        [SerializeField] private Text debugOutput;

        private void Start()
        {
            if (buildingSystem == null) buildingSystem = FindObjectOfType<BuildingSystem>();
            if (wheelSystem == null) wheelSystem = FindObjectOfType<WheelSystem>();
            UpdateDebugText();
        }

        public void AddGold1000()
        {
            buildingSystem.AddGold(1000);
            UpdateDebugText();
        }

        public void AddSpins5()
        {
            wheelSystem.AddSpins(5);
            UpdateDebugText();
        }

        public void ShowBuildingLevels()
        {
            if (debugOutput != null)
            {
                debugOutput.text = buildingSystem.DumpBuildingLevels();
            }
        }

        public void ResetBuildingLevels()
        {
            buildingSystem.ResetBuildingLevels();
            UpdateDebugText();
        }

        private void UpdateDebugText()
        {
            if (debugOutput != null)
            {
                debugOutput.text = $"Debug Ready | Gold {buildingSystem.PlayerGold} | Spins {wheelSystem.Spins}";
            }
        }
    }
}
