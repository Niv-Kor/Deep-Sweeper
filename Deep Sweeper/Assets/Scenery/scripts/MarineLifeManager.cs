using System.Collections.Generic;
using UnityEngine;

public class MarineLifeManager : ConfinedArea
{
    [Header("Prefabs")]
    [Tooltip("All creture prefabs whose spawn should be managed.")]
    [SerializeField] private List<GameObject> creaturesPool;

    [Header("Borders")]
    [Tooltip("The layer mask of the fish borders (must be one layered).")]
    [SerializeField] private LayerMask borderLayer;

    private static readonly string BORDERS_OBJ_NAME = "Borders";
    private static readonly string FISH_OBJ_NAME = "Fish";
    private static readonly float EXCESS_BORDERS_DEPTH = 20f;

    private SubmarineDepthControl depthControl;
    private List<MarineLifeSpawner> spawners;
    private GameObject fishParentObj;

    private void Start() {
        this.depthControl = FindObjectOfType<SubmarineDepthControl>();
        this.spawners = new List<MarineLifeSpawner>();
        this.fishParentObj = new GameObject(FISH_OBJ_NAME);
        fishParentObj.transform.SetParent(transform);

        //create a box collider for each confine edge
        GameObject borders = new GameObject(BORDERS_OBJ_NAME);
        borders.layer = Constants.Layers.GetLayerValue(borderLayer);
        borders.transform.SetParent(transform);
        CreateBorders(borders);

        Spawn();
    }

    /// <summary>
    /// Create box colliders on the borders child object.
    /// </summary>
    /// <param name="parentObj">
    /// The borders object to which the
    /// box collider components will be added
    /// </param>
    private void CreateBorders(GameObject parentObj) {
        Vector3[] boxSizes = {
            new Vector3(Confine.Size.x, Confine.Size.y, EXCESS_BORDERS_DEPTH),
            new Vector3(EXCESS_BORDERS_DEPTH, Confine.Size.y, Confine.Size.z),
            new Vector3(Confine.Size.x, Confine.Size.y, EXCESS_BORDERS_DEPTH),
            new Vector3(EXCESS_BORDERS_DEPTH, Confine.Size.y, Confine.Size.z)
        };

        Vector3[] centerPointsOffset = {
            new Vector3(0, 0, -boxSizes[0].z),
            new Vector3(-boxSizes[1].x, 0, 0),
            new Vector3(0, 0, Confine.Size.z),
            new Vector3(Confine.Size.x, 0, 0)
        };

        for (int i = 0; i < boxSizes.Length; i++) {
            Vector3 size = boxSizes[i];
            Vector3 centerOffset = centerPointsOffset[i];
            BoxCollider edge = parentObj.AddComponent<BoxCollider>();
            edge.size = size;
            edge.center = Confine.Offset + size / 2 + centerOffset;
            edge.isTrigger = true;
        }
    }

    /// <summary>
    /// Create a MarineLifeSpawner script for each of the prefabs in the pool.
    /// </summary>
    private void Spawn() {
        float depth = depthControl.Depth;
        var pool = new List<MarineLife>();

        //find all available prefabs according to the current depth
        foreach (GameObject prefab in creaturesPool) {
            MarineLife marineLife = prefab.GetComponent<MarineLife>();
            if (marineLife.DepthHabitat <= depth) pool.Add(marineLife);
        }

        //create a seperate script for each prefab
        foreach (MarineLife creature in pool) {
            MarineLifeSpawner spawner = fishParentObj.AddComponent<MarineLifeSpawner>();
            spawner.Prefabs = new GameObject[1];
            spawner.Prefabs[0] = creature.gameObject;
            spawner.Confine = Confine;
            spawner.SpreadRange = creature.PacksAmountRange;
            spawner.VolumeRange = creature.PackSizeRange;
            spawner.MinIndividualDistance = creature.MinIndividualDistance;
            spawner.MaxIndividualDistance = creature.MaxIndividualDistance;
            spawner.DepthAffection = creature.DepthAffection;
            spawner.TurbulenceAffection = creature.TurbulenceAffection;
            spawners.Add(spawner);
        }
    }
}