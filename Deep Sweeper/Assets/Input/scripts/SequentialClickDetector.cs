using System.Threading.Tasks;
using UnityEngine.Events;

namespace DeepSweeper.Player.Input
{
    public class SequentialClickDetector
    {
        #region Class Members
        private int counter;
        private int maxClickDelay;
        #endregion

        #region Events
        public event UnityAction TargetSequenceEvent;
        #endregion

        #region Properties
        public int CounterGoal { get; private set; }
        #endregion

        /// <param name="targetSequence">The target amount of clicks that will invoke an event</param>
        /// <param name="maxClickDelay">The maximum time allowed between clicks [s]</param>
        public SequentialClickDetector(int targetSequence, float maxClickDelay) {
            this.CounterGoal = targetSequence;
            this.maxClickDelay = (int)(maxClickDelay * 1000);
            this.counter = 0;
        }

        /// <summary>
        /// Reset the counter back to 0.
        /// </summary>
        public void ResetCounter() { counter = 0; }

        /// <summary>
        /// Increase the counter by 1 and invoke the target event if needed.
        /// </summary>
        public void IncreaseCounter() {
            if (++counter == CounterGoal) {
                TargetSequenceEvent?.Invoke();
                counter = 0;
            }
            else Task.Delay(maxClickDelay).ContinueWith(x => ResetCounter());
        }
    }
}