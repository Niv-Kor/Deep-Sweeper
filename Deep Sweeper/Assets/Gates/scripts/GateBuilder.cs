using Constants;
using UnityEngine;

namespace DeepSweeper.Level.PhaseGate
{
    public class GateBuilder : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The gate's electrode prefab.")]
        [SerializeField] private GameObject electrode;

        [Tooltip("The parent object of the electrodes.")]
        [SerializeField] private GameObject electrodesParent;

        [Tooltip("The gate's upper force field edge.")]
        [SerializeField] private GameObject upperEdge;

        [Tooltip("The gate's force field.")]
        [SerializeField] private GameObject forceField;

        [Tooltip("The gate's lock emblem.")]
        [SerializeField] private GameObject emblem;

        [Header("Position")]
        [Tooltip("The position of the left gate electrode.")]
        [SerializeField] private Vector3 leftElectrodePos;

        [Tooltip("The position of the right gate electrode.")]
        [SerializeField] private Vector3 rightElectrodePos;

        [Tooltip("The distance of the emblem from the force field.")]
        [SerializeField] private float emblemWallOffset;

        [Tooltip("The distance of the camera from the force field.")]
        [SerializeField] private float camWallOffset;

        [Tooltip("The camera's rotation relative to the gate.")]
        [SerializeField] private Vector3 camRotation;

        [Tooltip("The camera's offset from its fixed position in fron of the gate.")]
        [SerializeField] private Vector3 camOffset;

        [Header("Style")]
        [Tooltip("The emblem's rendered material.")]
        [SerializeField] private Material emblemMaterial;

        [Tooltip("The width of the emblem as a percentage of the gate's full width.")]
        [SerializeField] [Range(.01f, 1f)] private float emblemWidthPercent = .3f;

        [Tooltip("Width (x) length of the emblem divided by its height (y).")]
        [SerializeField] private float emblemRatio = 1;

        [Tooltip("The material of the upper edge laser.")]
        [SerializeField] private Material laserMaterial;

        [Tooltip("The force field's rendererd material.")]
        [SerializeField] private Material forceFieldMaterial;
        #endregion

        #region Constants
        private static readonly float GIZMO_RADIUS = 4;
        private static readonly Color LEFT_GIZMO_COLOR = new Color(0xff, 0xb4, 0x0);
        private static readonly Color RIGHT_GIZMO_COLOR = Color.green;
        private static readonly Color EDGE_GIZMO_COLOR = Color.red;
        private static readonly string ELECTRODES_PARENT_NAME = "Electrodes";
        private static readonly string LEFT_ELECTRODE_NAME = "Left";
        private static readonly string RIGHT_ELECTRODE_NAME = "Right";
        #endregion

        #region Class Members
        private float fieldLength;
        #endregion

        #region Properties
        public GameObject LeftElectrode { get; private set; }
        public GameObject RightElectrode { get; private set; }
        public GameObject ForceFieldEdge { get; private set; }
        public GameObject ForceField { get; private set; }
        public GameObject Emblem { get; private set; }
        public GameObject Camera { get; private set; }
        #endregion

        private void Start() {
            Vector3 forward;
            Vector3 pos = transform.position;
            Vector3 leftPos = pos + leftElectrodePos;
            Vector3 rightPos = pos + rightElectrodePos;
            float dist = Vector3.Distance(leftPos, rightPos);

            BuildElectrodes(leftPos, rightPos);
            forward = Vector3.Cross(Vector3.up, LeftElectrode.transform.forward);
            BuildForceField(dist, forward);
            this.fieldLength = BuildUpperEdge(leftPos);
            BuildEmblem(forward);
            FixCamPosition(forward);

            //initialize the gate component
            Gate gateCmp = GetComponent<Gate>();
            Transform emblemTransform = Emblem.transform;
            GateElectrodes electrodesCmp = GetComponentInChildren<GateElectrodes>();
            ForceField forceField = ForceField.GetComponent<ForceField>();
            electrodesCmp.Initiate(gateCmp);
            gateCmp.Initiate(forceField, emblemTransform);
        }

        private void OnValidate() {
            rightElectrodePos.y = leftElectrodePos.y;
            rightElectrodePos.x = -leftElectrodePos.x;
            rightElectrodePos.z = -leftElectrodePos.z;
        }

        private void OnDrawGizmos() {
            Vector3 pos = transform.position;
            Gizmos.color = LEFT_GIZMO_COLOR;
            Gizmos.DrawWireSphere(pos + leftElectrodePos, GIZMO_RADIUS);
            Gizmos.color = RIGHT_GIZMO_COLOR;
            Gizmos.DrawWireSphere(pos + rightElectrodePos, GIZMO_RADIUS);
            Gizmos.color = EDGE_GIZMO_COLOR;
            Gizmos.DrawLine(pos + leftElectrodePos, pos + rightElectrodePos);
        }

        /// <summary>
        /// Instantiate the electrodes of the gate.
        /// </summary>
        /// <param name="leftPos">The position of the left electrode</param>
        /// <param name="rightPos">The position of the right electrode</param>
        private void BuildElectrodes(Vector3 leftPos, Vector3 rightPos) {
            GameObject parent = Instantiate(electrodesParent);
            parent.name = ELECTRODES_PARENT_NAME;
            parent.layer = Layers.GetLayerValue(Layers.GATE);
            parent.transform.SetParent(transform);

            this.LeftElectrode = Instantiate(electrode);
            LeftElectrode.layer = Layers.GetLayerValue(Layers.GATE);
            LeftElectrode.name = LEFT_ELECTRODE_NAME;
            LeftElectrode.transform.SetParent(parent.transform);
            LeftElectrode.transform.position = leftPos;

            this.RightElectrode = Instantiate(electrode);
            RightElectrode.layer = Layers.GetLayerValue(Layers.GATE);
            RightElectrode.name = RIGHT_ELECTRODE_NAME;
            RightElectrode.transform.SetParent(parent.transform);
            RightElectrode.transform.position = rightPos;

            RightElectrode.transform.LookAt(leftPos, Vector3.up);
            LeftElectrode.transform.LookAt(rightPos, Vector3.up);
        }

        /// <summary>
        /// Instantiate the force field's upper edge.
        /// </summary>
        /// <param name="pos">The position of the edge</param>
        /// <returns>The length of the edge.</returns>
        private float BuildUpperEdge(Vector3 pos) {
            this.ForceFieldEdge = Instantiate(upperEdge);
            ForceFieldEdge.name = ForceFieldEdge.name.Replace("(Clone)", "");
            ForceFieldEdge.layer = Layers.GetLayerValue(Layers.GATE);
            ForceFieldEdge.transform.SetParent(transform);
            ForceFieldEdge.transform.position = pos;

            //set material
            LineRenderer line = ForceFieldEdge.GetComponent<LineRenderer>();
            line.material = laserMaterial;

            //scale
            line.SetPosition(0, LeftElectrode.transform.position);
            line.SetPosition(1, RightElectrode.transform.position);

            //calculate line length
            if (line.positionCount != 2) return 0;
            else {
                Vector3 pointA = line.GetPosition(0);
                Vector3 pointB = line.GetPosition(1);
                return Vector3.Distance(pointA, pointB);
            }
        }

        /// <summary>
        /// Instantiate the gate's force field.
        /// </summary>
        /// <param name="edgeLength">The length of the force field's edge</param>
        /// <param name="forward">The gate's forward direction</param>
        private void BuildForceField(float edgeLength, Vector3 forward) {
            this.ForceField = Instantiate(forceField);
            ForceField.name = ForceField.name.Replace("(Clone)", "");
            ForceField.layer = Layers.GetLayerValue(Layers.GATE);
            ForceField.transform.SetParent(transform);

            //set material
            var particlesRender = ForceField.GetComponent<ParticleSystemRenderer>();
            particlesRender.material = forceFieldMaterial;

            //resize
            BoxCollider collider = ForceField.GetComponent<BoxCollider>();
            Vector3 sizeRatio = collider.bounds.size;
            float height = transform.position.y;
            float xScale = edgeLength / sizeRatio.x;
            float yScale = height / sizeRatio.y;

            //set collider height as the terrain's height
            Terrain terrain = WaterPhysics.Instance.Terrain;
            float terrainHeight = terrain.terrainData.size.y;
            float terrainScale = terrainHeight / yScale;
            collider.size = Vector3.Scale(collider.size, Vector3.right) + Vector3.up * terrainScale + Vector3.forward;

            ForceField.transform.localPosition = Vector3.zero - Vector3.up * height / 2;
            ForceField.transform.localScale = new Vector3(xScale, yScale, 1);
            ForceField.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        /// <summary>
        /// Instantiate the gate's emblem.
        /// </summary>
        /// <param name="forward">The gate's forward direction</param>
        private void BuildEmblem(Vector3 forward) {
            this.Emblem = Instantiate(emblem);
            Emblem.name = Emblem.name.Replace("(Clone)", "");
            Emblem.layer = Layers.GetLayerValue(Layers.GATE);
            Emblem.transform.SetParent(transform);

            //edit sprite
            var particlesRender = Emblem.GetComponent<ParticleSystemRenderer>();
            particlesRender.material = emblemMaterial;
            var particles = Emblem.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule particlesMain = particles.main;
            particlesMain.startSizeX = fieldLength * emblemWidthPercent;
            particlesMain.startSizeY = particlesMain.startSizeX.constant / emblemRatio;

            //position
            Vector3 pos = transform.position;
            Vector3 offset = -Vector3.up * pos.y * .4f + forward * emblemWallOffset;
            Emblem.transform.position = pos + offset;
            Emblem.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        /// <summary>
        /// Fix the camera's position in front of the gate.
        /// </summary>
        /// <param name="forward">The gate's forward direction</param>
        private void FixCamPosition(Vector3 forward) {
            this.Camera = GetComponentInChildren<Camera>().gameObject;

            //fix at position
            Vector3 pos = transform.position;
            Vector3 fixedOffset = -Vector3.up * pos.y * .5f + forward * camWallOffset;
            Camera.transform.position = pos + fixedOffset;
            Camera.transform.localPosition += camOffset;

            //rotate
            Vector3 parallel = Quaternion.LookRotation(-forward, Vector3.up).eulerAngles;
            Vector3 rot = parallel + camRotation;
            Camera.transform.rotation = Quaternion.Euler(rot);
        }
    }
}