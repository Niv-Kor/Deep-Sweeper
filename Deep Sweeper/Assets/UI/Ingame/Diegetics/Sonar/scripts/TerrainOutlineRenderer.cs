using Constants;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.UI.Ingame.Diegetics.Sonar
{
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
        private static readonly float OUTER_GROUNDS_SCALE = 2;
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
        public float MinimapIconsHeight => planeHeight * ICONS_HEIGHT_PERCENT;
        #endregion

        protected override void Awake() {
            base.Awake();
            InitTerrain();

            this.lastHeightSetting = heightPercent;
            DuplicateTerrain();
            CreateInnerGroundsPlane();
            CreateOuterGroundsPlane();
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
            terrainCopy.materialTemplate = terrainMaterial;

            //locate
            terrainCopy.transform.SetParent(transform);
            terrainCopy.transform.position = terrain.transform.position;
            terrainCopy.transform.rotation = terrain.transform.rotation;

            terrainCopy.gameObject.SetActive(true);
        }

        /// <summary>
        /// Create a plane that intersects the terrain at a specified height.
        /// </summary>
        private void CreateInnerGroundsPlane() {
            plane = GameObject.CreatePrimitive(PrimitiveType.Quad);

            //remove collider component
            Collider collider = plane.GetComponent<Collider>();
            Destroy(collider);

            //resize and locate
            Vector3 terrainSize = terrain.terrainData.size;
            Vector3 planeSize = Vector3.Scale(terrainSize, Vector3.one);
            planeSize.y = planeSize.z; //swap y and z
            planeSize.z = 0;
            Quaternion rotation = Quaternion.Euler(Vector3.right * 90);
            plane.transform.rotation = rotation;
            plane.transform.localScale = planeSize;
            SetPlanePosition(terrainSize);

            //set plane material
            Renderer renderer = plane.GetComponent<Renderer>();
            renderer.material = backgroundMaterial;

            //set parent and other settings
            plane.transform.SetParent(terrainCopy.transform);
            plane.layer = Layers.GetLayerValue(Layers.MINIMAP);
            plane.name = "Inner " + PLANE_NAME;
        }

        /// <summary>
        /// Create a plane simulates the ground all around the actual scene.
        /// </summary>
        private void CreateOuterGroundsPlane() {
            GameObject outerPlane = Instantiate(plane);
            outerPlane.transform.SetParent(plane.transform.parent);
            Vector3 yOffset = Vector3.up * plane.transform.localPosition.y / 90f;
            outerPlane.transform.localRotation = plane.transform.localRotation;
            outerPlane.transform.localPosition = plane.transform.localPosition - yOffset;

            //scale
            Vector3 originScale = outerPlane.transform.localScale;
            Vector3 scaleVec = new Vector3(OUTER_GROUNDS_SCALE, OUTER_GROUNDS_SCALE, 1);
            outerPlane.transform.localScale = Vector3.Scale(originScale, scaleVec);

            //set plane material
            Renderer renderer = outerPlane.GetComponent<Renderer>();
            renderer.material = terrainMaterial;

            //set parent and other settings
            outerPlane.layer = Layers.GetLayerValue(Layers.MINIMAP);
            outerPlane.name = "Outer " + PLANE_NAME;
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
}