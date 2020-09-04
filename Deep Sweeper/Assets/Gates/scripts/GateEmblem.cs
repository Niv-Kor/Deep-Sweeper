using System.Collections;
using UnityEngine;

public class GateEmblem : MonoBehaviour
{
    [Header("Glitch")]
    [Tooltip("The range of possible glitch delay values.")]
    [SerializeField] private Vector2 randomGlitchDelay;

    [Tooltip("The strecth intensity of the emblem during the glitch.")]
    [SerializeField] private float glitchIntensity;

    [Tooltip("The time it takes the glitch to abort and return back to normal.")]
    [SerializeField] private float glitchTime;

    [Header("Motion")]
    [Tooltip("The time it takes to complete a full circular motion.")]
    [SerializeField] private float circularTime = 1;

    [Tooltip("The radius of the circle around with the emblem moves.")]
    [SerializeField] private float radius;

    [Tooltip("True to move the emblem clockwise around the circle.")]
    [SerializeField] private bool clockwise = true;

    private void Start() {
        StartCoroutine(Move());
        StartCoroutine(Glitch());
    }

    /// <summary>
    /// Move the emblem in a circular shape.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Move() {
        float angle = 0;
        int direction = clockwise ? -1 : 1;
        float speed = 2 * Mathf.PI / circularTime;

        while (true) {
            angle += speed * Time.deltaTime * direction;
            float deltaX = Mathf.Cos(angle) * radius;
            float deltaY = Mathf.Sin(angle) * radius;
            Vector3 delta = new Vector3(deltaX, deltaY, 0);
            transform.position += delta;

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