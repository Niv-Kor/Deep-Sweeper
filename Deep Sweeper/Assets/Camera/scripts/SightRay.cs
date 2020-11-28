﻿using Constants;
using UnityEngine;

public class SightRay : Singleton<SightRay>
{
    private class MineInfo
    {
        private GameObject avatar;

        public MineGrid Grid { get; private set; }
        public ObjectActivator Activator { get; private set; }
        public Indicator Indicator { get; private set; }
        public MineSelector Selector { get; private set; }

        public MineInfo(GameObject mine) {
            this.avatar = mine;
            this.Grid = mine.GetComponentInParent<MineGrid>();
            this.Activator = Grid.Activator;
            this.Indicator = Grid.MinesIndicator;
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

    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private float maxDistance = 100f;

    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private LayerMask hitLayers;

    private MineInfo selectedMine;
    private MineInfo selectedIndicator;
    private Transform camTransform;
    private SubmarineGun gun;
    private LayerMask mineLayer, indicatorLayer;

    public float MaxDistance { get { return maxDistance; } }
    public float HitDistance { get; private set; }

    private void Start() {
        this.camTransform = CameraManager.Instance.FPCam.transform;
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

        if (mouseLeft) gun.Fire();

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
            Phase phase = GameFlow.Instance.CurrentPhase;
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
                        selectedIndicator.SelectNeighbours(true);
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
    /// Select a mine object.
    /// </summary>
    /// <param name="mine">The object to select</param>
    private MineInfo SelectMine(GameObject mine) {
        MineInfo mineInfo = new MineInfo(mine);
        mineInfo.Activator.ActivateAndLock();
        Crosshair.Instance.Lock();

        print("Explosive Chance: " + mineInfo.Grid.ExplosiveChance);

        return mineInfo;
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