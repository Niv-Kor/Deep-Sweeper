using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIManager<T> : Singleton<T> where T : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The time it takes the spatials canvas to fade in or out.")]
    [SerializeField] protected float fadeTime;
    #endregion

    #region Class Members
    protected CanvasGroup canvas;
    #endregion

    #region Events
    public event UnityAction<bool> FadeCompleteEvent;
    #endregion

    protected virtual void Start() {
        this.canvas = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Fade the canvas group in or out.
    /// </summary>
    /// <param name="fadeIn">True to fade the spatials in, or false to fade them out</param>
    /// <param name="callback">A function to activate after fading in complete</param>
    protected virtual IEnumerator Fade(bool fadeIn, UnityAction callback = null) {
        float fromVal = fadeIn ? 0 : 1;
        float toVal = fadeIn ? 1 : 0;
        float timer = 0;

        while (timer <= fadeTime) {
            timer += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(fromVal, toVal, timer / fadeTime);
            yield return null;
        }

        callback?.Invoke();
        FadeCompleteEvent?.Invoke(fadeIn);
    }

    /// <summary>
    /// Activate or deactivate the UI components.
    /// </summary>
    /// <param name="flag">True to activate or false to deactivate</param>
    /// <param name="callback">A function to activate after the fade completes</param>
    protected void Activate(bool flag, UnityAction callback = null) {
        StopAllCoroutines();
        StartCoroutine(Fade(flag, callback));
    }
}