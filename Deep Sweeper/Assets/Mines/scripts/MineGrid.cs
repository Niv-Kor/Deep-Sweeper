using Constants;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DetonationSystem))]
[RequireComponent(typeof(SelectionSystem))]
[RequireComponent(typeof(MineActivator))]
[RequireComponent(typeof(LootGeneratorObject))]
public class MineGrid : MonoBehaviour
{
    #region Class Members
    private MineBouncer mine;
    private bool m_tempTarget;
    private int originLayer;
    #endregion

    #region Events
    public event UnityAction MineHitEvent;
    #endregion

    #region Properties
    public GameObject Avatar => mine.gameObject;
    public DetonationSystem DetonationSystem { get; private set; }
    public SelectionSystem SelectionSystem { get; private set; }
    public IndicationSystem IndicationSystem { get; private set; }
    public LootGeneratorObject LootGenerator { get; private set; }
    public MineActivator Activator { get; private set; }
    public MineField Field { get; set; }
    public Vector2Int Position { get; set; }
    public float ExplosiveChance { get; private set; }
    public bool IsMined { get; set; }
    public int Layer {
        get => Avatar.layer;
        set {
            if (!TempTarget) Layers.ApplyLayer(Avatar, value, true);
            originLayer = value;
        }
    }

    public bool TempTarget {
        get => m_tempTarget;
        set {
            if (value != m_tempTarget) {
                m_tempTarget = value;

                if (value) Layers.ApplyLayer(Avatar, Layers.TARGET_MINE, true);
                else Layer = originLayer;
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
        this.IndicationSystem = GetComponentInChildren<IndicationSystem>();
        this.DetonationSystem = GetComponent<DetonationSystem>();
        this.LootGenerator = GetComponent<LootGeneratorObject>();
        this.SelectionSystem = GetComponent<SelectionSystem>();
        this.mine = GetComponentInChildren<MineBouncer>();

        //set this object as a parent grid to its systems
        IndicationSystem.SetParentGrid(this);
        DetonationSystem.SetParentGrid(this);
        SelectionSystem.SetParentGrid(this);

        this.IsMined = false;

        //bind events
        DetonationSystem.MineDisposalEndEvent += Activator.Unlock;
        SelectionSystem.ModeApplicationEvent += ChangeLayer;
    }

    private void Start() {
        //calculate the explosiveness chance every time a change occurs to one of the neighbours
        foreach (MineGrid neighbour in Section) {
            if (neighbour == null) continue;

            neighbour.DetonationSystem.MineDisposalStartEvent += CalcExplosiveChance;
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
    /// <param name="oldMode">The previous mine selection mode</param>
    /// <param name="newMode">The applied mine selection mode</param>
    private void ChangeLayer(SelectionMode oldMode, SelectionMode newMode) {
        bool flagMode = SelectionSystem.IsFlagMode(newMode);
        Layer = flagMode ? Layers.FLAGGED_MINE : Layers.MINE;
    }

    /// <summary>
    /// Explode the mine.
    /// </summary>
    /// <param name="explosion">True to activate an explosion effect on revelation</param>
    /// <param name="ignoreFlagged">True to do nothing if this mine is flagged</param>
    /// <param name="allowDrop">True to allow the mine to drop an item</param>
    private void Detonate(bool explosion, bool ignoreFlagged = false, bool allowDrop = true) {
        bool ignored = SelectionSystem.IsFlagged && ignoreFlagged;
        if (IndicationSystem.IsDisplayed || ignored) return;

        //lose
        if (IsMined) {
            DetonationSystem.Explode();
            DeathTint.Instance.Tint();
            LevelFlow.Instance.Lose();
        }
        else {
            IndicationSystem.AllowRevelation(true);
            IndicationSystem.Activate();
            Activator.ActivateAndLock();
            SelectionSystem.ApplyFlag(false);

            int neighbours = IndicationSystem.Value;
            List<MineGrid> section = Section;

            if (!allowDrop) LootGenerator.Chance = 0;
            if (explosion) DetonationSystem.Explode();
            else DetonationSystem.Vanish();
            MineHitEvent?.Invoke();

            //keep revealing grids recursively
            if (neighbours == 0)
                foreach (MineGrid mineGrid in section)
                    if (mineGrid != null) mineGrid.TriggerHit(BulletHitType.SingleHit, explosion, allowDrop);

            LevelFlow.Instance.TryNextPhase();
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
                Detonate(explosion, true, allowDrop);
                break;

            case BulletHitType.SectionHit:
                List<MineGrid> section = Section;

                //check if section consists if the exact amount of flagged mines
                int flaggedCounter = 0;
                foreach (MineGrid grid in section)
                    if (grid != null && grid.SelectionSystem.IsFlagged) flaggedCounter++;

                //reveal each available grid in the section
                if (flaggedCounter == IndicationSystem.Value)
                    foreach (MineGrid mineGrid in section)
                        if (mineGrid != null) mineGrid.Detonate(explosion, true);

                break;
        }
    }

    /// <summary>
    /// Calculate the chance of this mine being an explosive mine.
    /// </summary>
    public void CalcExplosiveChance() {
        if (DetonationSystem.Detonated) {
            ExplosiveChance = 0;
            return;
        }

        FlagsManager flagsMngr = FlagsManager.Instance;
        List<float> chances = new List<float>();
        float defaultChance = 1f / flagsMngr.AvailableFlags;
        chances.Add(defaultChance);

        foreach (MineGrid neighbour in Section) {
            if (neighbour == null) continue;

            bool dismissed = neighbour.DetonationSystem.Detonated;
            int number = neighbour.IndicationSystem.Value;

            if (dismissed && number > 0) {
                List<MineGrid> neighbourSection = neighbour.Section;
                int emptyGrids = 0;
                int flaggedGrids = 0;

                foreach (MineGrid farNeighbour in neighbourSection) {
                    if (farNeighbour == null) continue;
                    else if (!farNeighbour.DetonationSystem.Detonated) {
                        if (farNeighbour.SelectionSystem.IsFlagged) flaggedGrids++;
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