using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlankScreen : Singleton<BlankScreen>
{
    #region Exposed Editor Parametere
    [Tooltip("The color into which the blank screen transforms on blank application.")]
    [SerializeField] private Color blankColor;

    [Header("Default Timing")]
    [Tooltip("The time it takes to lerp from a transparent screen to a fully blank screen, or the opposite.")]
    [SerializeField] private float defOneWayLerpTime = 1;

    [Tooltip("The time it takes to start lerping again from fully a blank screen.")]
    [SerializeField] private float defPauseTime = 0;
    #endregion

    #region Constants
    private static readonly float LOADER_FINISH_DELAY_PAUSE = .5f;
    #endregion

    #region Class Members
    private RawImage image;
    private Color transparent;
    private Loader loader;
    #endregion

    #region Events
    public event UnityAction FullyBlankEvent;
    public event UnityAction FullyTransparentEvent;
    #endregion

    private void Start() {
        this.image = GetComponent<RawImage>();
        this.loader = GetComponentInChildren<Loader>();
        this.transparent = image.color;
    }

    /// <summary>
    /// Apply a temporary blank screen.
    /// </summary>
    /// <param name="process">A process to invoke while the screen is fully blank</param>
    /// <param name="fullyTransparentCallback">An action to invoke when the screen is fully transparent again</param>
    public void Apply(LoadingProcess process = null, UnityAction fullyTransparentCallback = null) {
        Apply(defOneWayLerpTime, defPauseTime, process, fullyTransparentCallback);
    }

    /// <summary>
    /// Apply a temporary blank screen.
    /// </summary>
    /// <param name="oneWayTime">
    /// The time it takes to lerp from a transparent screen
    /// to a fully blank screen, or the opposite.
    /// </param>
    /// <param name="pauseTime">
    /// The time it takes to start lerping again from a fully blank screen
    /// towards a transparent one.
    /// </param>
    /// <param name="process">A process to invoke while the screen is fully blank</param>
    /// <param name="fullyTransparentCallback">An action to invoke when the screen is fully transparent again</param>
    public void Apply(float oneWayTime, float pauseTime, LoadingProcess process, UnityAction fullyTransparentCallback = null) {
        void fullyTransparentFunc() {
            fullyTransparentCallback?.Invoke();
            FullyTransparentEvent -= fullyTransparentFunc;
        };

        if (process == null) process = new LoadingProcess(false);
        FullyTransparentEvent += fullyTransparentFunc;
        StartCoroutine(LerpToBlack(oneWayTime, pauseTime, process));
    }

    /// <summary>
    /// Smoothly lerp the from being transparent to to being blank.
    /// </summary>
    /// <param name="oneWayTime">
    /// The time it takes to lerp from a transparent screen
    /// to a fully blank screen, or the opposite.
    /// </param>
    /// <param name="pauseTime">
    /// The minimum time it takes to start lerping again from a fully blank screen
    /// towards a transparent one.
    /// </param>
    private IEnumerator LerpToBlack(float oneWayTime, float pauseTime, LoadingProcess process) {
        float timer = 0;

        while (timer <= oneWayTime) {
            timer += Time.deltaTime;
            image.color = Color.Lerp(transparent, blankColor, timer / oneWayTime);
            yield return null;
        }

        FullyBlankEvent?.Invoke(); //feed loading process

        void ContinueLerp() {
            StartCoroutine(LerpToTransparent(oneWayTime, LOADER_FINISH_DELAY_PAUSE));
        }

        if (process.StageCount == 0) ContinueLerp();
        else {
            if (process.UsingLoader) {
                loader.Load(process, pauseTime);
                loader.LoaderFinishEvent += delegate { ContinueLerp(); };
            }
            else {
                process.ExecuteStage();
                ContinueLerp();
            }
        }
    }

    /// <summary>
    /// Smoothly lerp the from being blank to to being transparent.
    /// </summary>
    /// <param name="oneWayTime">
    /// The time it takes to lerp from a blank screen to fully transparent.
    /// </param>
    /// <param name="startDelay">A delay time to wait for before starting to lerp</param>
    private IEnumerator LerpToTransparent(float oneWayTime, float startDelay) {
        if (startDelay > 0) yield return new WaitForSeconds(startDelay);
        float timer = 0;

        while (timer <= oneWayTime) {
            timer += Time.deltaTime;
            image.color = Color.Lerp(blankColor, transparent, timer / oneWayTime);
            yield return null;
        }

        FullyTransparentEvent?.Invoke();
    }
}