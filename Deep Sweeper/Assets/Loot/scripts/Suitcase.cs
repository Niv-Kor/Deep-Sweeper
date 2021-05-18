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

    protected override void Awake() {
        base.Awake();
        this.items = new Stack<LootInfo>();
    }

    /// <summary>
    /// Collect an item into the suitcase.
    /// </summary>
    /// <param name="item">The item to collect</param>
    public void Collect(LootInfo loot) {
        items.Push(loot);
    }

    /// <summary>
    /// Remove all of the items that had been collected during a specific phase.
    /// </summary>
    /// <param name="phaseIndex">The index of the phase</param>
    /// <returns>The amount of removed items.</returns>
    public int RemovePhaseItems(int phaseIndex) {
        Stack<LootInfo> newStack = new Stack<LootInfo>();
        int amount = 0;

        //sort
        while (items.Count > 0) {
            LootInfo item = items.Pop();
            if (item.PhaseIndex != phaseIndex) newStack.Push(item);
            else amount++;
        }

        //restore original stack order
        while (newStack.Count > 0)
            items.Push(newStack.Pop());

        return amount;
    }
}