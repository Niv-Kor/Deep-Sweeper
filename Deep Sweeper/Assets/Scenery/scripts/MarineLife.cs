using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class MarineLife : MonoBehaviour
{
    [Tooltip("The creature's movement speed (distance moved per frame).")]
    [SerializeField] private float speed;

    [Tooltip("The range of the distance the creature is doing from its center point to any of its sides while swimming."
           + "The distance parameter is decided randomly within this range.")]
    [SerializeField] private Vector2 zigzagIntensityRange;

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

    Vector3 WaveLerp(Vector3 origin, Vector3 dest, Vector3 up, float amplitude, float freq, float fraction) {
        float x = fraction * (dest - origin).magnitude;
        float w = 2 * Mathf.PI * freq;
        float y = amplitude * Mathf.Sin(w * x);

        Vector3 xDir = (dest - origin).normalized;
        Vector3 yDir = Vector3.Cross(xDir, up).normalized;

        Vector3 pathOffset = x * xDir;
        Vector3 waveOffset = y * yDir;

        return origin + pathOffset + waveOffset;
    }
}