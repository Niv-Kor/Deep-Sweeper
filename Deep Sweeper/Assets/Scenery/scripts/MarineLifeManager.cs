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
        BordersCreator.Create(this, false, EXCESS_BORDERS_DEPTH, true, borders);

        Spawn();
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