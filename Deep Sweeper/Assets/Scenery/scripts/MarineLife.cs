using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MarineLife : MonoBehaviour
{
    #region Exposed Editor Parameters
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
    #endregion

    #region Constants
    private static readonly float TURN_TIME = .7f;
    private static readonly float SINE_TURN_ANGLE = 30f;
    private static readonly float APPEARANCE_TIME = 3.5f;
    #endregion

    #region Class Members
    private static int fishIdUtilizer = 0;
    private SkinnedMeshRenderer mesh;
    private FishPack pack;
    private Vector3 originScale;
    private float zigzagIntensity;
    private float timeFunction;
    private bool turning;
    #endregion

    #region Events
    public event UnityAction<MarineLife> FishTurnEvent;
    #endregion

    private void Awake() {
        this.mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        mesh.enabled = false;
    }

    /// <summary>
    /// Make the spawn swim along its defined direction.
    /// </summary>
    private IEnumerator Swim() {
        while (true) {
            if (!turning) {
                float sineWave = Mathf.Sin(timeFunction * zigzagIntensity);
                timeFunction += Time.deltaTime;

                Vector3 sideVector = transform.right * sineWave;
                Vector3 movementVector = (transform.forward + sideVector) * speed;
                Vector3 currentRot = transform.rotation.eulerAngles;
                Vector3 nextRot = new Vector3(currentRot.x, pack.YawDirection + sineWave * SINE_TURN_ANGLE, currentRot.z);

                transform.position += movementVector;
                transform.rotation = Quaternion.Euler(nextRot);
            }

            yield return null;
        }
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

    /// <summary>
    /// Make the spawn object appear of disappear.
    /// </summary>
    /// <param name="flag">True to appear of false to disappear</param>
    /// <param name="callback">A callback function to activate on completion (optional)</param>
    private IEnumerator Appear(bool flag, UnityAction callback = null) {
        Vector3 sourceScale = transform.localScale;
        Vector3 targetScale = flag ? originScale : Vector3.zero;
        float lerpedTime = 0;

        while (lerpedTime < APPEARANCE_TIME) {
            lerpedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(sourceScale, targetScale, lerpedTime / APPEARANCE_TIME);
            yield return null;
        }

        callback?.Invoke();
    }

    /// <summary>
    /// Destroy this marine life's object.
    /// </summary>
    public void Kill() {
        void callback() {
            StopAllCoroutines();
            FishPack pack = GetComponentInParent<FishPack>();
            Destroy(gameObject);
        };

        StartCoroutine(Appear(false, callback));
    }

    /// <summary>
    /// Spawn the marine life into the map.
    /// Calling this function is necessary.
    /// </summary>
    public void Spawn() {
        this.pack = GetComponentInParent<FishPack>();
        this.zigzagIntensity = 1 / Random.Range(zigzagIntensityRange.x, zigzagIntensityRange.y);
        this.originScale = transform.localScale;
        this.turning = false;
        this.timeFunction = 0;

        pack.YawDirectionChangeEvent += OnYawDirectionChange;
        StartCoroutine(Swim());
        transform.localScale = Vector3.zero;
        mesh.enabled = true;
        StartCoroutine(Appear(true));
    }
}