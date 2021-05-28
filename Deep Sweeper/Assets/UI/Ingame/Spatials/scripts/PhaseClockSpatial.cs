using DeepSweeper.Flow;
using System.Collections;
using TMPro;
using UnityEngine;

namespace DeepSweeper.UI.Ingame
{
    public class PhaseClockSpatial : Spatial
    {
        #region ClassMembers
        private TextMeshProUGUI counter;
        private Coroutine countCoroutine;
        private bool paused;
        #endregion

        protected override void Awake() {
            base.Awake();

            this.counter = GetComponentInChildren<TextMeshProUGUI>();
            this.paused = false;
        }

        /// <summary>
        /// Set a fixed amount of seconds in the counter clock.
        /// </summary>
        /// <param name="seconds">The amount of seconds to set</param>
        private void SetSeconds(int seconds) {
            counter.text = seconds.ToString();
        }

        /// <summary>
        /// Activate the clock and start counting from 0.
        /// </summary>
        private IEnumerator Count() {
            int seconds = 0;

            while (true) {
                while (!paused) {
                    SetSeconds(seconds++);
                    yield return new WaitForSeconds(1);
                }

                yield return null;
            }
        }

        /// <inheritdoc/>
        public override void ResetValue(Phase phase) {
            SetSeconds(0);
        }

        /// <inheritdoc/>
        public override void OnPhaseStarts(Phase phase) {
            Activate(true);
            countCoroutine = StartCoroutine(Count());
        }

        /// <inheritdoc/>
        public override void OnPhasePauses(Phase phase) { paused = true; }

        /// <inheritdoc/>
        public override void OnPhaseResumes(Phase phase) { paused = false; }

        /// <inheritdoc/>
        public override void OnPhaseEnds(Phase phase, bool success) {
            if (countCoroutine != null) StopCoroutine(countCoroutine);
            Activate(false);
        }
    }
}