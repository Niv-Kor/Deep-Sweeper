using DeepSweeper.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SplashScreen : Singleton<SplashScreen>
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("A splash logo to be displayed.")]
    [SerializeField] private RawImage logo;

    [Header("Timing")]
    [Tooltip("The time it takes the logo to start fading in at "
           + "the start of the splash screen.")]
    [SerializeField] private float logoDelayTime;

    [Tooltip("The time it takes the logo to fade in at the "
           + "start of the splash screen.")]
    [SerializeField] private float logoFadeInTime;

    [Tooltip("The time it takes the logo to fade out at the "
           + "end of the splash screen.")]
    [SerializeField] private float logoFadeOutTime;

    [Tooltip("The minimum amount of time the splash screen "
           + "should be displayed before fading away\n"
           + "(Including the time it takes the logo to appear).")]
    [SerializeField] private float minSplashTime = 1;

    [Tooltip("The amount of time it takes the splash screen to fade completetly.")]
    [SerializeField] private float fadeTime = .5f;
    #endregion

    #region Events
    public event UnityAction ScreenFadingEvent;
    public event UnityAction ScreenFadedEvent;
    #endregion

    #region Class Members
    private CanvasGroup canvas;
    private int stages;
    #endregion

    protected override void Awake() {
        base.Awake();
        this.canvas = GetComponent<CanvasGroup>();
        this.stages = 0;

        canvas.alpha = 1;
        RegisterStages();
        ScreenFadedEvent += delegate { gameObject.SetActive(false); };
        StartCoroutine(ShowLogo(true, logoFadeInTime));
    }

    private void OnValidate() {
        float prefixTime = logoDelayTime + logoFadeInTime;
        minSplashTime = Mathf.Max(prefixTime, minSplashTime);
    }

    /// <summary>
    /// Register all passive processes that are ought to be
    /// completed before the splash screen fades.
    /// </summary>
    private void RegisterStages() {
        //database
        RegisterLoadingStage(DatabaseHandler.Instance);
        DatabaseHandler.Instance.Init();
    }

    /// <summary>
    /// Register a new passive process that is ought to be
    /// completed before the splash screen fades.
    /// </summary>
    /// <param name="process">A passive process</param>
    private void RegisterLoadingStage(IPassiveLoadingProcess process) {
        stages++;
        process.ProcessFinishedEvent += delegate { stages--; };
    }

    /// <summary>
    /// Slowly fade the logo in or out.
    /// </summary>
    /// <param name="fadeIn">True to fade the logo in or false to fade it out</param>
    /// <param name="fadeTime">The time it takes the logo to fade in or out</param>
    private IEnumerator ShowLogo(bool fadeIn, float fadeTime) {
        float timer;

        //wait
        if (fadeIn) {
            timer = logoDelayTime;

            while (timer > 0) {
                timer -= Time.deltaTime;
                yield return null;
            }
        }

        //fade in/out
        timer = 0;
        float fromAlpha = fadeIn ? 0 : 1;
        float toAlpha = fadeIn ? 1 : 0;
        Color originColor = logo.color;

        while (timer <= fadeTime) {
            timer += Time.deltaTime;
            float stepAlpha = Mathf.Lerp(fromAlpha, toAlpha, timer / fadeTime);
            Color stepColor = originColor;
            stepColor.a = stepAlpha;
            logo.color = stepColor;
            yield return null;
        }

        //start fading the screen
        if (fadeIn) yield return TryFadeScreen();
    }

    /// <summary>
    /// Wait for the minimum amount of time configured,
    /// and then fade the splash screen out,
    /// but not before every passive process is complete.
    /// </summary>
    private IEnumerator TryFadeScreen() {
        //wait
        float timer = minSplashTime;
        while (timer > 0 || stages > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        ScreenFadingEvent?.Invoke();

        //fade out
        timer = 0;
        float originAlpha = canvas.alpha;
        StartCoroutine(ShowLogo(false, logoFadeOutTime));

        while (timer <= fadeTime) {
            timer += Time.deltaTime;
            float stepAlpha = Mathf.Lerp(originAlpha, 0, timer / fadeTime);
            canvas.alpha = stepAlpha;
            yield return null;
        }

        ScreenFadedEvent?.Invoke();
    }
}