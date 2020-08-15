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

    [Tooltip("The minimum (inclusive) and maximum (exclusive) value of the creature's height.")]
    [SerializeField] public Vector2 HeightRange;

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

    private static readonly float TURN_ANGLE = 30f;

    private Vector3 forwardVec;
    private float zigzagIntensity;

    private void Start() {
        this.forwardVec = transform.forward;
        this.zigzagIntensity = 1 / Random.Range(zigzagIntensityRange.x, zigzagIntensityRange.y);
        StartCoroutine(Swim());
    }

    /// <summary>
    /// Swim along the creature's path.
    /// </summary>
    private IEnumerator Swim() {
        while (true) {
            float sinePhase = Mathf.Sin(Time.time * zigzagIntensity);
            Vector3 sideVector = transform.right * sinePhase;
            Vector3 movementVector = (forwardVec + sideVector) * speed;
            transform.position += movementVector;
            transform.rotation = Quaternion.Euler(new Vector3(0, sinePhase * TURN_ANGLE, 0));

            yield return null;
        }
    }
}