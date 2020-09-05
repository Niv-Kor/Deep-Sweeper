using System.Collections;
using UnityEngine;

public class MineBouncer : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("The mine's grounding chain joint.")]
    [SerializeField] private GameObject baseChainJoint;

    [Header("Bounce Settings")]
    [Tooltip("True to randomly start the bouncing after X miliseconds.")]
    [SerializeField] private bool randomizeStart = false;

    [Tooltip("The distance the mine makes either up or down while bouncing.")]
    [SerializeField] private float epsilon;

    [Tooltip("Bouncing speed multiplier.")]
    [SerializeField] private float speed = 1;

    [Header("Rotation")]
    [Tooltip("The angle at which the mine yaws for each frame.")]
    [SerializeField] private float yawAlpha;

    private static readonly float MAX_DELAY = 1;

    private float chainLength;
    private Transform chain;

    private void Start() {
        Renderer chainJointRenderer = baseChainJoint.GetComponent<Renderer>();
        float jointLength = chainJointRenderer.bounds.size.y;
        this.chain = baseChainJoint.transform.parent;
        this.chainLength = chain.childCount * jointLength;

        float delay = randomizeStart ? Random.Range(0, MAX_DELAY) : 0;
        StartCoroutine(Bounce(delay));
    }

    private void Update() {
        Vector3 currentRot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRot + yawAlpha * Vector3.up);
    }

    /// <summary>
    /// Bounce the mine up and down.
    /// </summary>
    /// <param name="delay">Amount of time to wait before the first bounce.</param>
    private IEnumerator Bounce(float delay) {
        Vector3 originPos = transform.position;
        Vector3 chainScale = chain.transform.localScale;
        Vector3 chainScaleMask = Vector3.one - Vector3.up;
        float timer = 0;

        if (delay > 0) yield return new WaitForSeconds(delay);

        while (true) {
            timer += Time.deltaTime;
            float sineWave = Mathf.Sin(timer * speed);
            float delta = sineWave * epsilon;
            float chainPercent = (chainLength + delta) / chainLength;
            Vector3 chainScaleVect = chainScaleMask + chainPercent * Vector3.up;
            transform.position = originPos + delta * Vector3.up;
            chain.transform.localScale = Vector3.Scale(chainScale, chainScaleVect);

            yield return null;
        }
    }
}