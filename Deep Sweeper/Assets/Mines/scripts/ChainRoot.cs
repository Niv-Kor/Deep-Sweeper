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
    private float waveSpeed;
    private bool positiveWave;

    private void Start() {
        this.rigidBody = GetComponent<Rigidbody>();
        this.waveVector = GenerateBaseVector();
        this.positiveWave = Random.value > .5f;
        this.waveSpeed = Random.Range(minWaveSpeed, maxWaveSpeed);
    }

    private void Update() {
        float avgVectorVal = CalcAverageVector(waveVector);
        bool limit = (positiveWave && avgVectorVal >= waveForce) || (!positiveWave && avgVectorVal <= -waveForce);

        //regenerate movement
        if (limit) {
            positiveWave = !positiveWave;
            int baseMultiplier = positiveWave ? -1 : 1;
            waveVector = (Vector3.one * waveForce - GenerateBaseVector()) * baseMultiplier;
            waveSpeed = Random.Range(minWaveSpeed, maxWaveSpeed);
        }

        int multiplier = positiveWave ? 1 : -1;
        waveVector += Vector3.one * waveSpeed * multiplier;
        rigidBody.AddForce(waveVector * waveForce);
    }

    private Vector3 GenerateBaseVector() {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        return new Vector3(x, y, z);
    }

    private float CalcAverageVector(Vector3 vec) {
        return (vec.x + vec.y + vec.z) / 3;
    }
}