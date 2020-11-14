using UnityEngine;

public class MineMinimapActor : MinimapActor
{
    private void Awake() {
        MineGrid grid = GetComponent<MineGrid>();
        Sweeper sweeper = grid.Sweeper;
        sweeper.MineDisposalEndEvent += delegate { Sprite = null; };
    }
}