using UnityEngine.Events;

public class CoinGeneratorObject : LootGeneratorObject
{
    /// <inheritdoc/>
    protected override void TakeEffect(int collectingLayer) {
        print("collected coin");
    }

    /// <inheritdoc/>
    protected override void BindDropEvent(UnityAction dropAction) {
        MineGrid grid = GetComponentInParent<MineGrid>();
        grid.Sweeper.MineDisposalStartEvent += dropAction;
    }
}