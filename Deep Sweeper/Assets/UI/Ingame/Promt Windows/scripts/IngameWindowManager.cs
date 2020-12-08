using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameWindowManager : Singleton<IngameWindowManager>
{
    [Serializable]
    private struct PromtWindowConfig {
        [Tooltip("The promt window type.")]
        [SerializeField] public PromtTypes Type;

        [Tooltip("A prefab of the promt window.")]
        [SerializeField] public PromtWindow WindowPrefab;
    }

    #region Exposed Editor Parameters
    [Tooltip("A list of all available window configurations.")]
    [SerializeField] private List<PromtWindowConfig> windows;

    [Tooltip("The time it takes the window to fully scale up during entrance animation.")]
    [SerializeField] private float scaleTime;
    #endregion

    /// <summary>
    /// Pop a promt window on the screen.
    /// </summary>
    /// <param name="promtType">The type of the window to pop</param>
    /// <returns>The popped window, or null if it couldn't be created.</returns>
    public PromtWindow Pop(PromtTypes promtType) {
        PromtWindowConfig config = windows.Find(x => x.Type == promtType);

        if (windows.Count > 0 && config.Type == promtType) {
            PromtWindow window = Instantiate(config.WindowPrefab);
            float scale = window.transform.localScale.x;
            window.transform.SetParent(transform);
            window.transform.localScale = Vector3.zero;
            window.transform.localPosition = Vector3.zero;
            StartCoroutine(ScaleEnter(window, scale));

            //display cursor
            CursorViewer.Instance.Display = true;
            CursorViewer.Instance.Lock = true;
            return window;
        }

        return null;
    }

    /// <summary>
    /// Scale the window up from zero, until it reaches its original size.
    /// </summary>
    /// <param name="window">The window to scale</param>
    /// <param name="fullScale">The original scale value of the window</param>
    private IEnumerator ScaleEnter(PromtWindow window, float fullScale) {
        float timer = 0;
        window.transform.localScale = Vector3.one * fullScale;

        while (timer <= scaleTime) {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(0, fullScale, timer / scaleTime);
            window.transform.localScale = Vector3.one * scale;

            yield return null;
        }
    }
}