using System.Collections.Generic;
using UnityEngine;

public class LifeSupply : Spatial<LifeSupply>
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The prefab of a a single life bit object.")]
    [SerializeField] private LifeBit lifeBitPrefab;

    [Header("Settings")]
    [Tooltip("The amount of life supply bits with which to start the level.")]
    [SerializeField] private int initialAmount = 1;

    [Header("Position")]
    [Tooltip("The offset of the first life bit from the left.")]
    [SerializeField] private Vector3 firstOffset;

    [Tooltip("The space between two consecutive life bits.")]
    [SerializeField] private float space;
    #endregion

    #region Class Members
    private Vector2 lifeBitSize;
    private Stack<LifeBit> lifeBits;
    #endregion

    #region Properties
    public int RemainingLives { get; private set; }
    #endregion

    private void Start() {
        RectTransform lifeBitTransform = lifeBitPrefab.GetComponent<RectTransform>();
        this.lifeBitSize = lifeBitTransform.sizeDelta;
        this.lifeBits = new Stack<LifeBit>();
        this.RemainingLives = 0;
        LifeUp(initialAmount);
    }

    /// <summary>
    /// Increase life supplies amount by 1.
    /// </summary>
    public void LifeUp() { LifeUp(1); }

    /// <summary>
    /// Increase life supplies amount.
    /// </summary>
    /// <param name="amount">Amout of life supplies to add</param>
    public void LifeUp(int amount) {
        //find initial position
        float bitWidth = lifeBitSize.x;
        float totalSpace = RemainingLives * (bitWidth + space);
        Vector3 pos = firstOffset + Vector3.right * totalSpace;

        //instantiate bits
        for (int i = 0; i < amount; i++) {
            GameObject bitInstance = Instantiate(lifeBitPrefab.gameObject);
            bitInstance.transform.SetParent(transform);
            bitInstance.transform.localScale = Vector3.one;
            bitInstance.transform.localPosition = pos;
            LifeBit bitCmp = bitInstance.GetComponent<LifeBit>();
            bitCmp.Activate(true);
            lifeBits.Push(bitCmp);
            pos += Vector3.right * (bitWidth + space);
        }
    }

    /// <summary>
    /// Decrease life supplies amount by 1.
    /// </summary>
    /// <returns>True if the are still life bits remaining following by the decrease.</returns>
    public bool LifeDown() { return LifeDown(1); }

    /// <summary>
    /// Decrease life supplies amount.
    /// </summary>
    /// <param name="amount">Amout of life supplies to remove</param>
    /// <returns>True if the are still life bits remaining following by the decrease.</returns>
    public bool LifeDown(int amount) {
        for (int i = 0; i < amount && lifeBits.Count > 0; i++) {
            LifeBit bit = lifeBits.Pop();
            bit.Dispose(true);
        }

        return lifeBits.Count > 0;
    } 
}