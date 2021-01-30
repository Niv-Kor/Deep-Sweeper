using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIScreen : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The unique signature of the screen.")]
    [SerializeField] protected Screen screenId;
    #endregion

    #region Class Members
    protected MultiscreenUI multiscreenUI;
    protected CanvasGroup canvas;
    #endregion

    #region Properties
    public Screen ID { get { return screenId; } }
    public bool IsPresent { get { return canvas.alpha > 0; } }
    protected MultiscreenUI UI { get; private set; }
    #endregion

    protected virtual void Start() {
        this.UI = GetComponentInParent<MultiscreenUI>();
        this.canvas = GetComponent<CanvasGroup>();
    }

    /// <see cref="FadeScreen(bool)"/>
    protected virtual IEnumerator Fade(bool fadeIn, float time) {
        float from = canvas.alpha;
        float to = fadeIn ? 1 : 0;
        float timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(from, to, timer / time);
            yield return null;
        }

        canvas.blocksRaycasts = fadeIn;
    }

    /// <summary>
    /// Slowly fade the screen in or out.
    /// </summary>
    /// <param name="fadeIn">True to fade the sceen in (make it appear)</param>
    /// <param name="time">The time it takes to fade the screen</param>
    public void FadeScreen(bool fadeIn, float time) {
        StopAllCoroutines();
        StartCoroutine(Fade(fadeIn, time));
    }
}