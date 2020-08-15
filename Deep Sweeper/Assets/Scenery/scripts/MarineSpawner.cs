using UnityEngine;

public abstract class MarineSpawner : MonoBehaviour
{
    protected enum SpawnAffection
    {
        More,
        Less,
        NoEffect
    }

    [Header("Prefabs")]
    [Tooltip("The particle system prefabs to instantiate.")]
    [SerializeField] protected GameObject[] prefabs;

    [Tooltip("The name of the particle systems' parent object.")]
    [SerializeField] protected string parentObjectName;

    [Header("Settings")]
    [Tooltip("The minimum (inclusive) and maximum (exclusive) value of a particle system's height.")]
    [SerializeField] protected Vector2 heightRange;

    [Tooltip("Amount of particle systems to spread across the terrain.")]
    [SerializeField] protected Vector2Int spreadRange;

    [Tooltip("The minimum (inclusive) and maximum (exclusive) value of a particle system's volume.")]
    [SerializeField] protected Vector2Int volumeRange;

    [Header("Spawn Affection")]
    [Tooltip("Affection of the ground level's depth.")]
    [SerializeField] protected SpawnAffection depthAffection;

    [Tooltip("Affection of the waves' turbulence.")]
    [SerializeField] protected SpawnAffection turbulenceAffection;

    protected Terrain terrain;
    protected GameObject parent;
    protected Vector3 terrainPos;
    protected Vector3 terrainDim;
    protected int spread, emission;

    protected virtual void Start() {
        this.terrain = GetComponentInParent<Terrain>();
        this.terrainDim = terrain.terrainData.size;
        this.terrainPos = transform.position;
        this.parent = new GameObject(parentObjectName);
        parent.transform.SetParent(transform);

        Vector2Int environmentAffection = CalcVolume();
        this.spread = environmentAffection.x;
        this.emission = environmentAffection.y;
        Spawn(spread);
    }

    /// <summary>
    /// Randomly spawn particle systems across the terrain.
    /// </summary>
    /// <param name="amount">Amout of systems to spawn</param>
    protected virtual void Spawn(int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject instance = RandomlyInstantiate();
            instance.transform.position = RandomizePosition();
            instance.transform.SetParent(parent.transform);
            ApplyEmission(instance, emission);
        }
    }

    /// <summary>
    /// Instantiate a random prefab from the list.
    /// </summary>
    /// <returns>An instance of a randomly selected prefab.</returns>
    protected virtual GameObject RandomlyInstantiate() {
        if (prefabs.Length > 0) {
            int prefabIndex = Random.Range(0, prefabs.Length);
            return Instantiate(prefabs[prefabIndex]);
        }
        else return null;
    }

    /// <returns>A random position across the terrain</returns>
    protected virtual Vector3 RandomizePosition() {
        float x = Random.Range(terrainPos.x, terrainDim.x);
        float y = Random.Range(heightRange.x, heightRange.y);
        float z = Random.Range(terrainPos.z, terrainDim.z);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Apply emission level on a newly spawned instance.
    /// </summary>
    /// <param name="instance">The instance on which the emission should be applied</param>
    /// <param name="emission">Amount of emission to apply</param>
    protected abstract void ApplyEmission(GameObject instance, int emission);

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
    protected virtual Vector2Int CalcVolume() {
        float intensPercent = WaterPhysics.Instance.IntensityPercentage;
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