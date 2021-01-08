﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Spatial<T> : Singleton<T> where T : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Text Pump Effect")]
    [Tooltip("The percentage by which the scale of the text multiplies when is pumps (1:Inf).")]
    [SerializeField] protected float textPumpPercent;

    [Tooltip("The time it takes the text to pump once.")]
    [SerializeField] protected float textPumpTime;
    #endregion

    #region Class Members
    private List<GameObject> children;
    private bool m_enabled;
    #endregion

    #region Properties
    public bool Enabled {
        get { return m_enabled; }
        protected set {
            m_enabled = value;

            foreach (GameObject child in children)
                child.SetActive(value);
        }
    }
    #endregion

    protected virtual void Start() {
        this.children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        this.Enabled = false;
    }

    /// <summary>
    /// Set the value of a counter's text component.
    /// </summary>
    /// <param name="index">The index of the counter's text component</param>
    /// <param name="text">The counter's new string value</param>
    protected virtual void SetText(TextMeshProUGUI cmp, string text, bool pump) {
        if (pump) StartCoroutine(PumpText(cmp, textPumpPercent, textPumpTime));
        cmp.text = text;
    }

    /// <summary>
    /// Pump the flag icon.
    /// </summary>
    protected virtual IEnumerator PumpText(TextMeshProUGUI cmp, float percent, float time) {
        cmp.transform.localScale = Vector3.one;
        Vector3 startingScale = Vector3.one;
        Vector3 targetScale = startingScale * percent;
        float halfTime = time / 2;
        float timer = 0;

        //scale up
        while (timer <= halfTime) {
            timer += Time.deltaTime;
            Vector3 scale = Vector3.Lerp(startingScale, targetScale, timer / halfTime);
            cmp.transform.localScale = scale;

            yield return null;
        }

        //scale down
        timer = 0;
        while (timer <= halfTime) {
            timer += Time.deltaTime;
            Vector3 scale = Vector3.Lerp(targetScale, startingScale, timer / halfTime);
            cmp.transform.localScale = scale;

            yield return null;
        }
    }
}