using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraShaker : MonoBehaviour
{
    private class EffectFloatParameter
    {
        private FloatParameter parameter;

        public float OriginValue { get; private set; }
        public float TargetValue { get; set; }

        /// <param name="parameter">The effect's parameter reference</param>
        public EffectFloatParameter(FloatParameter parameter) {
            if (parameter == null) return;

            this.parameter = parameter;
            this.OriginValue = parameter.value;
            this.TargetValue = OriginValue;
        }

        /// <summary>
        /// Lerp the value of the parameter.
        /// </summary>
        /// <param name="positive">
        /// True to lerp positively towards the maximum range value,
        /// or false to lerp towards the minimum range value.
        /// </param>
        /// <param name="time">The time it takes the lerp to finish</param>
        public IEnumerator Lerp(bool positive, float time) {
            if (parameter == null) yield break;

            float source = parameter.value;
            float target = positive ? TargetValue : OriginValue;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                parameter.value = Mathf.Lerp(source, target, timer / time);
                yield return null;
            }
        }
    }

    [Header("Wave Physics")]
    [Tooltip("The minimum possible distance that the camera will move while shaking.\n"
           + "The actual value is determined according to the intensity of the shake.")]
    [SerializeField] Vector3 minIntensity;

    [Tooltip("The maximum possible distance that the camera will move while shaking.\n"
           + "The actual value is determined according to the intensity of the shake.")]
    [SerializeField] Vector3 maxIntensity;

    [Tooltip("The frequency of the camera's shake.")]
    [SerializeField] private float frequency = 1;

    [Tooltip("A parameter that determines how fast will the shake decay.")]
    [SerializeField] private float entropy = .5f;

    [Tooltip("The minimal exponential wave multiplier at which the shake stops.")]
    [SerializeField] private float minimalDecay = .1f;

    [Header("FX")]
    [SerializeField] private float FXDecayTime = 1;

    [Header("Chromatic Aberration")]
    [Tooltip("True to enable post processing's 'Chromatic Aberration' effect manipulation.")]
    [SerializeField] private bool useChromaticAberration;

    [Tooltip("Overall intensity of the 'Chromatic Aberration' effect.")]
    [SerializeField] private float chromaticIntensity;

    [Header("Ambient Occlusion")]
    [Tooltip("True to enable post processing's 'Ambient Occlusion' effect manipulation.")]
    [SerializeField] private bool useAmbientOcclusion;

    [Tooltip("Overall intensity of the 'Ambient Occlusion' effect.")]
    [SerializeField] private float ambientIntensity;

    [Header("Bloom")]
    [Tooltip("True to enable post processing's 'Bloom' effect manipulation.")]
    [SerializeField] private bool useBloom;

    [Tooltip("Overall intensity of the 'Bloom' effect.")]
    [SerializeField] private float bloomIntensity;

    [Header("Depth of Field")]
    [Tooltip("True to enable post processing's 'Depth of Field' effect manipulation.")]
    [SerializeField] private bool useDepthOfField;

    [Tooltip("The size of the len's aperture.")]
    [SerializeField] private float aperture;

    private Vector3 originPos;
    private EffectFloatParameter ambientOcclusion;
    private EffectFloatParameter chromaticAberration;
    private EffectFloatParameter depthOfField;
    private EffectFloatParameter bloom;

    private void Start() {
        this.originPos = transform.localPosition;

        //post processing effects
        PostProcessingManager postProcessMngr = PostProcessingManager.Instance;
        this.ambientOcclusion = new EffectFloatParameter(postProcessMngr.AmbientOcclusion?.intensity);
        this.chromaticAberration = new EffectFloatParameter(postProcessMngr.ChromaticAberration?.intensity);
        this.depthOfField = new EffectFloatParameter(postProcessMngr.DepthOfField?.aperture);
        this.bloom = new EffectFloatParameter(postProcessMngr.Bloom?.intensity);

        //set target values
        ambientOcclusion.TargetValue = ambientIntensity;
        chromaticAberration.TargetValue = chromaticIntensity;
        depthOfField.TargetValue = aperture;
        bloom.TargetValue = bloomIntensity;
    }

    /// <summary>
    /// Shake the camera on its X axis.
    /// </summary>
    /// <param name="intensity">The percentage of shake power [0:1]</param>
    private IEnumerator WaveShake(float intensity) {
        float timer = 0;
        float exponentialDecay;
        float dirX = intensity * ((Random.Range(0f, 1f) > .5f) ? 1 : -1);
        float dirY = intensity * ((Random.Range(0f, 1f) > .5f) ? 1 : -1);
        float dirZ = intensity * ((Random.Range(0f, 1f) > .5f) ? 1 : -1);
        float clampIntensity = Mathf.Clamp(intensity, 0, 1);
        Vector3 direction = new Vector3(dirX, dirY, dirZ);
        Vector3 waveLength = RangeMath.PercentOfVectorRange(clampIntensity, minIntensity, maxIntensity);

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
    /// Activate all adapted post processing effects.
    /// </summary>
    /// <param name="flag">True to activate or false to deactivate</param>
    private void ActivateFX(bool flag) {
        if (useAmbientOcclusion) StartCoroutine(ambientOcclusion.Lerp(flag, FXDecayTime));
        if (useChromaticAberration) StartCoroutine(chromaticAberration.Lerp(flag, FXDecayTime));
        if (useBloom) StartCoroutine(bloom.Lerp(flag, FXDecayTime));
        if (useDepthOfField) StartCoroutine(depthOfField.Lerp(flag, FXDecayTime));
    }

    /// <summary>
    /// Shake the camera on its X axis.
    /// </summary>
    /// <param name="intensity">The percentage of shake power [0:1]</param>
    public void Shake(float intensity = 1) {
        transform.localPosition = originPos;
        StopAllCoroutines();
        ActivateFX(true);
        StartCoroutine(WaveShake(intensity));
    }
}