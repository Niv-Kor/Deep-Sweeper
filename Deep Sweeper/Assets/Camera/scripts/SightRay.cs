using Constants;
using DeepSweeper.Camera;
using UnityEngine;
using UnityEngine.Events;

public class SightRay : Singleton<SightRay>
{
    public enum SightTargetType
    {
        None,
        Mine,
        Indicator
    }

    public class TargetInfo
    {
        #region Class Members
        private GameObject avatar;
        #endregion

        #region Properties
        public MineGrid Grid { get; private set; }
        public ObjectActivator Activator { get; private set; }
        public IndicationSystem Indicator { get; private set; }
        public SelectionSystem Selector { get; private set; }
        public bool IsValueable {
            get => !Grid.DetonationSystem.IsDetonated || Indicator.Value > 0;
        }
        #endregion

        public TargetInfo(GameObject mine) {
            this.avatar = mine;
            this.Grid = mine.GetComponentInParent<MineGrid>();
            this.Activator = Grid.Activator;
            this.Indicator = Grid.IndicationSystem;
            this.Selector = Grid.SelectionSystem;
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
    private TargetInfo selectedMine;
    private TargetInfo selectedIndicator;
    private SubmarineOrientation submarine;
    private PlayerController controller;
    private LevelFlow levelFlow;
    private LayerMask mineLayer, indicatorLayer;
    #endregion

    #region Events
    /// <param type=typeof(SightTargetType)>The type of locked target</param>
    /// <param type=typeof(TargetInfo)>The locked target (or null if it doesn't exist</param>
    public event UnityAction<SightTargetType, TargetInfo> PrimaryHitEvent;

    /// <param type=typeof(SightTargetType)>The type of locked target</param>
    /// <param type=typeof(TargetInfo)>The locked target (or null if it doesn't exist</param>
    public event UnityAction<SightTargetType, TargetInfo> SecondaryHitEvent;
    #endregion

    #region Properties
    public float MaxDistance => maxDistance;
    public float HitDistance { get; private set; }
    public TargetInfo Target => selectedMine?? selectedIndicator;
    public MineGrid TargetGrid => Target?.Grid;
    public SightTargetType TargetType {
        get {
            if (selectedMine != null) return SightTargetType.Mine;
            else if (selectedIndicator != null) return SightTargetType.Indicator;
            else return SightTargetType.None;
        }
    }
    #endregion

    private void Start() {
        this.controller = PlayerController.Instance;
        this.levelFlow = LevelFlow.Instance;
        this.submarine = Submarine.Instance.Oriantation;
        this.mineLayer = Layers.MINE | Layers.FLAGGED_MINE;
        this.indicatorLayer = Layers.MINE_INDICATION;
        this.HitDistance = Mathf.Infinity;

        controller.PrimaryOperationEvent += OnPrimaryOperationClick;
        controller.SecondaryOperationEvent += OnSecondaryOperationClick;
    }

    private void Update() {
        if (IngameCameraManager.Instance.FPCam.IsDisplaying) CastRay();
    }

    /// <summary>
    /// Activate when the primary operation key is pressed.
    /// This function fires a torpedo.
    /// </summary>
    private void OnPrimaryOperationClick() {
        if (!CursorViewer.Instance.IsDisplayed) PrimaryHitEvent?.Invoke(TargetType, Target);

        //release target
        if (TargetType == SightTargetType.Mine) {
            DeselectMines();
            DeselectIndicators();
            Crosshair.Instance.Release();
        }
    }

    /// <summary>
    /// Activate when the secondary operation key is pressed.
    /// If a mine is in sight range, it will toggle its flag state.
    /// </summary>
    private void OnSecondaryOperationClick() {
        SecondaryHitEvent?.Invoke(TargetType, Target);
    }

    /// <summary>
    /// Cast a ray at mines to select them.
    /// </summary>
    private void CastRay() {
        bool hit = Physics.Raycast(submarine.Position, submarine.Forward, out RaycastHit raycastHit, maxDistance, hitLayers);

        if (hit) {
            GameObject obj = raycastHit.collider.gameObject;
            MineGrid grid = obj.GetComponentInParent<MineGrid>();
            Phase phase = levelFlow.CurrentPhase;
            bool allowedGrid = levelFlow.DuringPhase && phase.Field.ContainsGrid(grid);
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
                        selectedIndicator?.Selector.SelectSection(true);
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
    private TargetInfo SelectMine(GameObject mine) {
        TargetInfo mineInfo = new TargetInfo(mine);

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
            selectedIndicator.Selector.SelectSection(false);
            selectedIndicator = null;
        }
    }
}