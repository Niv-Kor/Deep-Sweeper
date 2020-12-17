using Constants;
using UnityEngine;
using UnityEngine.Events;

public class TerrainOutlineRenderer : Singleton<TerrainOutlineRenderer>
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The main terrain of the level.")]
    [SerializeField] private Terrain terrain;

    [Header("Settings")]
    [Tooltip("The percentage of the terrain's height that's rendered in the minimap (from top to bottom).")]
    [SerializeField] [Range(0f, 1f)] private float heightPercent;

    [Tooltip("The material of the terrain's outline.")]
    [SerializeField] private Material terrainMaterial;

    [Tooltip("The material of the space between the outlines.")]
    [SerializeField] private Material backgroundMaterial;
    #endregion

    #region Constants
    private static readonly float ICONS_HEIGHT_PERCENT = 1.1f;
    private static readonly string TERRAIN_COPY_NAME = "Terrain Copy";
    private static readonly string PLANE_NAME = "Plane";
    #endregion

    #region Class Members
    private Terrain terrainCopy;
    private GameObject plane;
    private float planeHeight;
    private float lastHeightSetting;
    #endregion

    #region Events
    public event UnityAction PlaneHeightChangeEvent;
    #endregion

    #region Properties
    public float MinimapIconsHeight {
        get { return planeHeight * ICONS_HEIGHT_PERCENT; }
    }
    #endregion

    private void Awake() {
        InitTerrain();

        this.lastHeightSetting = heightPercent;
        DuplicateTerrain();
        CreateBackgroundPlane();
    }

    private void OnValidate() {
        InitTerrain();

        if (plane != null && heightPercent != lastHeightSetting) {
            SetPlanePosition();
            lastHeightSetting = heightPercent;
        }
    }

    /// <summary>
    /// Duplicate the main terrain in the game.
    /// </summary>
    private void DuplicateTerrain() {
        //safely duplicate a disabled terrain
        terrain.gameObject.SetActive(false);
        terrainCopy = Instantiate(terrain);
        terrain.gameObject.SetActive(true);

        //destroy unnecessary components
        TerrainCollider collider = terrainCopy.GetComponent<TerrainCollider>();
        Destroy(collider);

        //destroy all children objects
        foreach (Transform child in terrainCopy.transform)
            Destroy(child.gameObject);

        //set name and layer
        terrainCopy.name = TERRAIN_COPY_NAME;
        terrainCopy.gameObject.layer = Layers.GetLayerValue(Layers.MINIMAP);

        //locate
        terrainCopy.transform.SetParent(transform);
        terrainCopy.transform.position = terrain.transform.position;
        terrainCopy.transform.rotation = terrain.transform.rotation;

        //reassign material
        MicroSplatTerrain splat = terrainCopy.GetComponent<MicroSplatTerrain>();
        splat.templateMaterial = terrainMaterial;
        terrainCopy.gameObject.SetActive(true);
    }

    /// <summary>
    /// Create a plane that intersects the terrain at a specified height.
    /// </summary>
    private void CreateBackgroundPlane() {
        plane = GameObject.CreatePrimitive(PrimitiveType.Quad);

        //remove collider component
        Collider collider = plane.GetComponent<Collider>();
        Destroy(collider);

        //resize and locate
        Vector3 size = terrain.terrainData.size;
        Quaternion rotation = Quaternion.Euler(Vector3.right * 90);
        plane.transform.rotation = rotation;
        plane.transform.localScale = size;
        SetPlanePosition(size);

        //set plane material
        Renderer renderer = plane.GetComponent<Renderer>();
        renderer.material = backgroundMaterial;

        //set parent and other settings
        plane.transform.SetParent(terrainCopy.transform);
        plane.layer = Layers.GetLayerValue(Layers.MINIMAP);
        plane.name = PLANE_NAME;
    }

    /// <summary>
    /// Set the position of the plane over the terrain.
    /// </summary>
    /// <param name="terrainSize">The size of the terrain</param>
    private void SetPlanePosition(Vector3 terrainSize = default) {
        if (terrainSize == default) terrainSize = terrain.terrainData.size;

        Vector3 position = terrain.transform.position + terrainSize / 2;
        float maxHeight = terrain.terrainData.bounds.max.y;
        planeHeight = maxHeight * (1 - heightPercent);
        position.y = planeHeight;
        plane.transform.position = position;
        PlaneHeightChangeEvent?.Invoke();
    }

    /// <summary>
    /// Find the terrain component in the scene.
    /// </summary>
    private void InitTerrain() {
        if (terrain == null) terrain = FindObjectOfType<Terrain>();
    }
}