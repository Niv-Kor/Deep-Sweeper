using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlankScreen : Singleton<BlankScreen>
{
    #region Exposed Editor Parametere
    [Tooltip("The color into which the blank screen transforms on blank application.")]
    [SerializeField] private Color blankColor;

    [Header("Timing")]
    [Tooltip("The time it takes to lerp from a transparent screen to a fully blank screen, or the opposite.")]
    [SerializeField] private float defOneWayLerpTime = 1;

    [Tooltip("The time it takes to start lerping again from fully a blank screen.")]
    [SerializeField] private float defPauseTime = 0;
    #endregion

    #region Class Members
    private RawImage image;
    private Color transparent;
    #endregion

    #region Events
    public event UnityAction FullyBlankEvent;
    public event UnityAction FullyTransparentEvent;
    #endregion

    private void Start() {
        this.image = GetComponent<RawImage>();
        this.transparent = image.color;
    }

    /// <summary>
    /// Apply a temporary blank screen.
    /// </summary>
    public void Apply() {
        Apply(defOneWayLerpTime, defPauseTime);
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
    public void Apply(float oneWayTime, float pauseTime) {
        StartCoroutine(LerpScreen(oneWayTime, pauseTime));
    }

    /// <summary>
    /// Smoothly lerp the blank screen from a transparent screen, to blank,
    /// and then transparent again.
    /// </summary>
    /// <param name="oneWayTime">
    /// The time it takes to lerp from a transparent screen
    /// to a fully blank screen, or the opposite.
    /// </param>
    /// <param name="pauseTime">
    /// The time it takes to start lerping again from a fully blank screen
    /// towards a transparent one.
    /// </param>
    private IEnumerator LerpScreen(float oneWayTime, float pauseTime) {
        float timer = 0;

        while (timer <= oneWayTime) {
            timer += Time.deltaTime;
            image.color = Color.Lerp(transparent, blankColor, timer / oneWayTime);
            yield return null;
        }

        FullyBlankEvent?.Invoke();
        yield return new WaitForSeconds(pauseTime);
        timer = 0;

        while (timer <= oneWayTime) {
            timer += Time.deltaTime;
            image.color = Color.Lerp(blankColor, transparent, timer / oneWayTime);
            yield return null;
        }

        FullyTransparentEvent?.Invoke();
    }
}