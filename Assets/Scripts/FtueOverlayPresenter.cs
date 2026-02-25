using UnityEngine;
using UnityEngine.UI;

namespace FortuneHeist
{
    public class FtueOverlayPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject overlayRoot;
        [SerializeField] private Text titleText;
        [SerializeField] private Text bodyText;
        [SerializeField] private Text blockerHintText;

        public void Refresh(FtueSystem ftue)
        {
            if (ftue == null)
            {
                return;
            }

            bool show = !ftue.IsCompleted;
            if (overlayRoot != null)
            {
                overlayRoot.SetActive(show);
            }

            if (!show)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = $"Tutorial {ftue.CurrentStep + 1}/3";
            }

            if (bodyText != null)
            {
                bodyText.text = ftue.CurrentInstruction;
            }
        }

        public void ShowBlockedHint(string message)
        {
            if (blockerHintText != null)
            {
                blockerHintText.text = message;
            }
        }
    }
}
