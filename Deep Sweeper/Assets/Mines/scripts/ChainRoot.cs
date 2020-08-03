using UnityEngine;

public class ChainRoot : MonoBehaviour
{
    [Tooltip("The strength of the waves that move the chain.")]
    [SerializeField] private float waveForce = 1;

    [Tooltip("Minimum speed at which the wave is moving the chain.")]
    [SerializeField] private float minWaveSpeed = .2f;

    [Tooltip("Maximum speed at which the wave is moving the chain.")]
    [SerializeField] private float maxWaveSpeed = 1;

    private Rigidbody rigidBody;
    private Vector3 waveVector;
    private Vector3 stepVector;
    private float waveSpeed, currentWaveForce;

    private void Start() {
        this.rigidBody = GetComponent<Rigidbody>();
        this.waveVector = GenerateBaseVector();
        this.stepVector = GetSignVector(waveVector);
        this.waveSpeed = Random.Range(minWaveSpeed, maxWaveSpeed);
        this.currentWaveForce = waveForce;
    }

    private void Update() {
        float avgVectorVal = Mathf.Abs(CalcAverageVector(waveVector));
        bool limit = ReachedVectorLimit(waveVector, currentWaveForce);

        //regenerate movement
        if (limit) {
            waveVector = GenerateBaseVector();
            stepVector = GetSignVector(waveVector);
            waveSpeed = Random.Range(minWaveSpeed, maxWaveSpeed);
            currentWaveForce = waveForce;
        }

        waveVector += stepVector * waveSpeed;
        rigidBody.AddForce(waveVector * currentWaveForce);
    }

    /// <summary>
    /// Generate a small normalized vector.
    /// </summary>
    /// <returns>A small normalized vector where all values are in [-1|1] range.</returns>
    private Vector3 GenerateBaseVector() {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Calculate the normalized sign vector of a given vector.
    /// E.g: (.4, -.6, -.2) => (1, -1, -1).
    /// </summary>
    /// <param name="baseVector">The vector of which to get the sign vector</param>
    /// <returns>A normalized sign vector.</returns>
    private Vector3 GetSignVector(Vector3 baseVector) {
        int x = (baseVector.x > 0) ? 1 : -1;
        int y = (baseVector.y > 0) ? 1 : -1;
        int z = (baseVector.z > 0) ? 1 : -1;
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Check if a vector has reached a maximum with each of its coordinates.
    /// </summary>
    /// <param name="vector">The vector to check</param>
    /// <param name="maxValue">Maximum value (is altered according to the sign of the coordinate)</param>
    /// <returns>True if the vector has reached the value with each of its coordinates.</returns>
    private bool ReachedVectorLimit(Vector3 vector, float maxValue) {
        bool xOver = Mathf.Abs(vector.x) >= maxValue;
        bool yOver = Mathf.Abs(vector.y) >= maxValue;
        bool zOver = Mathf.Abs(vector.z) >= maxValue;
        return xOver && yOver && zOver;
    }

    /// <summary>
    /// Calculate the mean of a vector's values.
    /// </summary>
    /// <param name="vec">The vector to calculate</param>
    /// <returns>The mean of the vector's values.</returns>
    private float CalcAverageVector(Vector3 vec) {
        return (vec.x + vec.y + vec.z) / 3;
    }

    /// <summary>
    /// Push the chain to a direction away from a specified position.
    /// </summary>
    /// <param name="source">The source position from which to push the chain</param>
    /// <param name="speed">Speed multiplier to the natural wave speed</param>
    /// <param name="force">Force multiplier to the natural wave speed</param>
    public void PushAwayFrom(Vector3 source, float speed, float force) {
        waveVector = (transform.position - source).normalized;
        stepVector = GetSignVector(waveVector);
        waveSpeed = Random.Range(minWaveSpeed, maxWaveSpeed) * speed;
        currentWaveForce = waveForce * force;
    }
}