using System.Collections.Generic;
using UnityEngine;

public class MineGrid : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Child object that indicates the amount of mined neighbours.")]
    [SerializeField] private Indicator indicator;

    [Tooltip("Flag cane child object")]
    [SerializeField] private GameObject flagCane;

    private static readonly float EXTERN_RECOIL_SPEED = 30;
    private static readonly float EXTERN_RECOIL_FORCE = 3.5f;

    private Sweeper sweeper;
    private MineFlagger flagger;
    private ChainRoot chain;

    public Indicator MinesIndicator { get { return indicator; } }
    public MineField Field { get; set; }
    public Vector2 Position { get; set; }

    public bool IsMined { get; set; }
    public bool IsFlagged {
        get { return flagger.IsFlagged; }
        set {
            if (value) flagger.Place();
            else flagger.Pull();
        }
    }

    private void Awake() {
        this.sweeper = GetComponentInChildren<Sweeper>();
        this.flagger = GetComponentInChildren<MineFlagger>();
        this.chain = GetComponentInChildren<ChainRoot>();
        this.IsMined = false;
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
        if (MinesIndicator.Enabled) return;
        int neighbours = MinesIndicator.MinedNeighbours;

        if (IsMined) {
            ///TODO explode and lose
        }
        else {
            int row = (int) Position.y;
            int col = (int) Position.x;
            List<MineGrid> section = Field.GetSection(row, col);
            MinesIndicator.Enabled = true;
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
}