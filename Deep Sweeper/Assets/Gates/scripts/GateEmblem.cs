using System.Collections;
using UnityEngine;

public class GateEmblem : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Glitch")]
    [Tooltip("The range of possible glitch delay values.")]
    [SerializeField] private Vector2 randomGlitchDelay;

    [Tooltip("The strecth intensity of the emblem during the glitch.")]
    [SerializeField] private float glitchIntensity;

    [Tooltip("The time it takes the glitch to abort and return back to normal.")]
    [SerializeField] private float glitchTime;

    [Header("Motion")]
    [Tooltip("The speed of the emblem while circulatin.")]
    [SerializeField] private float speed = 1;

    [Tooltip("The radius of the circle around with the emblem moves.")]
    [SerializeField] private float radius;

    [Tooltip("True to move the emblem clockwise around the circle.")]
    [SerializeField] private bool clockwise = true;
    #endregion

    #region Constants
    private static readonly float CIRCULAR_DELTA = .1f;
    #endregion

    #region Class Members
    private Vector3 initPos;
    private int direction;
    #endregion

    private void Start() {
        this.direction = clockwise ? -1 : 1;
        float initialDeltaX = radius / 2 * direction;
        float initialDeltaY = radius / 2 * direction * -1;
        transform.Translate(initialDeltaX, initialDeltaY, 0);
        this.initPos = transform.position;

        StartCoroutine(Move());
        StartCoroutine(Glitch());
    }

    /// <summary>
    /// Move the emblem in a circular shape.
    /// </summary>
    private IEnumerator Move() {
        float angle;
        float timer = 0;
        float maxAngle = Mathf.PI * 2 * direction;
        transform.position = initPos;

        while (true) {
            //reset circular motion
            if (timer >= radius) {
                timer = 0;
                transform.position = initPos;
            }

            timer += Time.deltaTime * speed;
            angle = Mathf.Lerp(0, maxAngle, timer / radius);

            //clamp angle between 0 and 360 degrees
            if (maxAngle > 0) angle = Mathf.Clamp(angle, 0, maxAngle);
            else angle = Mathf.Clamp(angle, maxAngle, 0);

            //move
            float deltaX = Mathf.Cos(angle) * CIRCULAR_DELTA;
            float deltaY = Mathf.Sin(angle) * CIRCULAR_DELTA;
            transform.Translate(deltaX, deltaY, 0);

            yield return null;
        }
    }

    /// <summary>
    /// Apply a glitch effect to the emblem.
    /// </summary>
    private IEnumerator Glitch() {
        Vector3 originScale = transform.localScale;
        Vector3 reverseMask = Vector3.one - Vector3.right;
        float originWidth = originScale.x;
        float glitchWidth = originWidth + glitchIntensity;
        float glitchDelay = GenerateGlitchDelay();
        bool currentOrigin = true;
        float timer = 0;

        while (true) {
            timer += Time.deltaTime;
            bool timeup = timer >= glitchDelay;
            bool glitchAbortion = !currentOrigin && timer >= glitchTime;

            if (timeup || glitchAbortion) {
                float newWidth = currentOrigin ? glitchWidth : originWidth;
                Vector3 widthVector = Vector3.right * newWidth;
                transform.localScale = Vector3.Scale(originScale, reverseMask) + widthVector;

                //finish
                glitchDelay = GenerateGlitchDelay();
                currentOrigin ^= true;
                timer = 0;
            }

            yield return null;
        }
    }

    /// <returns>A random glitch delay.</returns>
    private float GenerateGlitchDelay() {
        Vector2 range = randomGlitchDelay;
        return Random.Range(range.x, range.y);
    }
}