using UnityEngine;

public class SightRay : MonoBehaviour
{
    private class MineInfo
    {
        private GameObject avatar;

        public MineGrid Grid { get; private set; }
        public ObjectActivator Activator { get; private set; }

        public MineInfo(GameObject mine) {
            this.avatar = mine;
            this.Activator = mine.GetComponentInParent<MineActivator>();
            this.Grid = mine.GetComponentInParent<MineGrid>();
        }

        /// <summary>
        /// Check if the refrenced object is the same as another object.
        /// </summary>
        /// <param name="other">The object to test</param>
        /// <returns>True if both objects reference the same location in memory.</returns>
        public bool Equals(GameObject other) {
            return avatar == other;
        }
    }

    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private float maxDistance = 100f;

    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private LayerMask hitLayers;

    private MineInfo selectedMine;
    private Transform camTransform;
    private SubmarineGun gun;

    private void Start() {
        this.camTransform = CameraManager.Instance.FPCam.transform;
        this.gun = FindObjectOfType<SubmarineGun>();
    }

    private void Update() {
        CastRay();

        bool mouseRight = Input.GetMouseButtonDown(1);
        bool mouseLeft = Input.GetMouseButtonDown(0);

        if (mouseLeft) gun.Fire();

        if (selectedMine != null) {
            if (mouseRight) selectedMine.Grid.ToggleFlag();
            if (mouseLeft) {
                DeselectAll();
                Crosshair.Instance.Release();
            }
        }
    }

    /// <summary>
    /// Cast a ray at mines to select them.
    /// </summary>
    private void CastRay() {
        Vector3 origin = camTransform.position;
        Vector3 direction = camTransform.forward;
        bool hit = Physics.Raycast(origin, direction, out RaycastHit raycastHit, maxDistance, hitLayers);

        if (hit) {
            GameObject mineObj = raycastHit.collider.gameObject;

            if (selectedMine == null) SelectMine(mineObj);
            else if (!selectedMine.Equals(mineObj)) SelectMine(mineObj);
        }
        else if (selectedMine != null) {
            DeselectAll();
            Crosshair.Instance.Release();
        }
    }

    /// <summary>
    /// Select a mine object.
    /// </summary>
    /// <param name="mine">The object to select</param>
    private void SelectMine(GameObject mine) {
        selectedMine = new MineInfo(mine);
        selectedMine.Activator.ActivateAndLock(true);
        Crosshair.Instance.Lock();
    }

    /// <summary>
    /// Deselect any selected mine.
    /// If no mine is selected, this method does nothing.
    /// </summary>
    private void DeselectAll() {
        if (selectedMine != null) {
            selectedMine.Activator.Unlock();
            selectedMine = null;
        }
    }
}
