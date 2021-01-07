using System.Collections.Generic;

public class Suitcase : Singleton<Suitcase>
{
    #region Class Members
    private Stack<LootInfo> items;
    #endregion

    #region Properties
    public long CashValue {
        get {
            long sum = 0;
            foreach (LootInfo item in items)
                sum += item.Value;

            return sum;
        }
    }
    #endregion

    private void Awake() {
        this.items = new Stack<LootInfo>();
    }

    /// <summary>
    /// Collect an item into the suitcase.
    /// </summary>
    /// <param name="item">The item to collect</param>
    public void Collect(LootInfo loot) {
        items.Push(loot);
    }
}