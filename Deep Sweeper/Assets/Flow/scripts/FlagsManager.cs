using DeepSweeper.Level.Mine;
using UnityEngine.Events;

namespace DeepSweeper.Flow
{
    public class FlagsManager : Singleton<FlagsManager>
    {
        #region Events
        public event UnityAction<bool> FlagTakenEvent;
        public event UnityAction<bool> FlagReturnedEvent;
        public event UnityAction FlagsAmountUpdateEvent;
        #endregion

        #region Properties
        public int MaxFlags { get; private set; }
        public int AvailableFlags { get; private set; }
        #endregion

        private void Awake() {
            LevelFlow.Instance.PhaseUpdatedEvent += delegate (PhaseConfig pahseConfig, PhaseDifficultyConfig diffConfig, int index) {
                CollectFieldInfo();
            };
        }

        /// <summary>
        /// Collect and update the UI info according to the current phase.
        /// </summary>
        private void CollectFieldInfo() {
            MineField field = LevelFlow.Instance.CurrentPhase.Field;
            MaxFlags = field.MinesAmount;
            ResetGauge();
        }

        /// <summary>
        /// Reset the flags gauge and make all taken flags available again.
        /// </summary>
        public void ResetGauge() {
            AvailableFlags = MaxFlags;
            FlagsAmountUpdateEvent?.Invoke();
        }

        /// <summary>
        /// Take a flag in order to use it on a mine.
        /// </summary>
        /// <returns>True if there are enough available flags.</returns>
        public bool TakeFlag() {
            bool success;

            if (AvailableFlags > 0) {
                AvailableFlags--;
                success = true;
            }
            else success = false;

            FlagTakenEvent?.Invoke(success);
            return success;
        }

        /// <summary>
        /// Return a flag to the pile.
        /// </summary>
        /// <returns>True if the pile was not full before returning the flag.</returns>
        public bool ReturnFlag() {
            bool success;

            if (AvailableFlags < MaxFlags) {
                AvailableFlags++;
                success = true;
            }
            else success = false;

            FlagReturnedEvent?.Invoke(success);
            return success;
        }
    }
}