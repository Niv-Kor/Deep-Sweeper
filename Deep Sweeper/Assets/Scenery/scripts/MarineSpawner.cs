using UnityEngine;

public class MarineSpawner : MonoBehaviour
{
    private enum SpawnAffection
    {
        More,
        Less,
        NoEffect
    }

    [Header("Prefabs")]
    [Tooltip("The particle system prefabs to instantiate.")]
    [SerializeField] private GameObject[] prefabs;

    [Tooltip("The name of the particle systems' parent object.")]
    [SerializeField] private string parentObjectName;

    [Header("Settings")]
    [Tooltip("The minimum (inclusive) and maximum (exclusive) value of a particle system's height.")]
    [SerializeField] private Vector2 heightRange;

    [Tooltip("Amount of particle systems to spread across the terrain.")]
    [SerializeField] private Vector2Int spreadRange;

    [Tooltip("The minimum (inclusive) and maximum (exclusive) value of a particle system's volume.")]
    [SerializeField] private Vector2Int volumeRange;

    [Header("Spawn Affection")]
    [Tooltip("Affection of the ground level's depth.")]
    [SerializeField] private SpawnAffection depthAffection;

    [Tooltip("Affection of the waves' turbulence.")]
    [SerializeField] private SpawnAffection turbulenceAffection;

    private GameObject parent;
    private int spread, emission;

    private void Start() {
        this.parent = new GameObject(parentObjectName);
        parent.transform.SetParent(transform);

        Vector2Int environmentAffection = CalcSpreadAndEmission();
        this.spread = environmentAffection.x;
        this.emission = environmentAffection.y;
        Spawn(spread);
    }

    /// <summary>
    /// Randomly spawn particle systems across the terrain.
    /// </summary>
    /// <param name="amount">Amout of systems to spawn</param>
    private void Spawn(int amount) {
        Terrain terrainComponent = GetComponentInParent<Terrain>();
        Vector3 terrainDim = terrainComponent.terrainData.size;
        Vector3 terrainPos = transform.position;

        //find correct level emission
        var emissionCurve = new ParticleSystem.MinMaxCurve(emission);

        //instantiate
        for (int i = 0; i < amount; i++) {
            int prefabIndex = Random.Range(0, prefabs.Length);
            GameObject instance = Instantiate(prefabs[prefabIndex]);
            float x = Random.Range(terrainPos.x, terrainDim.x);
            float y = Random.Range(heightRange.x, heightRange.y);
            float z = Random.Range(terrainPos.z, terrainDim.z);
            instance.transform.position = new Vector3(x, y, z);
            instance.transform.SetParent(parent.transform);

            //set emission according to the waves' intensity
            ParticleSystem particles = instance.GetComponent<ParticleSystem>();
            ParticleSystem.EmissionModule module = particles.emission;
            module.rateOverTime = emissionCurve;
        }
    }

    /// <summary>
    /// Calculate the correct amount of particles spread and emission
    /// according to the affection of the environment.
    /// Spread is the amount of particle systems around the map,
    /// and emission is the amount of particles emitted in each system.
    /// </summary>
    /// <returns>
    /// A Vector2 object with the spread value as X,
    /// and the emission value as Y.
    /// </returns>
    private Vector2Int CalcSpreadAndEmission() {
        float intensPercent = Waves.Instance.IntensityPercentage;
        int spreadDiff = spreadRange.y - spreadRange.x;
        int volumeDiff = volumeRange.y - volumeRange.x;
        int avgSpread = (spreadRange.x + spreadRange.y) / 2;
        int avgVolume = (volumeRange.x + volumeRange.y) / 2;
        int spreadByDepth, spreadByTurbulence;
        int emissionByDepth, emissionByTurbulence;

        //calculate depth affection
        switch (depthAffection) {
            case SpawnAffection.More:
                spreadByDepth = avgSpread;
                emissionByDepth = avgVolume;
                break;

            case SpawnAffection.Less:
                spreadByDepth = avgSpread;
                emissionByDepth = avgVolume;
                break;

            case SpawnAffection.NoEffect:
                spreadByDepth = avgSpread;
                emissionByDepth = avgVolume;
                break;

            default:
                spreadByDepth = spreadRange.x;
                emissionByDepth = volumeRange.x;
                break;
        }

        //calculate turbulence affection
        switch (turbulenceAffection) {
            case SpawnAffection.More:
                spreadByTurbulence = (int) (intensPercent * spreadDiff / 100f + spreadRange.x);
                emissionByTurbulence = (int) (intensPercent * volumeDiff / 100f + volumeRange.x);
                break;

            case SpawnAffection.Less:
                float intense = 100 - intensPercent;
                spreadByTurbulence = (int) (intense * spreadDiff / 100f + spreadRange.x);
                emissionByTurbulence = (int) (intense * volumeDiff / 100f + volumeRange.x);
                break;

            case SpawnAffection.NoEffect:
                spreadByTurbulence = avgSpread;
                emissionByTurbulence = avgVolume;
                break;

            default:
                spreadByTurbulence = spreadRange.x;
                emissionByTurbulence = volumeRange.x;
                break;
        }

        int spread = (spreadByDepth + spreadByTurbulence) / 2;
        int emission = (emissionByDepth + emissionByTurbulence) / 2;
        return new Vector2Int(spread, emission);
    }
}