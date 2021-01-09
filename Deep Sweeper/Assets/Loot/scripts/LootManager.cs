using System.Collections.Generic;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    #region Class Members
    private List<LootInfo> items;
    #endregion

    private void Awake() {
        this.items = new List<LootInfo>();
    }

    /// <summary>
    /// Drop a loot item in the scene.
    /// </summary>
    /// <param name="item">The item's prefab object</param>
    /// <param name="position">The position at which the item should be dropped</param>
    /// <returns>The instance of the newly created item</returns>
    public LootItem DropItem(LootItem item, Vector3 position) {
        //instantiate
        GameObject instance = Instantiate(item.gameObject);
        instance.transform.SetParent(transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.position = position;
        instance.transform.localScale = Vector3.zero;

        //append to list
        LootItem instanceCmp = instance.GetComponent<LootItem>();
        LootInfo info;
        info.Item = instanceCmp;
        info.Name = instanceCmp.ItemName;
        info.Value = instanceCmp.Value;
        info.Type = instanceCmp.Type;
        info.PhaseIndex = GameFlow.Instance.CurrentPhase.Index;
        items.Add(info);

        return instanceCmp;
    }

    /// <summary>
    /// Dispose an item from the list.
    /// </summary>
    /// <param name="item">The item to dispose</param>
    public void DisposeItem(LootItem item) {
        items.Remove(GetInfo(item));
        Destroy(item.gameObject);
    }

    /// <summary>
    /// Clear all items that have been dropped during a specifig phase from the scene.
    /// </summary>
    /// <param name="phaseIndex">The index of the phase</param>
    public void ClearPhaseItems(int phaseIndex) {
        Queue < LootInfo > disposables = new Queue<LootInfo>();
        List<LootInfo> newList = new List<LootInfo>();

        //sort
        foreach (LootInfo item in items) {
            if (item.PhaseIndex != phaseIndex) newList.Add(item);
            else disposables.Enqueue(item);
        }

        //destroy phase items
        while (disposables.Count > 0) {
            LootItem item = disposables.Dequeue().Item;
            Destroy(item.gameObject);
        }

        //replace old list
        items = newList;
    }

    /// <param name="item">The subject item</param>
    /// <returns>The info of the specified item.</returns>
    public LootInfo GetInfo(LootItem item) {
        LootInfo info = items.Find(x => x.Item == item);
        return info;
    }

    /// <summary>
    /// Update the info of an item.
    /// </summary>
    /// <param name="item">The item to update</param>
    public void UpdateInfo(LootItem item) {
        for (int i = 0; i < items.Count; i++) {
            LootInfo info = items[i];

            //found correct info
            if (info.Item == item) {
                info.Name = item.ItemName;
                info.Value = item.Value;
                info.Type = item.Type;
                items[i] = info;
                break;
            }
        }
    }
}