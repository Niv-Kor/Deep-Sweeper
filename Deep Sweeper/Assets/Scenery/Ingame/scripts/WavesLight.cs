using UnityEngine;

public class WavesLight : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The alpha value of the waves' light")]
    [SerializeField] private float brightness;

    [Tooltip("The minimum (inclusive) and maximum (exclusive) values the waves' speed can get.\n"
           + "This value is generated based on the waves intensity in the global Waves component.")]
    [SerializeField] private Vector2 speedRange;
    #endregion

    #region Class Members
    private Light lightComponent;
    private Vector3 direction;
    private float motionSpeed;
    #endregion

    private void Start() {
        WaterPhysics waves = WaterPhysics.Instance;
        this.lightComponent = GetComponent<Light>();
        this.direction = waves.Direction;
        float globalIntens = waves.IntensityPercentage;
        float intensDiff = speedRange.y - speedRange.x;
        this.motionSpeed = globalIntens * intensDiff / 100f;
        WaterPhysics.Instance.WavesChangedEvent += OnWavesChange;
        lightComponent.intensity = brightness;
    }

    private void Update() {
        transform.position += motionSpeed * direction;
    }

    /// <summary>
    /// Activate on WavesChangeTrigger event.
    /// </summary>
    /// <param name="settings">The new settings of the waves.</param>
    private void OnWavesChange(WaveSettings settings) {
        this.direction = settings.Direction;
        this.motionSpeed = settings.Intensity;
    }
}