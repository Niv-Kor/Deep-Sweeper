using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Loader : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The line that extends")]
    [SerializeField] private RawImage loadLine;
    [Tooltip("The object that displays the loading stage's desription")]
    [SerializeField] private TextMeshProUGUI message;

    [Header("Timing")]
    [Tooltip("The minimum time it would take the loader to fully load [s].")]
    [SerializeField] private float minLoadTime = 1;

    [Tooltip("The time it takes the loader to fade in or out.")]
    [SerializeField] private float appearTime = .5f;
    #endregion

    #region Class Members
    private CanvasGroup canvas;
    private RectTransform line;
    private float loaderHeight;
    private float lineMaxWidth;
    #endregion

    #region Events
    private event UnityAction FadeFinishEvent;
    public event UnityAction LoaderFinishEvent;
    #endregion

    #region Properties
    public bool IsLoading { get; set; }
    #endregion

    private void Start() {
        this.canvas = GetComponent<CanvasGroup>();
        this.line = loadLine.GetComponent<RectTransform>();
        this.loaderHeight = line.sizeDelta.y;
        this.lineMaxWidth = line.offsetMax.x;
        Reset();
    }

    public void Reset() {
        line.sizeDelta = new Vector2(lineMaxWidth, loaderHeight);
    }

    /// <summary>
    /// Fade the loader in or out.
    /// </summary>
    /// <param name="fadeIn">True to fade in or false to fade out</param>
    private IEnumerator Fade(bool fadeIn) {
        float from = fadeIn ? 0 : 1;
        float to = fadeIn ? 1 : 0;
        float timer = 0;

        while (timer <= appearTime) {
            timer += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(from, to, timer / appearTime);
            yield return null;
        }

        FadeFinishEvent?.Invoke();
        FadeFinishEvent = null;
    }

    /// <summary>
    /// Start loading a process.
    /// </summary>
    /// <param name="process">The process to execute</param>
    /// <param name="extraMinTime">
    /// An additional amount of time to add
    /// as the loader's minimum load time
    /// </param>
    public void Load(LoadingProcess process, float extraMinTime = 0) {
        if (IsLoading) return;

        Reset();
        IsLoading = true;

        FadeFinishEvent += delegate {
            StartCoroutine(ExecuteProcesses(process, minLoadTime + extraMinTime));
        };

        StartCoroutine(Fade(true));
    }

    /// <summary>
    /// Execute the loading process entirely,
    /// while stretching the line from the left side of the screen all the way to its right side.
    /// </summary>
    /// <param name="process">The process to execute</param>
    /// <param name="minLoadTime">The minimum time it should take the loader to load</param>
    private IEnumerator ExecuteProcesses(LoadingProcess process, float minLoadTime) {
        float lastWidth = 0;
        float time = minLoadTime;
        float partitionTime = time / (process.StageCount + 1);
        float timer = 0, partitionTimer = 0;
        message.text = process.StageTitle; //init first message

        while (partitionTimer <= partitionTime) {
            timer += Time.deltaTime;
            partitionTimer += Time.deltaTime;

            //execute process stage
            if (partitionTimer >= partitionTime && process.StageCount > 0) {
                if (!string.IsNullOrEmpty(process.StageTitle)) message.text = process.StageTitle;
                process.ExecuteStage();
                partitionTimer = 0;
            }

            //stretch line
            float lineWidth = Mathf.Lerp(lineMaxWidth, 0, timer / time);
            bool wider = lineWidth > lastWidth;
            lastWidth = lineWidth;
            if (wider) line.sizeDelta = new Vector2(lineWidth, loaderHeight);

            yield return null;
        }

        StartCoroutine(Fade(false));
        IsLoading = false;
        LoaderFinishEvent?.Invoke();
        LoaderFinishEvent = null;
    }
}