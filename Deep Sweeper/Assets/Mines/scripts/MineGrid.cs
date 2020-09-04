using Constants;
using System.Collections.Generic;
using UnityEngine;

public class MineGrid : MonoBehaviour
{
    [Tooltip("Mine head avatar.")]
    [SerializeField] private GameObject avatar;

    private static readonly float EXTERN_RECOIL_SPEED = 30;
    private static readonly float EXTERN_RECOIL_FORCE = 3.5f;

    public delegate void MineHit();
    public event MineHit MineHitEvent;

    private Sweeper sweeper;
    private MineSelector selector;
    private ChainRoot chain;
    private MineClone mineClone;

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
            int row = (int) Position.x;
            int col = (int) Position.y;
            return Field.GetSection(row, col);
        }
    }

    private void Awake() {
        this.Activator = GetComponent<MineActivator>();
        this.MinesIndicator = GetComponentInChildren<Indicator>();
        this.sweeper = GetComponent<Sweeper>();
        this.selector = GetComponent<MineSelector>();
        this.chain = GetComponentInChildren<ChainRoot>();
        this.mineClone = GetComponentInChildren<MineClone>();
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
        mineClone.gameObject.layer = Layers.GetLayerValue(targetLayer);
    }

    /// <summary>
    /// Put a flag on the mine or dispose it.
    /// </summary>
    public void ToggleFlag() { IsFlagged = !IsFlagged; }

    /// <summary>
    /// Explode the mine.
    /// </summary>
    public void Reveal(bool explosion) {
        if (MinesIndicator.IsDisplayed()) return;

        int neighbours = MinesIndicator.MinedNeighbours;
        MinesIndicator.AllowRevelation(true);
        MinesIndicator.gameObject.layer = Layers.GetLayerValue(Layers.MINE_INDICATION);
        IsFlagged = false;

        //disable mine layer and activate mine
        MineClone mineClone = GetComponentInChildren<MineClone>();
        mineClone.gameObject.SetActive(false);
        Activator.ActivateAndLock();

        if (IsMined) {
            ///TODO explode and lose
        }
        else {
            List<MineGrid> section = Section;

            if (explosion) sweeper.Explode();
            else sweeper.Vanish();

            //push away neighbour mines with recoil
            foreach (MineGrid mineGrid in section) {
                if (mineGrid != null) {
                    Vector3 recoilSource = transform.position;
                    recoilSource.y = mineGrid.chain.transform.position.y;
                    mineGrid.chain.PushAwayFrom(recoilSource, EXTERN_RECOIL_SPEED, EXTERN_RECOIL_FORCE);
                }
            }

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