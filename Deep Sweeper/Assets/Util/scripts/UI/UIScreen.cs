using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIScreen : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Settings")]
    [Tooltip("The unique signature of the screen.")]
    [SerializeField] protected ScreenLayout screenId;

    [Header("Fade Delay")]
    [Tooltip("The time it takes the screen to start fading in.")]
    [SerializeField] protected float fadeInDelay = 0;

    [Tooltip("True to only wait once for the fade delay.")]
    [SerializeField] protected bool delayOnce;
    #endregion

    #region Class Members
    protected MultiscreenUI multiscreenUI;
    protected CanvasGroup canvas;
    protected bool shouldDelayFadeIn;
    #endregion

    #region Properties
    public ScreenLayout ID { get => screenId; }
    public bool IsPresent { get => canvas.alpha > 0 || canvas.blocksRaycasts; }
    public List<UIScreen> ChildScreens { get; protected set; }
    public List<UIScreen> ParentScreens { get; protected set; }
    public List<ScreenLayout> ChildScreensID { get; protected set; }
    public List<ScreenLayout> ParentScreensID { get; protected set; }
    protected MultiscreenUI UI { get; private set; }
    #endregion

    protected virtual void Start() {
        this.UI = GetComponentInParent<MultiscreenUI>();
        this.canvas = GetComponent<CanvasGroup>();
        this.ChildScreens = GetComponentsInChildren<UIScreen>().ToList();
        this.ParentScreens = GetComponentsInParent<UIScreen>().ToList();
        ChildScreens.Remove(this);
        ParentScreens.Remove(this);
        this.shouldDelayFadeIn = true;
        this.ChildScreensID = (from screen in ChildScreens
                               select screen.ID).ToList();

        this.ParentScreensID = (from screen in ParentScreens
                                select screen.ID).ToList();
    }

    /// <see cref="FadeScreen(bool)"/>
    protected virtual IEnumerator Fade(bool fadeIn, float time) {
        if (!fadeIn) canvas.blocksRaycasts = false;
        float timer;

        //wait
        if (fadeIn && shouldDelayFadeIn) {
            timer = fadeInDelay;

            while (timer > 0) {
                timer -= Time.deltaTime;
                yield return null;
            }

            if (delayOnce) shouldDelayFadeIn = false;
        }

        float from = canvas.alpha;
        float to = fadeIn ? 1 : 0;
        timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(from, to, timer / time);
            yield return null;
        }

        if (fadeIn) canvas.blocksRaycasts = true;
    }

    /// <summary>
    /// Slowly fade the screen in or out.
    /// </summary>
    /// <param name="fadeIn">True to fade the sceen in (make it appear)</param>
    /// <param name="time">The time it takes to fade the screen</param>
    public void FadeScreen(bool fadeIn, float time) {
        //display all parent screens
        if (fadeIn) {
            foreach (UIScreen screen in ParentScreens)
                if (!screen.IsPresent) screen.FadeScreen(true, time);
        }
        //dismiss all child screens
        else {
            foreach (UIScreen screen in ChildScreens)
                if (screen.IsPresent) screen.FadeScreen(false, time);
        }

        StopAllCoroutines();
        StartCoroutine(Fade(fadeIn, time));
    }
}