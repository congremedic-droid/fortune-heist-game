using UnityEngine;
using UnityEngine.UI;

namespace FortuneHeist
{
    public class GameplayHudPresenter : MonoBehaviour
    {
        [SerializeField] private Text goldText;
        [SerializeField] private Text spinsText;
        [SerializeField] private Text selectedTargetText;
        [SerializeField] private Text ftueText;
        [SerializeField] private Text dailyRewardText;

        public void Refresh(int gold, int spins, int heistStreak, string selectedTarget, string ftueInstruction, int dailyStreak)
        {
            if (goldText != null) goldText.text = $"Gold: {gold}";
            if (spinsText != null) spinsText.text = $"Spins: {spins} | Heist x{heistStreak}";
            if (selectedTargetText != null) selectedTargetText.text = $"Target: {selectedTarget}";
            if (ftueText != null) ftueText.text = ftueInstruction;
            if (dailyRewardText != null) dailyRewardText.text = $"Daily streak: {dailyStreak}";
        }
    }
}
