using UnityEngine;

namespace FortuneHeist
{
    public enum FtueAction
    {
        SelectTarget,
        SpinWheel,
        UpgradeBuilding
    }

    public class FtueSystem : MonoBehaviour
    {
        public bool IsEnabled { get; private set; } = true;
        public int CurrentStep { get; private set; }
        public bool IsCompleted { get; private set; }

        public string CurrentInstruction
        {
            get
            {
                if (IsCompleted)
                {
                    return "FTUE completed";
                }

                if (!IsEnabled)
                {
                    return "FTUE disabled";
                }

                if (CurrentStep == 0) return "Paso 1: selecciona un objetivo";
                if (CurrentStep == 1) return "Paso 2: gira la ruleta";
                if (CurrentStep == 2) return "Paso 3: mejora un edificio";
                return "Tutorial en progreso";
            }
        }

        public void Restore(int step, bool completed)
        {
            CurrentStep = Mathf.Max(0, step);
            IsCompleted = completed;
        }

        public void SetEnabled(bool enabled)
        {
            IsEnabled = enabled;
            if (!enabled)
            {
                CurrentStep = 0;
                IsCompleted = true;
                return;
            }

            if (IsCompleted)
            {
                IsCompleted = false;
            }
        }

        public bool IsActionAllowed(FtueAction action)
        {
            if (IsCompleted)
            {
                return true;
            }

            if (!IsEnabled)
            {
                return true;
            }

            if (CurrentStep == 0)
            {
                return action == FtueAction.SelectTarget;
            }

            if (CurrentStep == 1)
            {
                return action == FtueAction.SpinWheel || action == FtueAction.SelectTarget;
            }

            if (CurrentStep == 2)
            {
                return action == FtueAction.UpgradeBuilding || action == FtueAction.SelectTarget;
            }

            return true;
        }

        public void OnTargetSelected()
        {
            if (!IsCompleted && CurrentStep == 0)
            {
                CurrentStep = 1;
            }
        }

        public void OnSpinDone()
        {
            if (!IsCompleted && CurrentStep == 1)
            {
                CurrentStep = 2;
            }
        }

        public void OnBuildingUpgraded()
        {
            if (!IsCompleted && CurrentStep == 2)
            {
                IsCompleted = true;
            }
        }
    }
}
