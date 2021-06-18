using DeepSweeper.CameraSet.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.CameraSet
{
    public class CameraShaker : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Intensity")]
        [Tooltip("The minimum possible distance that the camera will move while shaking.\n"
            + "The actual value is determined according to the intensity of the shake.")]
        [SerializeField] private Vector3 minIntensity;

        [Tooltip("The maximum possible distance that the camera will move while shaking.\n"
                + "The actual value is determined according to the intensity of the shake.")]
        [SerializeField] private Vector3 maxIntensity;

        [Tooltip("Minimum and maximum distance, by which the relative shake insensity is calculated.\n"
                + "Maximum intensity takes place at the minimum distance, and minimum intensity takes"
                + "place at the maximum distance.")]
        [SerializeField] private Vector2Int distanceRange;

        [Header("Physics")]
        [Tooltip("The frequency of the camera's wave shake.")]
        [SerializeField] private float frequency = 1;

        [Tooltip("A parameter that determines how fast will the shake energy decay.")]
        [SerializeField] private float entropy = .5f;

        [Tooltip("The minimal exponential wave multiplier at which the shake stops entirely.")]
        [SerializeField] private float minimalDecay = .1f;

        [Header("FX")]
        [Tooltip("A list of effect instructions to apply on shake.")]
        [SerializeField] private List<EffectInstructions> FXInstructions;

        [Tooltip("The time it takes the effects to decay [s].")]
        [SerializeField] private float FXDecayTime = 1;
        #endregion

        #region Class Members
        private Camera cameraCmp;
        private VisualEffectsSheet visualFX;
        private Coroutine shakeCoroutine;
        private Coroutine vibrationCoroutine;
        private Vector3 originPos;
        private float originFieldOfView;
        #endregion

        private void Awake() {
            this.originPos = transform.localPosition;
            this.cameraCmp = GetComponent<Camera>();
            this.originFieldOfView = cameraCmp.fieldOfView;
        }

        private void Start() {
            this.visualFX = VisualEffectsSheet.Create(CameraRole.Main, FXInstructions);
        }

        /// <summary>
        /// Shake the camera.
        /// </summary>
        /// <param name="intensity">The percentage of shake power [0:1]</param>
        /// <param name="xAxis">True to include an X axis shake</param>
        /// <param name="yAxis">True to include a Y axis shake</param>
        /// <param name="zAxis">True to include a Z axis shake</param>
        private IEnumerator WaveShake(float intensity, bool xAxis = true, bool yAxis = true, bool zAxis = true) {
            if (!xAxis && !yAxis && !zAxis) yield break;

            float timer = 0;
            float exponentialDecay;
            float dirX = intensity * ((Random.Range(0f, 1f) > .5f) ? 1 : -1);
            float dirY = intensity * ((Random.Range(0f, 1f) > .5f) ? 1 : -1);
            float dirZ = intensity * ((Random.Range(0f, 1f) > .5f) ? 1 : -1);
            Vector3 multiplier = new Vector3(xAxis ? 1 : 0, yAxis ? 1 : 0, zAxis ? 1 : 0);
            Vector3 direction = Vector3.Scale(new Vector3(dirX, dirY, dirZ), multiplier);
            Vector3 waveLength = RangeMath.PercentOfVectorRange(intensity, minIntensity, maxIntensity);

            do {
                timer += Time.deltaTime;
                exponentialDecay = Mathf.Exp(-timer / entropy);
                float sineWave = Mathf.Sin(frequency * timer);
                float dampedOscillation = exponentialDecay * sineWave;
                Vector3 delta = Vector3.Scale(dampedOscillation * waveLength, direction);
                transform.localPosition = originPos + delta;

                yield return null;
            }
            while (exponentialDecay > minimalDecay);

            transform.localPosition = originPos;
            ActivateFX(false);
        }

        /// <summary>
        /// Rapidly change the camera's field of view back and forth,
        /// creating a vibartion effect.
        /// </summary>
        /// <param name="intensity">The percentage of vibration power [0:1]</param>
        private IEnumerator WaveVibrate(float intensity) {
            float timer = 0;
            float exponentialDecay;
            float waveLength1 = RangeMath.PercentOfRange(intensity, minIntensity.z, maxIntensity.z);
            float dir = intensity * ((Random.Range(0f, 1f) > .5f) ? 1 : -1);
            originFieldOfView = cameraCmp.fieldOfView;

            do {
                timer += Time.deltaTime;
                exponentialDecay = Mathf.Exp(-timer / entropy);
                float sineWave = Mathf.Sin(frequency * timer);
                float dampedOscillation = exponentialDecay * sineWave;
                float delta = dampedOscillation * waveLength1 * dir;
                cameraCmp.fieldOfView = originFieldOfView + delta;

                yield return null;
            }
            while (exponentialDecay > minimalDecay);

            cameraCmp.fieldOfView = originFieldOfView;
            ActivateFX(false);
        }

        /// <summary>
        /// Activate all adapted post processing effects.
        /// </summary>
        /// <param name="flag">True to activate or false to deactivate</param>
        /// <param name="intensity">The percentage of effect intensity [0:1]</param>
        private void ActivateFX(bool flag, float intensity = 1) {
            float percentage = flag ? intensity : 0;
            float? time = flag ? (float?) null : FXDecayTime;
            VisualEffectsAnimator.Instance.Animate(visualFX, time, percentage);
        }

        /// <summary>
        /// Shake the camera.
        /// </summary>
        /// <param name="intensity">The percentage of shake power [0:1]</param>
        /// <param name="xAxis">True to include an X axis shake</param>
        /// <param name="yAxis">True to include a Y axis shake</param>
        /// <param name="zAxis">True to include a Z axis shake</param>
        public void Shake(float intensity = 1, bool xAxis = true, bool yAxis = true, bool zAxis = true) {
            intensity = Mathf.Clamp(intensity, 0, 1);
            if (intensity == 0) return;

            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            transform.localPosition = Vector3.zero;
            ActivateFX(true, intensity);
            shakeCoroutine = StartCoroutine(WaveShake(intensity, xAxis, yAxis, zAxis));
        }

        /// <summary>
        /// Shake the camera relative to its distance from an object.
        /// </summary>
        /// <param name="obj">The object against which to measure the distance</param>
        /// <param name="xAxis">True to include an X axis shake</param>
        /// <param name="yAxis">True to include a Y axis shake</param>
        /// <param name="zAxis">True to include a Z axis shake</param>
        public void ShakeRelativeTo(Transform obj, bool xAxis = true, bool yAxis = true, bool zAxis = true) {
            float dist = Vector3.Distance(transform.position, obj.position);
            dist = Mathf.Max(distanceRange.x, dist);
            float intensity = 1 - RangeMath.NumberOfRange(dist, distanceRange.x, distanceRange.y);
            Shake(intensity, xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// Rapidly change the camera's field of view back and forth,
        /// creating a vibartion effect.
        /// </summary>
        /// <param name="intensity">The percentage of shake power [0:1]</param>
        /// <param name="effectsIntesity">
        /// The percentage of camera effects intensity [0:1].
        /// If set to -1 (by default), its intensity is the same as the 'intensity' parameter.
        /// </param>
        public void Vibrate(float intensity = 1, float effectsIntesity = -1) {
            intensity = Mathf.Clamp(intensity, 0, 1);
            if (intensity == 0) return;

            effectsIntesity = (effectsIntesity == -1) ? intensity : Mathf.Clamp(effectsIntesity, 0, 1);

            if (vibrationCoroutine != null) StopCoroutine(vibrationCoroutine);
            cameraCmp.fieldOfView = originFieldOfView;
            ActivateFX(true, effectsIntesity);
            vibrationCoroutine = StartCoroutine(WaveVibrate(intensity));
        }
    }
}