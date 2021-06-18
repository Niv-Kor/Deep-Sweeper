using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace DeepSweeper.CameraSet.PostProcessing
{
    public class VisualEffectsSheet
    {
        #region Constants
        private static readonly string FILTERS_NAMESPACE = "UnityEngine.Rendering.PostProcessing";
        private static readonly string EFFECT_NOT_FOUND_ERR = "Encountered an issue finiding the effect '{0}' in the {1} filter.";
        #endregion

        #region Class Members
        private List<DynamicEffectValue> effects;
        private PostProcessingManager manager;
        private IDictionary<DynamicEffectValue, float> maxValues;
        private Coroutine executionCoroutine;
        #endregion

        /// <summary>
        /// Create a new VisualEffectsSheet instance.
        /// </summary>
        /// <param name="camera">The camera whose post processing manager this sheet manipulates</param>
        /// <param name="instructions">A list of effect manipulation instructions</param>
        /// <returns>
        /// A new VisualEffectsSheet instance,
        /// or null if the post processing manager could not be found.
        /// </returns>
        public static VisualEffectsSheet Create(CameraRole camera, List<EffectInstructions> instructions = null) {
            PostProcessingManager mngr = CameraManager.Instance.GetPostProcessManager(camera);
            return (mngr != null) ? new VisualEffectsSheet(mngr, instructions) : null;
        }

        /// <param name="manager">A post processing manager to manipulate</param>
        /// <param name="instructions">A list of effect manipulation instructions</param>
        private VisualEffectsSheet(PostProcessingManager manager, List<EffectInstructions> instructions) {
            this.effects = new List<DynamicEffectValue>();
            this.maxValues = new Dictionary<DynamicEffectValue, float>();
            this.manager = manager;

            SetInstructions(instructions);
        }

        /// <summary>
        /// Set a list of instruction for this sheet to manipulate.
        /// </summary>
        /// <param name="instructions">The new list of instructions</param>
        public void SetInstructions(List<EffectInstructions> instructions) {
            maxValues.Clear();
            instructions ??= new List<EffectInstructions>();

            //create an effect value for each item in the list
            foreach (EffectInstructions instruction in instructions) {
                PostProcessEffectSettings filter = FindFilterType(instruction.Filter);

                if (filter != null) {
                    try {
                        //find the exact class member using reflection
                        Type filterType = filter.GetType();
                        MemberInfo memberInfo = filterType.GetMember(instruction.Effect)[0];
                        FieldInfo fieldInfo = (FieldInfo)memberInfo;
                        FloatParameter parameter = (FloatParameter)fieldInfo.GetValue(filter);
                        DynamicEffectValue effect = DynamicEffectValue.Create(parameter, instruction.LerpTime, instruction.Value);

                        if (effect != null) {
                            maxValues.Add(effect, instruction.Value);
                            effects.Add(effect);
                        }
                    }
                    //filter not found (probably mistyped)
                    catch (Exception e) {
                        string errString = string.Format(EFFECT_NOT_FOUND_ERR, instruction.Effect, filter);
                        throw new Exception(errString, e);
                    }
                }
            }
        }

        /// <summary>
        /// Find the type of a filter based on its enum value.
        /// </summary>
        /// <param name="filter">The filter's enum value</param>
        /// <returns>The exact type of the filter (derived from PostProcessingEffectSettings).</returns>
        private PostProcessEffectSettings FindFilterType(Filter filter) {
            string className = filter.ToString();
            Type filterType = typeof(PostProcessVolume).Assembly.GetType($"{FILTERS_NAMESPACE}.{className}");

            if (filterType != null) {
                var genMethod = typeof(PostProcessingManager).GetMethod(nameof(PostProcessingManager.GetFilter));
                var typedMethod = genMethod.MakeGenericMethod(filterType);
                var result = (PostProcessEffectSettings) typedMethod.Invoke(manager, null);
                return result;
            }
            else return null;
        }

        /// <summary>
        /// Execute the change of the effects' values.
        /// </summary>
        /// <param name="time">The time it takes the change to finish [s]</param>
        /// <param name="percent">
        /// The percentage of the change [0:1],
        /// where 0 means the effects' original float value (from when the sheet was created),
        /// and 1 means the effects' configured target float value (from the given instructions).
        /// </param>
        public List<IEnumerator> GetExecutionCoroutines(float? time = null, float percent = 1) {
            List<IEnumerator> list = new List<IEnumerator>();
            percent = Mathf.Clamp(percent, 0f, 1f);

            foreach (DynamicEffectValue effect in effects) {
                if (maxValues.TryGetValue(effect, out float maxValue)) {
                    Vector2 valuesRange = new Vector2(effect.OriginValue, maxValue);
                    float tempValue = RangeMath.PercentOfRange(percent, valuesRange);
                    effect.TargetValue = tempValue;
                    list.Add(effect.Lerp(time));
                }
            }

            return list;
        }
    }
}