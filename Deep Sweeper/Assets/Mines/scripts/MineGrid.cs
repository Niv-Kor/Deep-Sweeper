using Constants;
using System.Collections.Generic;
using UnityEngine;

public class MineGrid : MonoBehaviour
{
    [Tooltip("Mine head avatar.")]
    [SerializeField] private GameObject avatar;

    public delegate void MineHit();
    public event MineHit MineHitEvent;

    private Sweeper sweeper;
    private MineSelector selector;
    private MineBouncer mine;

    public MineActivator Activator { get; private set; }
    public Indicator MinesIndicator { get; private set; }
    public MineField Field { get; set; }
    public Vector2Int Position { get; set; }
    public bool IsMined { get; set; }

    public bool IsFlagged {
        get {
            bool flagged = selector.Mode == SelectionMode.Flagged;
            bool indicatedFlagged = selector.Mode == SelectionMode.FlaggedNeighbourIndication;
            return flagged || indicatedFlagged;
        }
        set {
            if (value != IsFlagged) {
                var targetMode = value ? SelectionMode.Flagged : SelectionMode.Default;
                selector.Mode = targetMode;
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

    private void Awake() {
        this.Activator = GetComponent<MineActivator>();
        this.MinesIndicator = GetComponentInChildren<Indicator>();
        this.sweeper = GetComponent<Sweeper>();
        this.selector = GetComponent<MineSelector>();
        this.mine = GetComponentInChildren<MineBouncer>();
        this.IsMined = false;

        //bind events
        MineHitEvent += delegate { Reveal(true); };
        sweeper.MineDisposalEndEvent += Activator.Unlock;
        selector.ModeApplicationEvent += ChangeMineLayer;
    }

    /// <summary>
    /// Change the layer of the mine avatar and mine clone,
    /// according to their flag state.
    /// </summary>
    /// <param name="mode">The next selection mode of the mine</param>
    private void ChangeMineLayer(SelectionMode mode, Material _ = null) {
        LayerMask targetLayer;

        switch (mode) {
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
    public void Reveal(bool explosion) {
        if (MinesIndicator.IsDisplayed()) return;

        MinesIndicator.AllowRevelation(true);
        MinesIndicator.gameObject.layer = Layers.GetLayerValue(Layers.MINE_INDICATION);
        Activator.ActivateAndLock();
        IsFlagged = false;

        if (IsMined) {
            ///TODO explode and lose
        }
        else {
            int neighbours = MinesIndicator.MinedNeighbours;
            List<MineGrid> section = Section;

            if (explosion) sweeper.Explode();
            else sweeper.Vanish();

            //keep revealing grids recursively
            if (neighbours == 0)
                foreach (MineGrid mineGrid in section)
                    if (mineGrid != null) mineGrid.Reveal(explosion);
        }
    }

    /// <summary>
    /// Trigger the mine's hit.
    /// A hit of type 'SingleHit' means that the mine itself has been hit with a bullet,
    /// while a hit type of 'SectionHit' means that the mine's indicator has been hit,
    /// and the bullet is meant for each of the mine's neighbours.
    /// </summary>
    /// <param name="hitType">The type of hit that occured</param>
    public void TriggerHit(BulletHitType hitType) {
        MineHitEvent?.Invoke();
    }
}