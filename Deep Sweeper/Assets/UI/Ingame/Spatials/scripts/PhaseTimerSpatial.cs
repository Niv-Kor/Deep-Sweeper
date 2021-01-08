using Constants;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhaseTimerSpatial : Spatial<PhaseTimerSpatial>
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The text component that consists of the timer's seconds.")]
    [SerializeField] private TextMeshProUGUI modifiableText;

    [Tooltip("The timeline under the clock that represents the time that has left.")]
    [SerializeField] private RectTransform timeline;

    [Header("Warning")]
    [Tooltip("The amount of seconds under which the clock starts to warn the player about a timeout.")]
    [SerializeField] private float warningThreshold;

    [Tooltip("The color of the text after reaching the warning line.")]
    [SerializeField] private Color warningForeground;
    #endregion

    #region ClassMembers
    private RawImage timelineImg;
    private Color originTextColor;
    private Color originTimelineColor;
    private Vector2 originTimelineSize;
    #endregion

    #region Properties
    public bool IsWarning { get; private set; }
    #endregion

    #region Events
    public event UnityAction TimeoutEvent;
    #endregion

    protected override void Start() {
        base.Start();
        this.timelineImg = timeline.GetComponent<RawImage>();
        this.originTextColor = (modifiableText != null) ? modifiableText.color : default;
        this.originTimelineColor = timelineImg.color;
        this.originTimelineSize = timeline.sizeDelta;
        this.IsWarning = false;
    }

    /// <summary>
    /// Activate the clock and start counting towards 0.
    /// </summary>
    /// <param name="seconds">Amount of seconds to start with</param>
    private IEnumerator Countdown(int seconds) {
        seconds = Mathf.Max(seconds, 0);

        while (seconds > 0) {
            //set text
            if (IsWarning != seconds <= warningThreshold) {
                modifiableText.color = warningForeground;
                IsWarning = true;
            }

            SetText(modifiableText, seconds--.ToString(), IsWarning);
            yield return new WaitForSeconds(1);
        }

        TimeoutEvent?.Invoke();
    }

    /// <summary>
    /// Slowly shorten the timeline.
    /// </summary>
    /// <param name="lineColor">The color of the timeline</param>
    /// <param name="time">The time it takes to timeline to reach width 0</param>
    private IEnumerator ShortenTimeline(Color lineColor, float time) {
        timelineImg.color = lineColor;
        float timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            float step = timer / time;
            float width = Mathf.Lerp(originTimelineSize.x, 0, step);
            timeline.sizeDelta = new Vector2(width, originTimelineSize.y);

            yield return null;
        }
    }

    /// <summary>
    /// Set and start the timer.
    /// </summary>
    /// <param name="difficulty">The difficulty level of the current phase</param>
    /// <param name="seconds">Initial clock setting</param>
    public void Set(DifficultyLevel difficulty, int seconds) {
        Stop();
        Enabled = true;
        Color timelineColor = Colors.DifficultyColors.Get(difficulty);
        StartCoroutine(Countdown(seconds));
        StartCoroutine(ShortenTimeline(timelineColor, seconds));
    }

    /// <summary>
    /// Stop the timer.
    /// </summary>
    public void Stop() {
        StopAllCoroutines();
        IsWarning = false;
        modifiableText.color = originTextColor;
        timelineImg.color = originTimelineColor;
        timeline.sizeDelta = originTimelineSize;
        Enabled = false;
    }
}