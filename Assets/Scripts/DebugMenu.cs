using UnityEngine;
using UnityEngine.UI;

namespace FortuneHeist
{
    public class DebugMenu : MonoBehaviour
    {
        [SerializeField] private BuildingSystem buildingSystem;
        [SerializeField] private WheelSystem wheelSystem;
        [SerializeField] private GameplayController gameplayController;
        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private Text debugOutput;

        private void Start()
        {
            if (buildingSystem == null) buildingSystem = FindObjectOfType<BuildingSystem>();
            if (wheelSystem == null) wheelSystem = FindObjectOfType<WheelSystem>();
            if (gameplayController == null) gameplayController = FindObjectOfType<GameplayController>();
            if (saveSystem == null) saveSystem = FindObjectOfType<SaveSystem>();
            UpdateDebugText();
        }

        public void AddGold1000()
        {
            buildingSystem.AddGold(1000);
            gameplayController.SaveProgress();
            UpdateDebugText();
        }

        public void AddSpins5()
        {
            wheelSystem.AddSpins(5);
            gameplayController.SaveProgress();
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
            gameplayController.SaveProgress();
            UpdateDebugText();
        }

        public void ClearProgress()
        {
            if (saveSystem != null)
            {
                saveSystem.ClearSave();
            }

            if (debugOutput != null)
            {
                debugOutput.text = "Save file cleared.";
            }
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
