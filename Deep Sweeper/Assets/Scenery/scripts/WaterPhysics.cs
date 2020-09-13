using UnityEngine;
using UnityEngine.Events;

public class WaterPhysics : Singleton<WaterPhysics>
{
    [Header("Turbulence")]
    [Tooltip("Set waves' intensity and direction randomly on awake.")]
    [SerializeField] private bool setRandom;

    [Tooltip("The minimum (invlusive) and maximum (exclusive) values a random intensity can receive.")]
    [SerializeField] private Vector2 intensityRange;

    [Tooltip("The Y axis of the gravitational force when setting gravity direction randomly.")]
    [SerializeField] private float baseFloatForce = 1f;

    [Header("Vision")]
    [Tooltip("Maximum distance that the camera may display.")]
    [SerializeField] private float maxVisionDistance = 500f;

    [Tooltip("Maximum distance of clear vision before the cloud of fog.")]
    [SerializeField] private float fogDistance = 25f;

    private Vector3 m_direction;
    private float m_intensity;

    public event UnityAction<WaveSettings> WavesChangeTrigger;

    public Vector3 Direction {
        get { return m_direction; }
        private set {
            value = Vector3.Normalize(value);
            m_direction = value;
            InvokeChanges(value, Intensity);
        }
    }

    public float Intensity {
        get { return m_intensity; }
        set {
            m_intensity = value;
            InvokeChanges(Direction, value);
        }
    }

    public float IntensityPercentage {
        get { return RangeMath.NumberOfRange(Intensity, intensityRange); }
    }

    public float FogDistance {
        get { return fogDistance; }
    }

    private void Awake() {
        //concatenate gravity event
        WavesChangeTrigger += OnWavesChange;

        if (setRandom) {
            float dirX = Random.Range(-1f, 1f);
            float dirZ = Random.Range(-1f, 1f);
            this.Direction = new Vector3(dirX, baseFloatForce, dirZ);
            this.Intensity = Random.Range(intensityRange.x, intensityRange.y);
        }
        else {
            this.Direction = Vector3.zero;
            this.Intensity = 0;
        }
    }

    private void OnValidate() {
        Camera.main.farClipPlane = maxVisionDistance;
    }

    /// <summary>
    /// Create a new WavesSettings structure.
    /// </summary>
    /// <param name="dir">Normalized direction of the waves</param>
    /// <param name="intensity">The intensity of the waves</param>
    /// <returns>A structure that contains the input settings.</returns>
    private WaveSettings CreateSettings(Vector3 dir, float intensity) {
        WaveSettings settings;
        settings.Direction = dir;
        settings.Intensity = intensity;
        return settings;
    }

    /// <summary>
    /// Invoke the event of wave changes.
    /// </summary>
    /// <param name="dir">Normalized direction of the waves</param>
    /// <param name="intensity">The intensity of the waves</param>
    private void InvokeChanges(Vector3 dir, float intensity) {
        WaveSettings settings = CreateSettings(dir, intensity);
        WavesChangeTrigger?.Invoke(settings);
    }

    /// <summary>
    /// Activate on WavesChangeTrigger event.
    /// </summary>
    /// <param name="settings">The new settings of the waves.</param>
    private void OnWavesChange(WaveSettings settings) {
        Physics.gravity = settings.Direction * settings.Intensity;
    }
}