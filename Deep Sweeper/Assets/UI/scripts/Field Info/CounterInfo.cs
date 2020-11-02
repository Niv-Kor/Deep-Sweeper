using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Collections;

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

        [Header("Pump Settings")]
        [Tooltip("The percentage by which the scale of the text multiplies when is pumps (1:Inf).")]
        [SerializeField] public float pumpPercentOnUpdate;

        [Tooltip("The time it takes the flag icon to pump once.")]
        [SerializeField] public float pumpTime;
    }

    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("A list of modifiable counters.")]
    [SerializeField] protected List<CounterSettings> counters;
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

        //pump
        if (textCmp.text != text)
            StartCoroutine(Pump(counter));

        textCmp.text = text;
    }

    /// <see cref="SetCounter(int, string)"/>
    /// <param name="text">The counter's new numeric value</param>
    protected virtual void SetCounter(int index, int num) {
        SetCounter(index, num.ToString());
    }

    /// <summary>
    /// Pump the flag icon.
    /// </summary>
    private IEnumerator Pump(CounterSettings counter) {
        TextMeshProUGUI textCmp = counter.textComponent;
        textCmp.transform.localScale = Vector3.one;
        Vector3 startingScale = Vector3.one;
        Vector3 targetScale = startingScale * counter.pumpPercentOnUpdate;
        float halfTime = counter.pumpTime / 2;
        float timer = 0;

        //scale up
        while (timer <= halfTime) {
            timer += Time.deltaTime;
            Vector3 scale = Vector3.Lerp(startingScale, targetScale, timer / halfTime);
            textCmp.transform.localScale = scale;

            yield return null;
        }

        //scale down
        timer = 0;
        while (timer <= halfTime) {
            timer += Time.deltaTime;
            Vector3 scale = Vector3.Lerp(targetScale, startingScale, timer / halfTime);
            textCmp.transform.localScale = scale;

            yield return null;
        }
    }
}