using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoaderLine : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The line that extends")]
    [SerializeField] private RawImage loadLine;

    [Tooltip("The minimum time it would take the loader to fully load (in seconds).")]
    [SerializeField] private float minLoadTime = 1;
    #endregion

    #region Class Members
    private RectTransform lineTransform;
    private float loaderHeight;
    #endregion

    #region Events
    public event UnityAction LoaderFinishEvent;
    #endregion

    private void Start() {
        this.lineTransform = loadLine.GetComponent<RectTransform>();
        this.loaderHeight = lineTransform.sizeDelta.y;
        Reset();
    }

    public void Reset() {
        lineTransform.sizeDelta = new Vector2(0, loaderHeight);
    }

    /// <summary>
    /// Start the line loader.
    /// </summary>
    public void Load(LoadingProcess process) {
        StartCoroutine(ExecuteProcesses(process));
    }

    /// <summary>
    /// Stretch the line from the left side of the screen all the way to its right side,
    /// while executing the loading process entirely.
    /// </summary>
    /// <param name="process">The process to execute</param>
    private IEnumerator ExecuteProcesses(LoadingProcess process) {
        float lastWidth = 0;
        float time = minLoadTime;
        float timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            process.ExecuteStage();

            //stretch line
            float lineWidth = Mathf.Lerp(0, UnityEngine.Screen.width, timer / time);
            bool wider = lineWidth > lastWidth;
            lastWidth = lineWidth;
            if (wider) lineTransform.sizeDelta = new Vector2(lineWidth, loaderHeight);

            //extend loader time
            if (timer >= time && process.StageCount > 0)
                time += minLoadTime;

            yield return null;
        }

        LoaderFinishEvent?.Invoke();
    }
}