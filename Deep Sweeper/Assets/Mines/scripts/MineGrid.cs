using System.Collections.Generic;
using UnityEngine;

public class MineGrid : MonoBehaviour
{
    private static readonly float EXTERN_RECOIL_SPEED = 30;
    private static readonly float EXTERN_RECOIL_FORCE = 3.5f;

    private Sweeper sweeper;
    private MineSelector selector;
    private ChainRoot chain;

    public Indicator MinesIndicator { get; private set; }
    public MineField Field { get; set; }
    public Vector2 Position { get; set; }

    public bool IsMined { get; set; }
    public bool IsFlagged {
        get { return selector.Mode == SelectionMode.FLAG; }
        set {
            var targetMode = value ? SelectionMode.FLAG : SelectionMode.DEFAULT;
            selector.Mode = targetMode;
        }
    }

    public delegate void MineHit();
    public event MineHit MineHitTrigger;

    private void Awake() {
        this.MinesIndicator = GetComponentInChildren<Indicator>();
        this.sweeper = GetComponentInChildren<Sweeper>();
        this.selector = GetComponentInChildren<MineSelector>();
        this.chain = GetComponentInChildren<ChainRoot>();
        this.IsMined = false;
    }

    private void Start() {
        MineHitTrigger += delegate() { Reveal(true); };
    }

    /// <summary>
    /// Put a flag on the mine or dispose it.
    /// </summary>
    public void ToggleFlag() {
        IsFlagged = !IsFlagged;
    }

    /// <summary>
    /// Explode the mine.
    /// </summary>
    public void Reveal(bool explosion) {
        if (MinesIndicator.IsDisplayed()) return;

        int neighbours = MinesIndicator.MinedNeighbours;
        IsFlagged = false;

        if (IsMined) {
            ///TODO explode and lose
        }
        else {
            int row = (int) Position.y;
            int col = (int) Position.x;
            List<MineGrid> section = Field.GetSection(row, col);
            MinesIndicator.Display(true, !explosion);
            IsFlagged = false;

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
    /// </summary>
    public void TriggerHit() {
        MineHitTrigger?.Invoke();
    }
}