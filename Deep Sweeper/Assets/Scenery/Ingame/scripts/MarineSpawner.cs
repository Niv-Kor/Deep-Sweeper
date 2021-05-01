using UnityEngine;

public abstract class MarineSpawner : ConfinedArea
{
    #region Exposed Editor Parameters
    [Header("Settings")]
    [Tooltip("The particle system prefabs to instantiate.")]
    [SerializeField] public GameObject[] Prefabs;

    [Tooltip("The name of the particle systems' parent object.")]
    [SerializeField] protected string parentObjectName;

    [Header("Settings")]
    [Tooltip("Amount of spawns to spread across the terrain.")]
    [SerializeField] public Vector2Int SpreadRange;

    [Tooltip("The minimum (inclusive) and maximum (exclusive) value of a spawn's volume.")]
    [SerializeField] public Vector2Int VolumeRange;

    [Header("Spawn Affection")]
    [Tooltip("Affection of the ground level's depth.")]
    [SerializeField] public SpawnAffection DepthAffection;

    [Tooltip("Affection of the waves' turbulence.")]
    [SerializeField] public SpawnAffection TurbulenceAffection;
    #endregion

    #region Class Members
    protected GameObject parent;
    protected int spread, emission;
    #endregion

    protected virtual void Start() {
        if (!string.IsNullOrEmpty(parentObjectName)) {
            GameObject existingParent = transform.Find(parentObjectName)?.gameObject;
            this.parent = existingParent ?? new GameObject(parentObjectName);
        }
        else this.parent = gameObject;
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
            instance.transform.SetParent(parent.transform);
            instance.transform.position = GeneratePosition();
            ApplyEmission(instance, emission);
        }
    }

    /// <summary>
    /// Instantiate a random prefab from the list.
    /// </summary>
    /// <returns>An instance of a randomly selected prefab.</returns>
    protected virtual GameObject RandomlyInstantiate() {
        if (Prefabs.Length > 0) {
            int prefabIndex = Random.Range(0, Prefabs.Length);
            return Instantiate(Prefabs[prefabIndex]);
        }
        else return null;
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
        int spreadDiff = SpreadRange.y - SpreadRange.x;
        int volumeDiff = VolumeRange.y - VolumeRange.x;
        int avgSpread = (SpreadRange.x + SpreadRange.y) / 2;
        int avgVolume = (VolumeRange.x + VolumeRange.y) / 2;
        int spreadByDepth, spreadByTurbulence;
        int emissionByDepth, emissionByTurbulence;

        //calculate depth affection
        switch (DepthAffection) {
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
                spreadByDepth = SpreadRange.x;
                emissionByDepth = VolumeRange.x;
                break;
        }

        //calculate turbulence affection
        switch (TurbulenceAffection) {
            case SpawnAffection.More:
                spreadByTurbulence = (int) (intensPercent * spreadDiff / 100f + SpreadRange.x);
                emissionByTurbulence = (int) (intensPercent * volumeDiff / 100f + VolumeRange.x);
                break;

            case SpawnAffection.Less:
                float intense = 100 - intensPercent;
                spreadByTurbulence = (int) (intense * spreadDiff / 100f + SpreadRange.x);
                emissionByTurbulence = (int) (intense * volumeDiff / 100f + VolumeRange.x);
                break;

            case SpawnAffection.NoEffect:
                spreadByTurbulence = avgSpread;
                emissionByTurbulence = avgVolume;
                break;

            default:
                spreadByTurbulence = SpreadRange.x;
                emissionByTurbulence = VolumeRange.x;
                break;
        }

        int spread = (spreadByDepth + spreadByTurbulence) / 2;
        int emission = (emissionByDepth + emissionByTurbulence) / 2;
        return new Vector2Int(spread, emission);
    }
}