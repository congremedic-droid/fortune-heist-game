using System.Collections;
using UnityEngine;

namespace FortuneHeist
{
    public class UIFeedbackAnimator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup targetGroup;
        [SerializeField] private float pulseDuration = 0.2f;

        private Coroutine pulseRoutine;

        public void SetPulseDuration(float value)
        {
            pulseDuration = Mathf.Clamp(value, 0.08f, 1.25f);
        }

        public void PlayPulse()
        {
            if (targetGroup == null)
            {
                return;
            }

            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
            }

            pulseRoutine = StartCoroutine(Pulse());
        }

        private IEnumerator Pulse()
        {
            float timer = 0f;
            while (timer < pulseDuration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / pulseDuration);
                targetGroup.alpha = Mathf.Lerp(0.4f, 1f, t);
                yield return null;
            }

            targetGroup.alpha = 1f;
            pulseRoutine = null;
        }
    }
}
