using System.Collections.Generic;
using UnityEngine;

public class MarineLifeSpawnControl : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("Maximum amount of packs in the scene.")]
    [SerializeField] private int maxPacks = 100;
    #endregion

    #region Class Members
    private Queue<FishPack> packs;
    #endregion

    private void Start() {
        this.packs = new Queue<FishPack>();
    }

    /// <summary>
    /// Submit a fish pack to the queue.
    /// </summary>
    /// <param name="pack">The pack to submit</param>
    public void SubmitPack(FishPack pack) {
        packs.Enqueue(pack);
        if (packs.Count >= maxPacks) {
            FishPack excessPack = packs.Dequeue();
            Destroy(excessPack.gameObject);
        }
    }
}