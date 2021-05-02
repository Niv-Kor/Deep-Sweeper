using System.Collections.Generic;
using UnityEngine.Events;

public class CoinGeneratorObject : LootGeneratorObject
{
    #region Constants
    private static readonly int SILVER_THRESHOLD = 250;
    private static readonly int GOLD_THRESHOLD = 1000;
    private static readonly string BRONZE_COIN_NAME = "bronze_coin";
    private static readonly string SILVER_COIN_NAME = "silver_coin";
    private static readonly string GOLD_COIN_NAME = "gold_coin";
    #endregion

    /// <inheritdoc/>
    protected override LootItem SelectItem(List<LootItem> items) {
        string coinName;

        if (ItemValue < SILVER_THRESHOLD) coinName = BRONZE_COIN_NAME;
        else if (ItemValue < GOLD_THRESHOLD) coinName = SILVER_COIN_NAME;
        else coinName = GOLD_COIN_NAME;

        return items.Find(x => x.ItemName == coinName);
    }

    /// <inheritdoc/>
    protected override void TakeEffect(int collectingLayer) {}

    /// <inheritdoc/>
    protected override void BindDropEvent(UnityAction dropAction) {
        MineGrid grid = GetComponentInParent<MineGrid>();
        grid.DetonationSystem.DetonationEvent += dropAction;
    }
}