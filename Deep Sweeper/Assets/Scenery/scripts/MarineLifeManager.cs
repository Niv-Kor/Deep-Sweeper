using System.Collections.Generic;
using UnityEngine;

public class MarineLifeManager : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("All creture prefabs whose spawn should be managed.")]
    [SerializeField] private List<GameObject> creaturesPool;

    [Tooltip("The object under which all creatures may spawn.")]
    [SerializeField] private GameObject parentObj;

    private SubmarineDepthControl depthControl;
    private List<MarineLifeSpawner> spawners;

    private void Start() {
        this.depthControl = FindObjectOfType<SubmarineDepthControl>();
        this.spawners = new List<MarineLifeSpawner>();
        Spawn();
    }

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
            MarineLifeSpawner spawner = parentObj.AddComponent<MarineLifeSpawner>();
            spawner.Prefabs = new GameObject[1];
            spawner.Prefabs[0] = creature.gameObject;
            spawner.HeightRange = creature.HeightRange;
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