using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.CameraSet.PostProcessing
{
    public class VisualEffectsAnimator : Singleton<VisualEffectsAnimator>
    {
        #region Class Members
        private IDictionary<VisualEffectsSheet, Coroutine> coroutines;
        #endregion

        protected override void Awake() {
            base.Awake();
            this.coroutines = new Dictionary<VisualEffectsSheet, Coroutine>();
        }

        /// <summary>
        /// Revert the effects of a sheet back to their original values.
        /// </summary>
        /// <param name="sheet">The sheet to animate</param>
        /// <param name="time">The time it takes the animation to finish [s]</param>
        public void Revert(VisualEffectsSheet sheet, float? time = null) {
            Animate(sheet, time, 0);
        }

        /// <summary>
        /// Animate all effects on a sheet.
        /// </summary>
        /// <param name="sheet">The sheet to animate</param>
        /// <param name="time">The time it takes the animation to finish [s]</param>
        /// <param name="percent">
        /// The percentage of the change [0:1],
        /// where 0 means the effects' original float value (from when the sheet was created),
        /// and 1 means the effects' configured target float value (from the given instructions).
        /// </param>
        public void Animate(VisualEffectsSheet sheet, float? time = null, float percent = 1) {
            Stop(sheet);
            coroutines[sheet] = StartCoroutine(sheet.Execute(time, percent));
        }

        /// <summary>
        /// Check if a specific sheet is now animating.
        /// </summary>
        /// <param name="sheet">The sheet to check</param>
        /// <returns>True if the specified sheet is now animating.</returns>
        public bool IsAnimating(VisualEffectsSheet sheet) {
            bool exists = coroutines.TryGetValue(sheet, out Coroutine coroutine);
            return exists && coroutine != null;
        }

        /// <summary>
        /// Stop the animation of a sheet.
        /// This method does nothing if the sheet is not already being animated.
        /// </summary>
        /// <param name="sheet">The sheet whose animation to stop</param>
        public void Stop(VisualEffectsSheet sheet) {
            bool exists = coroutines.TryGetValue(sheet, out Coroutine coroutine);
            if (exists) StopCoroutine(coroutine);
        }
    }
}