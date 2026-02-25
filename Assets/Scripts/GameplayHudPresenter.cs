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
        [SerializeField] private Text resultText;
        [SerializeField] private Image accentBar;

        [SerializeField] private Color positiveColor = new Color(0.35f, 1f, 0.45f);
        [SerializeField] private Color neutralColor = Color.white;
        [SerializeField] private Color warningColor = new Color(1f, 0.6f, 0.3f);

        [SerializeField] private UIFeedbackAnimator resultAnimator;

        public void ApplyTheme(ThemeConfigData data)
        {
            if (data == null)
            {
                return;
            }

            positiveColor = ParseColor(data.PositiveHex, positiveColor);
            neutralColor = ParseColor(data.NeutralHex, neutralColor);
            warningColor = ParseColor(data.WarningHex, warningColor);

            if (accentBar != null)
            {
                accentBar.color = ParseColor(data.AccentHex, accentBar.color);
            }

            if (resultAnimator != null)
            {
                resultAnimator.SetPulseDuration(data.ResultPulseDuration);
            }
        }

        public void Refresh(int gold, int spins, int heistStreak, string selectedTarget, string ftueInstruction, int dailyStreak)
        {
            if (goldText != null) goldText.text = $"Gold: {gold}";
            if (spinsText != null) spinsText.text = $"Spins: {spins} | Heist x{heistStreak}";
            if (selectedTargetText != null) selectedTargetText.text = $"Target: {selectedTarget}";
            if (ftueText != null) ftueText.text = ftueInstruction;
            if (dailyRewardText != null) dailyRewardText.text = $"Daily streak: {dailyStreak}";
        }

        public void ShowResult(string text, bool positive, bool warning = false)
        {
            if (resultText == null)
            {
                return;
            }

            resultText.text = text;
            if (warning)
            {
                resultText.color = warningColor;
            }
            else
            {
                resultText.color = positive ? positiveColor : neutralColor;
            }

            resultAnimator?.PlayPulse();
        }

        private Color ParseColor(string hex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                return fallback;
            }

            return ColorUtility.TryParseHtmlString(hex, out Color parsed) ? parsed : fallback;
        }
    }
}
