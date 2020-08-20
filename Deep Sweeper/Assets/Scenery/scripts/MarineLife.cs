using System.Collections;
using UnityEngine;

public class MarineLife : MonoBehaviour
{
    [Header("Individual Settings")]
    [Tooltip("The creature's movement speed (distance moved per frame).")]
    [SerializeField] private float speed;

    [Tooltip("The range of the distance the creature is doing from its center point to any of its sides while swimming."
           + "The distance parameter is decided randomly within this range.")]
    [SerializeField] private Vector2 zigzagIntensityRange;

    [Header("Spawn Preferances")]
    [Tooltip("The minimal depth at which the creature can live.")]
    [SerializeField] public float DepthHabitat;

    [Tooltip("The minimum (inclusive) and maximum (exclusive) amount of packs to spawn around the map.")]
    [SerializeField] public Vector2Int PacksAmountRange;

    [Tooltip("Minimum (inclusive) and maximum (exclusive) pack size value.")]
    [SerializeField] public Vector2Int PackSizeRange;

    [Tooltip("The minimum 3D distance between each individual from the center instance.")]
    [SerializeField] public Vector3 MinIndividualDistance;

    [Tooltip("The maximum 3D distance between each individual from the center instance.")]
    [SerializeField] public Vector3 MaxIndividualDistance;

    [Tooltip("Affection of the ground level's depth on the volume of spawn.")]
    [SerializeField] public SpawnAffection DepthAffection;

    [Tooltip("Affection of the waves' turbulence on the volume of spawn.")]
    [SerializeField] public SpawnAffection TurbulenceAffection;

    private static readonly float TURN_TIME = .7f;
    private static readonly float SINE_TURN_ANGLE = 30f;

    private FishPack pack;
    private float zigzagIntensity;
    private float sineWave, timeFunction;
    private bool turning;

    public delegate void FishTurn(MarineLife fish);
    public event FishTurn FishTurnEvent;

    private void Start() {
        this.pack = GetComponentInParent<FishPack>();
        this.zigzagIntensity = 1 / Random.Range(zigzagIntensityRange.x, zigzagIntensityRange.y);
        this.turning = false;
        this.timeFunction = 0;
        this.sineWave = 0;

        pack.YawDirectionChangeEvent += OnYawDirectionChange;
    }

    private void Update() {
        if (!turning) {
            sineWave = Mathf.Sin(timeFunction * zigzagIntensity);
            timeFunction += Time.deltaTime;
            Swim(sineWave);
        }
    }

    /// <summary>
    /// Make the spawn swim along its defined direction.
    /// </summary>
    /// <param name="sineWave">A changing sine wave value [-1:1]</param>
    private void Swim(float sineWave) {
        Vector3 sideVector = transform.right * sineWave;
        Vector3 movementVector = (transform.forward + sideVector) * speed;
        Vector3 currentRot = transform.rotation.eulerAngles;
        Vector3 nextRot = new Vector3(currentRot.x, pack.YawDirection + sineWave * SINE_TURN_ANGLE, currentRot.z);

        transform.position += movementVector;
        transform.rotation = Quaternion.Euler(nextRot);
    }

    /// <summary>
    /// Slowly turn to a specific direction.
    /// </summary>
    /// <param name="direction">The direction into which to turn</param>
    private IEnumerator Turn(Vector3 direction) {
        Quaternion sourceRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(direction);
        float lerpedTime = 0;

        while (lerpedTime < TURN_TIME) {
            lerpedTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(sourceRot, targetRot, lerpedTime / TURN_TIME);
            yield return null;
        }

        FishTurnEvent?.Invoke(this);
        turning = false;
    }

    /// <summary>
    /// Activate when the pack's yaw direction changes.
    /// </summary>
    /// <param name="yaw">The yaw value</param>
    private void OnYawDirectionChange(float yaw) {
        turning = true;
        timeFunction = 0;
        Vector3 currentRot = transform.rotation.eulerAngles;
        Vector3 direction = new Vector3(currentRot.x, yaw, currentRot.z);
        StartCoroutine(Turn(direction));
    }
}