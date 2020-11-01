using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public abstract class CounterInfo : MonoBehaviour
{
    [Serializable]
    protected struct CounterSettings
    {
        [Tooltip("A list of the cunters' text components.")]
        [SerializeField] public TextMeshProUGUI textComponent;

        [Tooltip("The counter's corresponding font size for each amount of characters\n"
               + "(as noted by the list indices).")]
        [SerializeField] public List<int> fontSizes;
    }

    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("A list of modifiable counters.")]
    [SerializeField] protected List<CounterSettings> counters;
    #endregion

    #region Class Members
    protected int m_leftCounter;
    protected int m_rightCounter;
    #endregion

    protected virtual void Awake() {
        BindEvents();
    }

    /// <summary>
    /// Assign values or triggers to the left and right counters.
    /// </summary>
    protected abstract void BindEvents();

    /// <param name="index">The index of the counter's text component</param>
    /// <returns>The counter's text component.</returns>
    protected virtual CounterSettings GetCounter(int index) {
        return counters[index];
    }

    /// <summary>
    /// Set the value of a counter's text component.
    /// </summary>
    /// <param name="index">The index of the counter's text component</param>
    /// <param name="text">The counter's new string value</param>
    protected virtual void SetCounter(int index, string text) {
        CounterSettings counter = GetCounter(index);
        TextMeshProUGUI textCmp = counter.textComponent;
        List<int> fontSizes = counter.fontSizes;
        int len = text.Length;
        bool listAvailable = fontSizes.Count > 0;
        bool sizeDefined = fontSizes.Count > len;
        int defaultSize = listAvailable ? fontSizes[fontSizes.Count - 1] : 0;
        int size = sizeDefined ? fontSizes[len] : defaultSize;
        textCmp.fontSize = size;
        textCmp.text = text;
    }

    /// <see cref="SetCounter(int, string)"/>
    /// <param name="text">The counter's new numeric value</param>
    protected virtual void SetCounter(int index, int num) {
        SetCounter(index, num.ToString());
    }
}