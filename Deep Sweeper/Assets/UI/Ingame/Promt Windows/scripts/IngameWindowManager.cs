using System;
using System.Collections.Generic;
using UnityEngine;

public class IngameWindowManager : Singleton<IngameWindowManager>
{
    [Serializable]
    private struct PromtWindowConfig
    {
        [Tooltip("The promt window type.")]
        [SerializeField] public PromtTypes Type;

        [Tooltip("A prefab of the promt window.")]
        [SerializeField] public PromtWindow WindowPrefab;
    }

    #region Exposed Editor Parameters
    [Tooltip("A list of all available window configurations.")]
    [SerializeField] private List<PromtWindowConfig> windows;
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
            window.transform.SetParent(transform);
            return window;
        }

        return null;
    }
}