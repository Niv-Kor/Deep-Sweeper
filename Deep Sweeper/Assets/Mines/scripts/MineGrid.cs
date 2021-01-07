using Constants;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Sweeper))]
[RequireComponent(typeof(MineSelector))]
[RequireComponent(typeof(MineActivator))]
[RequireComponent(typeof(LootGeneratorObject))]
public class MineGrid : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("Mine head avatar.")]
    [SerializeField] private GameObject avatar;
    #endregion

    #region Class Members
    private MineBouncer mine;
    #endregion

    #region Events
    public event UnityAction MineHitEvent;
    #endregion

    #region Properties
    public GameObject Avatar { get { return avatar; } }
    public Sweeper Sweeper { get; private set; }
    public MineActivator Activator { get; private set; }
    public MineSelector Selector { get; private set; }
    public LootGeneratorObject LootGenerator { get; private set; }
    public Indicator MinesIndicator { get; private set; }
    public float ExplosiveChance { get; private set; }
    public MineField Field { get; set; }
    public Vector2Int Position { get; set; }
    public bool IsMined { get; set; }
    public bool IsFlagged {
        get {
            bool flagged = Selector.Mode == SelectionMode.Flagged;
            bool indicatedFlagged = Selector.Mode == SelectionMode.FlaggedNeighbourIndication;
            return flagged || indicatedFlagged;
        }
        set {
            if (value != IsFlagged) {
                var targetMode = value ? SelectionMode.Flagged : SelectionMode.Default;
                Selector.Mode = targetMode;
            }
        }
    }

    public List<MineGrid> Section {
        get {
            int row = Position.x;
            int col = Position.y;
            return Field.GetSection(row, col);
        }
    }
    #endregion

    private void Awake() {
        this.Activator = GetComponent<MineActivator>();
        this.MinesIndicator = GetComponentInChildren<Indicator>();
        this.LootGenerator = GetComponent<LootGeneratorObject>();
        this.Sweeper = GetComponent<Sweeper>();
        this.Selector = GetComponent<MineSelector>();
        this.mine = GetComponentInChildren<MineBouncer>();
        this.IsMined = false;

        //bind events
        Sweeper.MineDisposalEndEvent += Activator.Unlock;
        Selector.ModeApplicationHalfwayEvent += ChangeMineLayer;
    }

    private void Start() {
        //calculate the explosiveness chance every time a change occurs to one of the neighbours
        foreach (MineGrid neighbour in Section) {
            if (neighbour == null) continue;

            neighbour.Sweeper.MineDisposalStartEvent += CalcExplosiveChance;
            FlagsManager.Instance.FlagsAmountUpdateEvent += CalcExplosiveChance;
            FlagsManager.Instance.FlagReturnedEvent += delegate(bool _) { CalcExplosiveChance(); };
            FlagsManager.Instance.FlagTakenEvent += delegate(bool _) { CalcExplosiveChance(); };
        }

        CalcExplosiveChance();
    }

    /// <summary>
    /// Change the layer of the mine avatar and mine clone,
    /// according to their flag state.
    /// </summary>
    /// <param name="newMode">The previous mine selection mode</param>
    /// <param name="newMode">The applied mine selection mode</param>
    private void ChangeMineLayer(SelectionMode oldMode, SelectionMode newMode, Material _ = null) {
        LayerMask targetLayer;

        switch (newMode) {
            case SelectionMode.Flagged:
            case SelectionMode.FlaggedNeighbourIndication:
                targetLayer = Layers.FLAGGED_MINE;
                break;

            case SelectionMode.Default:
            case SelectionMode.NeighbourIndication:
                targetLayer = Layers.MINE;
                break;

            default: return;
        }

        avatar.layer = Layers.GetLayerValue(targetLayer);
        mine.gameObject.layer = Layers.GetLayerValue(targetLayer);
    }

    /// <summary>
    /// Put a flag on the mine or dispose it.
    /// </summary>
    public void ToggleFlag() { IsFlagged ^= true; }

    /// <summary>
    /// Explode the mine.
    /// </summary>
    /// <param name="explosion">True to activate an explosion effect on revelation</param>
    /// <param name="ignoreFlagged">True to do nothing if this mine is flagged</param>
    /// <param name="allowDrop">True to allow the mine to drop an item</param>
    private void Reveal(bool explosion, bool ignoreFlagged = false, bool allowDrop = true) {
        bool ignored = IsFlagged && ignoreFlagged;
        if (MinesIndicator.IsDisplayed() || ignored) return;

        if (IsMined) {
            ///TODO explode and lose
        }
        else {
            MinesIndicator.AllowRevelation(true);
            MinesIndicator.Activate();
            Activator.ActivateAndLock();
            IsFlagged = false;

            int neighbours = MinesIndicator.MinedNeighbours;
            List<MineGrid> section = Section;

            if (!allowDrop) LootGenerator.Chance = 0;
            if (explosion) Sweeper.Explode();
            else Sweeper.Vanish();
            MineHitEvent?.Invoke();

            //keep revealing grids recursively
            if (neighbours == 0)
                foreach (MineGrid mineGrid in section)
                    if (mineGrid != null) mineGrid.TriggerHit(BulletHitType.SingleHit, explosion, allowDrop);

            GameFlow.Instance.TryNextPhase();
        }
    }

    /// <summary>
    /// Trigger the mine's hit.
    /// A hit of type 'SingleHit' means that the mine itself has been hit with a bullet,
    /// while a hit type of 'SectionHit' means that the mine's indicator has been hit,
    /// and the bullet is meant for each of the mine's neighbours.
    /// </summary>
    /// <param name="hitType">The type of hit that occured</param>
    /// <param name="explosion">True to activate an explosion effect on revelation</param>
    /// <param name="allowDrop">True to allow the mine to drop an item</param>
    public void TriggerHit(BulletHitType hitType, bool explosion, bool allowDrop = true) {
        switch (hitType) {
            case BulletHitType.SingleHit:
                Reveal(explosion, false, allowDrop);
                break;

            case BulletHitType.SectionHit:
                List<MineGrid> section = Section;

                //check if section consists if the exact amount of flagged mines
                int flaggedCounter = 0;
                foreach (MineGrid mineGrid in section)
                    if (mineGrid != null && mineGrid.IsFlagged) flaggedCounter++;

                //reveal each available grid in the section
                if (flaggedCounter == MinesIndicator.MinedNeighbours)
                    foreach (MineGrid mineGrid in section)
                        if (mineGrid != null) mineGrid.Reveal(explosion, true);

                break;
        }
    }

    /// <summary>
    /// Calculate the chance of this mine being an explosive mine.
    /// </summary>
    public void CalcExplosiveChance() {
        if (Sweeper.IsDismissed) {
            ExplosiveChance = 0;
            return;
        }

        FlagsManager flagsMngr = FlagsManager.Instance;
        List<float> chances = new List<float>();
        float defaultChance = 1f / flagsMngr.AvailableFlags;
        chances.Add(defaultChance);

        foreach (MineGrid neighbour in Section) {
            if (neighbour == null) continue;

            bool dismissed = neighbour.Sweeper.IsDismissed;
            int number = neighbour.MinesIndicator.MinedNeighbours;

            if (dismissed && number > 0) {
                List<MineGrid> neighbourSection = neighbour.Section;
                int emptyGrids = 0;
                int flaggedGrids = 0;

                foreach (MineGrid farNeighbour in neighbourSection) {
                    if (farNeighbour == null) continue;
                    else if (!farNeighbour.Sweeper.IsDismissed) {
                        if (farNeighbour.IsFlagged) flaggedGrids++;
                        else emptyGrids++;
                    }
                }

                int unfulfilled = number - flaggedGrids;
                float neighbourChance = (float) unfulfilled / emptyGrids;
                chances.Add(neighbourChance);
            }
        }

        //find maximum chance
        float maxChance = defaultChance;
        foreach (float chance in chances)
            if (chance > maxChance) maxChance = chance;

        ExplosiveChance = maxChance;
    }
}