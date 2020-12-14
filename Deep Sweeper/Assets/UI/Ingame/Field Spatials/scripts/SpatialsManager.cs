using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class SpatialsManager : Singleton<SpatialsManager>
{
    #region Exposed Editor Parameters
    [Tooltip("The time it takes the spatials canvas to fade in or out.")]
    [SerializeField] private float fadeTime;
    #endregion

    #region Class Members
    private CanvasGroup canvas;
    #endregion

    private void Start() {
        this.canvas = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Fade the canvas group in or out.
    /// </summary>
    /// <param name="fadeIn">True to fade the spatials in, or false to fade them out</param>
    /// <param name="callback">A function to activate after fading in complete</param>
    private IEnumerator Fade(bool fadeIn, UnityAction callback = null) {
        float fromVal = fadeIn ? 0 : 1;
        float toVal = fadeIn ? 1 : 0;
        float timer = 0;

        while (timer <= fadeTime) {
            timer += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(fromVal, toVal, timer / fadeTime);
            yield return null;
        }

        callback?.Invoke();
    }

    /// <summary>
    /// Activate the phase spatials.
    /// </summary>
    /// <param name="difficulty">Current phase's difficulty level</param>
    /// <param name="timer">Currernt phase's timer (in integer seconds)</param>
    public void Activate(DifficultyLevel difficulty, int timer) {
        PhaseTimerSpatial.Instance.Set(difficulty, timer);
        PhaseNameSpatial.Instance.Display(true);
        FlagsGaugeSpatial.Instance.Display(true);

        StopAllCoroutines();
        StartCoroutine(Fade(true));
    }

    /// <summary>
    /// Deactivate the phase spatials.
    /// </summary>
    public void Deactivate() {
        void Callback() {
            PhaseTimerSpatial.Instance.Stop();
            PhaseNameSpatial.Instance.Display(false);
            FlagsGaugeSpatial.Instance.Display(false);
        }

        StopAllCoroutines();
        StartCoroutine(Fade(false, Callback));
    }
}