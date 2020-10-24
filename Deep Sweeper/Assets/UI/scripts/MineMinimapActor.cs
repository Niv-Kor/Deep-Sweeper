using UnityEngine;

public class MineMinimapActor : MinimapActor
{
    [SerializeField] private Sprite altSprite;

    private void Awake() {
        MineGrid grid = GetComponent<MineGrid>();
        Sweeper sweeper = grid.Sweeper;
        sweeper.MineDisposalEndEvent += delegate { Sprite = null; };
    }
}