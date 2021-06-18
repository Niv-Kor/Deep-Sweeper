using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace DeepSweeper.CameraSet.PostProcessing
{
    [RequireComponent(typeof(PostProcessVolume))]
    public class PostProcessingManager : MonoBehaviour
    {
        #region Class Members
        private PostProcessVolume volume;
        private float startingWeight;
        #endregion

        private void Awake() {
            this.volume = GetComponent<PostProcessVolume>();
            this.startingWeight = volume.weight;
        }

        /// <summary>
        /// Set the volume's weight.
        /// </summary>
        /// <param name="targetWeight">A weight to set</param>
        /// <param name="time">The time it takes to set that weight</param>
        private IEnumerator SetWeight(float targetWeight, float time) {
            float srcWeight = volume.weight;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                volume.weight = Mathf.Lerp(srcWeight, targetWeight, timer / time);
                yield return null;
            }
        }

        /// <summary>
        /// Activate or deactivate the post processing volume.
        /// </summary>
        /// <param name="flag">True to activate or false to deactivate</param>
        /// <param name="time">The time it takes to activate or deactivate</param>
        public void Activate(bool flag = true, float time = 0) {
            float targetWeight = flag ? 1 : startingWeight;
            StopAllCoroutines();
            StartCoroutine(SetWeight(targetWeight, time));
        }

        /// <param name="filter">The name of the filter</param>
        /// <returns>The filter's settings object.</returns>
        public PostProcessEffectSettings GetFilter<T>() where T : PostProcessEffectSettings {
            volume.profile.TryGetSettings(out T settings);
            return settings;
        }
    }
}