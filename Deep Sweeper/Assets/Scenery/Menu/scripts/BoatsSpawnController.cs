using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoatsSpawnController : MonoBehaviour
{
    [System.Serializable]
    private struct BoartSpawnSetting
    {
        [Tooltip("The area from which a boat can randomly spawn.")]
        [SerializeField] public Confine SpawnArea;

        [Tooltip("The sailing direction of the boats after they spawn.")]
        [SerializeField] public Vector3 BoatDirection;
    }

    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("An array of boat prefabs to spawn.")]
    [SerializeField] private GameObject[] boatPrefabs;

    [Tooltip("A list of confines from which boats can spawn")]
    [SerializeField] private List<BoartSpawnSetting> areas;

    [Header("Movement")]
    [Tooltip("The minimum and maximum values of the boats' sailing speed.")]
    [SerializeField] private Vector2 sailSpeedRange;

    [Tooltip("The distance after which a boat is destroyed.")]
    [SerializeField] private float boatSailDistance;

    [Header("Spawn Settings")]
    [Tooltip("The minimum amount of time before which no new boat can spawn.")]
    [SerializeField] private float spawnDelay;

    [Tooltip("Maximum amount of boats that can exist in the scene simultaneously.")]
    [SerializeField] private int maxBoats = 1;
    #endregion

    #region Constatnts
    private static readonly Color AREA_GIZMO_COLOR = Color.red;
    #endregion

    #region Class Members
    private List<GameObject> boats;
    private float spawnTimer;
    private List<Confine> areaConfines;
    #endregion

    private void Start() {
        this.boats = new List<GameObject>();
        this.spawnTimer = spawnDelay;
    }

    private void OnValidate() {
        areaConfines = (from BoartSpawnSetting setting in areas
                        select setting.SpawnArea).ToList();
    }

    private void Update() {
        if (spawnTimer >= spawnDelay) {
            if (boats.Count < maxBoats) {
                Spawn();
                spawnTimer = 0;
            }
        }
        else spawnTimer += Time.deltaTime;
    }

    private void OnDrawGizmos() {
        Gizmos.color = AREA_GIZMO_COLOR;

        foreach (Confine confine in areaConfines) {
            Vector3 center = confine.Offset + confine.Size / 2;
            Gizmos.DrawWireCube(center, confine.Size);
        }
    }

    private void Spawn() {
        //randomly select a boat and a spawn area
        int boatIndex = Random.Range(0, boatPrefabs.Length);
        int areaIndex = Random.Range(0, areas.Count);
        GameObject prefab = boatPrefabs[boatIndex];
        BoartSpawnSetting area = areas[areaIndex];

        //generate a random position in the area
        float x = area.SpawnArea.Offset.x + Random.Range(0, area.SpawnArea.Size.x);
        float y = area.SpawnArea.Offset.y + Random.Range(0, area.SpawnArea.Size.y);
        float z = area.SpawnArea.Offset.z + Random.Range(0, area.SpawnArea.Size.z);
        Vector3 pos = new Vector3(x, y, z);

        //instantiate and ignite
        GameObject instance = Instantiate(prefab);
        instance.transform.SetParent(transform);
        instance.transform.position = pos;
        instance.transform.rotation = Quaternion.LookRotation(area.BoatDirection, Vector3.up);
        boats.Add(instance);
        StartCoroutine(Sail(instance, area));
    }

    private IEnumerator Sail(GameObject boat, BoartSpawnSetting area) {
        float speed = Random.Range(sailSpeedRange.x, sailSpeedRange.y);
        Vector3 targetPos = area.BoatDirection * boatSailDistance;
        float dist = 0;

        while (dist < boatSailDistance) {
            Vector3 startPos = boat.transform.position;
            float sailSpeed = Time.deltaTime * speed;
            boat.transform.position = Vector3.Lerp(startPos, targetPos, sailSpeed);
            Vector3 endPos = boat.transform.position;
            dist += Vector3.Distance(startPos, endPos);
            yield return null;
        }

        //destroy boat
        boats.Remove(boat);
        Destroy(boat);
    }
}