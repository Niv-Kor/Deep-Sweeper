using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace DeepSweeper.CameraSet
{
    [RequireComponent(typeof(PostProcessVolume))]
    public class PostProcessingManager : MonoBehaviour
    {
        #region Class Members
        private PostProcessVolume volume;
        private float startingWeight;
        #endregion

        #region Properties
        public ScreenSpaceReflections ScreenSpaceReflections { get; private set; }
        public ChromaticAberration ChromaticAberration { get; private set; }
        public AmbientOcclusion AmbientOcclusion { get; private set; }
        public LensDistortion LensDistortion { get; private set; }
        public AutoExposure AutoExposure { get; private set; }
        public ColorGrading ColorGrading { get; private set; }
        public DepthOfField DepthOfField { get; private set; }
        public MotionBlur MotionBlur { get; private set; }
        public Vignette Vignette { get; private set; }
        public Bloom Bloom { get; private set; }
        public Grain Grain { get; private set; }
        #endregion

        private void Start() {
            this.volume = GetComponent<PostProcessVolume>();
            this.startingWeight = volume.weight;

            //extract settings
            volume.profile.TryGetSettings(out ScreenSpaceReflections screenSpaceReflections);
            volume.profile.TryGetSettings(out ChromaticAberration chromaticAberration);
            volume.profile.TryGetSettings(out AmbientOcclusion ambientOcclusion);
            volume.profile.TryGetSettings(out LensDistortion lensDistortion);
            volume.profile.TryGetSettings(out AutoExposure autoExposure);
            volume.profile.TryGetSettings(out ColorGrading colorGrading);
            volume.profile.TryGetSettings(out DepthOfField depthOfField);
            volume.profile.TryGetSettings(out MotionBlur motionBlur);
            volume.profile.TryGetSettings(out Vignette vignette);
            volume.profile.TryGetSettings(out Bloom bloom);
            volume.profile.TryGetSettings(out Grain grain);

            //assign to properties
            this.ScreenSpaceReflections = screenSpaceReflections;
            this.ChromaticAberration = chromaticAberration;
            this.AmbientOcclusion = ambientOcclusion;
            this.LensDistortion = lensDistortion;
            this.AutoExposure = autoExposure;
            this.ColorGrading = colorGrading;
            this.DepthOfField = depthOfField;
            this.MotionBlur = motionBlur;
            this.Vignette = vignette;
            this.Bloom = bloom;
            this.Grain = grain;
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
    }
}