using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace DeepSweeper.CameraSet.PostProcessing
{
    public class DynamicEffectValue
    {
        #region Class Members
        private FloatParameter parameter;
        #endregion

        #region Properties
        public float OriginValue { get; private set; }
        public float TargetValue { get; set; }
        public float LerpTime { get; set; }
        #endregion

        /// <summary>
        /// Create a new DynamicEffectValue instamce.
        /// </summary>
        /// <param name="parameter">The effect's parameter reference</param>
        /// <param name="time">The time it takes the effect to finish lerping towards the target value</param>
        /// <param name="targetValue">The effect's target float value</param>
        /// <returns>
        /// A new DynamicEffectValue instance,
        /// or null if the parameter is not valid.
        /// </returns>
        public static DynamicEffectValue Create(FloatParameter parameter, float time = 0, float targetValue = Mathf.Infinity) {
            return (parameter != null) ? new DynamicEffectValue(parameter, time, targetValue) : null;
        }

        /// <param name="parameter">The effect's parameter reference</param>
        /// <param name="time">The time it takes the effect to finish lerping towards the target value</param>
        /// <param name="targetValue">The effect's target float value</param>
        private DynamicEffectValue(FloatParameter parameter, float time, float targetValue = Mathf.Infinity) {
            this.parameter = parameter;
            this.OriginValue = parameter.value;
            this.TargetValue = (targetValue == Mathf.Infinity) ? OriginValue : targetValue;
            this.LerpTime = time;
        }

        /// <summary>
        /// Lerp the value of the parameter towards the target value.
        /// </summary>
        /// <param name="time">
        /// The time it takes lerp to finish
        /// (defaults to the given time parameter on instance creation if null)
        /// </param>
        public IEnumerator Lerp(float? time = null) {
            time = (time == null) ? LerpTime : Mathf.Max(0, (float) time);
            float source = parameter.value;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                float step = timer / (float) time;
                parameter.value = Mathf.Lerp(source, TargetValue, step);
                yield return null;
            }
        }
    }
}