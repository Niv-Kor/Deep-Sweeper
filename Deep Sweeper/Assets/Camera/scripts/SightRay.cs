using Constants;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DeepSweeper.Camera;

public class SightRay : Singleton<SightRay>
{
    public class MineInfo {
        #region Class Members
        private GameObject avatar;
        #endregion

        #region Properties
        public MineGrid Grid { get; private set; }
        public ObjectActivator Activator { get; private set; }
        public Indicator Indicator { get; private set; }
        public MineSelector Selector { get; private set; }
        public bool IsValueable {
            get => !Grid.Sweeper.IsDismissed || Indicator.Value > 0;
        }
        #endregion

        public MineInfo(GameObject mine) {
            this.avatar = mine;
            this.Grid = mine.GetComponentInParent<MineGrid>();
            this.Activator = Grid.Activator;
            this.Indicator = Grid.Indicator;
            this.Selector = Grid.GetComponent<MineSelector>();
        }

        /// <summary>
        /// Highlight all neigbours of the selected mine.
        /// </summary>
        /// <param name="flag">True to highlight the neighbours or false to cancel</param>
        public void SelectNeighbours(bool flag) {
            foreach (MineGrid grid in Grid.Section) {
                if (grid == null) continue;

                MineSelector gridSelector = grid.GetComponent<MineSelector>();
                MineActivator gridActivator = grid.GetComponent<MineActivator>();

                //activate or deactivate each of the neighbours
                if (flag) gridActivator.ActivateAndLock();
                else gridActivator.Unlock();

                //apply the correct selection mode
                SelectionMode mode;

                if (grid.IsFlagged) mode = flag ? SelectionMode.FlaggedNeighbourIndication : SelectionMode.Flagged;
                else mode = flag ? SelectionMode.NeighbourIndication : SelectionMode.Default;

                gridSelector.Mode = mode;
            }
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

    #region Exposed Editor Parameters
    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private float maxDistance = 100f;

    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private LayerMask hitLayers;
    #endregion

    #region Class Members
    private MineInfo selectedMine;
    private MineInfo selectedIndicator;
    private Transform camTransform;
    private SubmarineGun gun;
    private LayerMask mineLayer, indicatorLayer;
    #endregion

    #region Properties
    public float MaxDistance => maxDistance;
    public float HitDistance { get; private set; }
    public MineGrid TargetMine {
        get {
            MineInfo mineInfo = selectedMine?? selectedIndicator;
            return mineInfo?.Grid;
        }
    }
    #endregion

    private void Start() {
        this.camTransform = IngameCameraManager.Instance.FPCam.transform;
        this.gun = FindObjectOfType<SubmarineGun>();
        this.mineLayer = Layers.MINE | Layers.FLAGGED_MINE;
        this.indicatorLayer = Layers.MINE_INDICATION;
        this.HitDistance = Mathf.Infinity;
    }

    private void Update() {
        if (CursorViewer.Instance.Display) return;

        CastRay();

        bool mouseRight = Input.GetMouseButtonDown(1);
        bool mouseLeft = Input.GetMouseButtonDown(0);

        if (mouseLeft) Fire();
        if (selectedMine != null) {
            if (mouseRight) selectedMine.Grid.ToggleFlag();
            if (mouseLeft) {
                DeselectMines();
                DeselectIndicators();
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
            GameObject obj = raycastHit.collider.gameObject;
            MineGrid grid = obj.GetComponentInParent<MineGrid>();
            Phase phase = LevelFlow.Instance.CurrentPhase;
            bool allowedGrid = phase != null && phase.Field.ContainsGrid(grid);
            HitDistance = raycastHit.distance;

            //only enable raycasting if the grid is within the current phase.
            if (allowedGrid) {
                //hit a mine
                if (Layers.ContainedInMask(obj.layer, mineLayer)) {
                    DeselectIndicators();

                    bool noMine = selectedMine == null;
                    bool sameMine = !noMine && selectedMine.Equals(obj);

                    if (noMine || !sameMine) {
                        DeselectMines();
                        selectedMine = SelectMine(obj);
                    }
                }
                //hit an indicator
                else if (Layers.ContainedInMask(obj.layer, indicatorLayer)) {
                    DeselectMines();

                    bool noIndicator = selectedIndicator == null;
                    bool sameIndicator = !noIndicator && selectedIndicator.Equals(obj);

                    if (noIndicator || !sameIndicator) {
                        DeselectIndicators();
                        selectedIndicator = SelectMine(obj);
                        selectedIndicator?.SelectNeighbours(true);
                    }
                }
            }
        }
        else {
            HitDistance = Mathf.Infinity;
            DeselectMines();
            DeselectIndicators();
            Crosshair.Instance.Release();
        }
    }
    
    /// <summary>
    /// Fire the submarine gun.
    /// </summary>
    private void Fire() {
        Vector3 upDir = camTransform.up;
        Vector3 pos = camTransform.position;
        Vector3 dir = camTransform.forward;

        //split the bullet to the indicator's neighbours
        if (selectedIndicator != null) {
            //only fire the bullets if the indicator is fulfilled
            if (selectedIndicator.Indicator.IsIndicationFulfilled) {
                IEnumerable<MineGrid> section = from neighbour in selectedIndicator.Grid.Section
                                                where neighbour != null && !neighbour.Sweeper.IsDismissed && !neighbour.IsFlagged
                                                select neighbour;

                //fire a bullet at each of the neighbours
                if (section.Count() > 0) {
                    foreach (MineGrid neighbour in section) {
                        Vector3 neighbourPos = neighbour.Avatar.transform.position;
                        Vector3 neighbourDir = Vector3.Normalize(neighbourPos - pos);
                        gun.Fire(neighbourDir, upDir, false, neighbour, true);
                    }
                }
                else DryFireForward();
            }
            //fire at the indicator itself
            else DryFireForward();
        }
        //only fire at the targeted spot, or rather a selected mine
        else DryFireForward();
    }

    /// <summary>
    /// Fire a bullet forward.
    /// This bullet will not be able to explode a mine.
    /// </summary>
    private void DryFireForward() {
        gun.Fire(camTransform.forward, camTransform.up, true, TargetMine);
    }

    /// <summary>
    /// Select a mine object.
    /// </summary>
    /// <param name="mine">The object to select</param>
    private MineInfo SelectMine(GameObject mine) {
        MineInfo mineInfo = new MineInfo(mine);

        //check that the given mine is not an empty indication
        if (mineInfo.IsValueable) {
            mineInfo.Activator.ActivateAndLock();
            Crosshair.Instance.Lock();
            return mineInfo;
        }
        else return null;
    }

    /// <summary>
    /// Deselect any selected mine.
    /// If no mine is selected, this method does nothing.
    /// </summary>
    private void DeselectMines() {
        if (selectedMine != null) {
            selectedMine.Activator.Unlock();
            selectedMine = null;
        }
    }

    /// <summary>
    /// Deselect any selected indicator.
    /// If no indicator is selected, this method does nothing.
    /// </summary>
    private void DeselectIndicators() {
        if (selectedIndicator != null) {
            selectedIndicator.SelectNeighbours(false);
            selectedIndicator = null;
        }
    }
}