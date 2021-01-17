using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoatsSpawnController : MonoBehaviour
{
    [System.Serializable]
    private struct BoatSpawnSetting
    {
        [Tooltip("The area from which a boat can randomly spawn.")]
        [SerializeField] public Confine SpawnArea;

        [Tooltip("The closest point on the confined area to the camera.")]
        [SerializeField] public Vector3 ClosestCameraPoint;

        [Tooltip("The furthest point on the confined area from the camera.")]
        [SerializeField] public Vector3 FurthestCameraPoint;

        [Tooltip("The sailing direction of the boats after they spawn.")]
        [SerializeField] public Vector3 BoatDirection;

        [Tooltip("The minimum and maximum possible distances from the camera.")]
        [SerializeField] [HideInInspector] public Vector2 CameraDistancesRange;
    }

    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("An array of boat prefabs to spawn.")]
    [SerializeField] private GameObject[] boatPrefabs;

    [Tooltip("A list of confines from which boats can spawn")]
    [SerializeField] private List<BoatSpawnSetting> areas;

    [Header("Movement")]
    [Tooltip("The minimum and maximum values of the boats' sailing speed.")]
    [SerializeField] private Vector2 sailSpeedRange;

    [Tooltip("The distance after which a boat is destroyed.")]
    [SerializeField] private float boatSailDistance;

    [Header("Spawn Settings")]
    [Tooltip("A point that cannot be seen from the scene's main camera, "
           + "at which all created boats are initially spawned.")]
    [SerializeField] private Vector3 neutralSpawnPoint;

    [Tooltip("The minimum amount of time before which no new boat can spawn.")]
    [SerializeField] private float spawnDelay;

    [Tooltip("Maximum amount of boats that can exist in the scene simultaneously.")]
    [SerializeField] private int maxBoats = 1;

    [Tooltip("The minimum and maximum size of the spawned boats "
           + "(relative to their distance from the camera).")]
    [SerializeField] private Vector2 boatSizeRange;
    #endregion

    #region Constatnts
    private static readonly Color AREA_GIZMO_COLOR = Color.red;
    private static readonly Color CLOSE_CAM_POINT_GIZMO_COLOR = Color.green;
    private static readonly Color FAR_CAM_POINT_GIZMO_COLOR = Color.yellow;
    private static readonly float CAM_POINT_GIZMO_RAD = 3;
    #endregion

    #region Class Members
    private Queue<GameObject> boats;
    #endregion

    #region Properties
    public int MaxBoats {
        get { return maxBoats; }
        set {
            maxBoats = value;
            SetFleet();
        }
    }
    #endregion

    private void Start() {
        this.boats = new Queue<GameObject>();

        //pre calculations
        for (int i = 0; i < areas.Count; i++) {
            BoatSpawnSetting setting = areas[i];

            //calc camera distances range
            Vector3 camPos = Camera.main.transform.position;
            float minCamDist = Vector3.Distance(camPos, setting.ClosestCameraPoint);
            float maxCamDist = Vector3.Distance(camPos, setting.FurthestCameraPoint);
            setting.CameraDistancesRange = new Vector2(minCamDist, maxCamDist);

            areas[i] = setting;
        }

        SetFleet();
        StartCoroutine(RunBoatsSchedule());
    }

    private void OnDrawGizmos() {
        foreach (BoatSpawnSetting setting in areas) {
            //draw confined area
            Gizmos.color = AREA_GIZMO_COLOR;
            Confine confine = setting.SpawnArea;
            Vector3 center = confine.Offset + confine.Size / 2;
            Gizmos.DrawWireCube(center, confine.Size);

            //draw cam points
            Gizmos.color = CLOSE_CAM_POINT_GIZMO_COLOR;
            Gizmos.DrawWireSphere(setting.ClosestCameraPoint, CAM_POINT_GIZMO_RAD);
            Gizmos.color = FAR_CAM_POINT_GIZMO_COLOR;
            Gizmos.DrawWireSphere(setting.FurthestCameraPoint, CAM_POINT_GIZMO_RAD);
        }
    }

    private void SetFleet() {
        int boatsDiff = maxBoats - boats.Count;

        //create boats
        if (boatsDiff > 0)
            for (int i = 0; i < boatsDiff; i++) CreateBoat();
        //remove boats
        else if (boatsDiff < 0)
            for (int i = 0; i < Mathf.Abs(boatsDiff); i++) boats.Dequeue();
    }

    private IEnumerator RunBoatsSchedule() {
        float spawnTimer = spawnDelay;

        while (true) {
            if (spawnTimer >= spawnDelay) {
                if (boats.Count > 0) {
                    spawnTimer = 0;
                    Spawn();
                }
            }
            else spawnTimer += Time.deltaTime;
            yield return null;
        }
    }

    private void CreateBoat() {
        if (boats.Count >= maxBoats) return;

        //randomly select a boat
        int boatIndex = Random.Range(0, boatPrefabs.Length);
        GameObject prefab = boatPrefabs[boatIndex];

        //instantiate
        GameObject instance = Instantiate(prefab);
        instance.transform.SetParent(transform);
        instance.transform.position = neutralSpawnPoint;
        boats.Enqueue(instance);
    }

    private void Spawn() {
        if (boats.Count == 0) return;

        //randomly select a spawn area
        int areaIndex = Random.Range(0, areas.Count);
        BoatSpawnSetting area = areas[areaIndex];

        //generate a random position in the area
        float x = area.SpawnArea.Offset.x + Random.Range(0, area.SpawnArea.Size.x);
        float y = area.SpawnArea.Offset.y + Random.Range(0, area.SpawnArea.Size.y);
        float z = area.SpawnArea.Offset.z + Random.Range(0, area.SpawnArea.Size.z);
        Vector3 pos = new Vector3(x, y, z);

        //set instance config
        GameObject instance = boats.Dequeue();
        instance.transform.position = pos;
        instance.transform.rotation = Quaternion.LookRotation(area.BoatDirection, Vector3.up);
        Vector3 camPos = Camera.main.transform.position;
        float boatCamDist = Vector3.Distance(camPos, instance.transform.position);
        float boatDistPercent = 1 - RangeMath.NumberOfRange(boatCamDist, area.CameraDistancesRange);
        float boatSize = RangeMath.PercentOfRange(boatDistPercent, boatSizeRange);
        instance.transform.localScale = Vector3.one * boatSize;

        //ignite
        StartCoroutine(Sail(instance, area));
    }

    private IEnumerator Sail(GameObject boat, BoatSpawnSetting area) {
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
        boat.transform.position = neutralSpawnPoint;
        boats.Enqueue(boat);
    }
}