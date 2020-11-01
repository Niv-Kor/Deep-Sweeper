using System.Collections;
using UnityEngine;

public class FlagsGauge : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The flag's gauge bar.")]
    [SerializeField] private RectTransform gauge;

    [Header("Settings")]
    [Tooltip("The percentage by which the scale of the icon multiplies when is pumps (1:Inf).")]
    [SerializeField] private float pumpPercent = 1.1f;

    [Header("Timing")]
    [Tooltip("The time it takes to complete one fill or drain action.")]
    [SerializeField] private float fillTime = .5f;

    [Tooltip("The time it takes the flag icon to pump once.")]
    [SerializeField] private float pumpTime = .5f;
    #endregion

    #region Class Members
    private FlagsManager flagsMngr;
    private RectTransform gaugeRect;
    private float initHeight;
    private float currentHeight;
    private float fullHeight;
    private bool isPumping;
    #endregion

    private void Start() {
        this.flagsMngr = FlagsManager.Instance;
        this.gaugeRect = gauge.GetComponent<RectTransform>();
        float scaleY = gaugeRect.transform.localScale.y;
        this.fullHeight = gaugeRect.rect.height * scaleY;
        this.initHeight = gaugeRect.localPosition.y - fullHeight;
        this.currentHeight = initHeight;
        this.isPumping = false;

        //bind events
        flagsMngr.FlagsAmountUpdateEvent += delegate { Reset(); };
        flagsMngr.FlagReturnedEvent += delegate(bool success) { SetGauge(); };
        flagsMngr.FlagTakenEvent += delegate(bool success) {
            if (success) SetGauge();
            else if (!isPumping) StartCoroutine(Pump());
        };
    }

    private void Reset() {
        currentHeight = initHeight;
        SetGauge();
    }

    /// <summary>
    /// Set the gauge's height according to the current available phase's flags.
    /// </summary>
    public void SetGauge() {
        float overallPercent = CalcAvailableFlagsPercent();
        float partialHeight = (1 - overallPercent) * fullHeight;
        float targetHeight = initHeight + partialHeight;

        StopCoroutine("Set");
        StartCoroutine(Set(targetHeight));
    }

    /// <summary>
    /// Move the gauge upwards or downwards.
    /// </summary>
    /// <param name="targetHeight">The destination height of the gauge</param>
    private IEnumerator Set(float targetHeight) {
        Vector3 pos = gaugeRect.localPosition;
        float startHeight = currentHeight;
        float timer = 0;

        while (timer <= fillTime) {
            timer += Time.deltaTime;
            currentHeight = Mathf.Lerp(startHeight, targetHeight, timer / fillTime);
            pos.y = currentHeight;
            gaugeRect.localPosition = pos;

            yield return null;
        }
    }

    /// <summary>
    /// Pump the flag icon.
    /// </summary>
    private IEnumerator Pump() {
        isPumping = true;
        Vector3 startingScale = transform.localScale;
        Vector3 targetScale = startingScale * pumpPercent;
        float halfTime = pumpTime / 2;
        float timer = 0;

        //scale up
        while (timer <= pumpTime / 2) {
            timer += Time.deltaTime;
            Vector3 scale = Vector3.Lerp(startingScale, targetScale, timer / halfTime);
            transform.localScale = scale;

            yield return null;
        }

        //scale down
        timer = 0;
        while (timer <= halfTime) {
            timer += Time.deltaTime;
            Vector3 scale = Vector3.Lerp(targetScale, startingScale, timer / halfTime);
            transform.localScale = scale;

            yield return null;
        }

        isPumping = false;
    }

    /// <returns>The percentage of available flags out of the maximum flags in the current phase.</returns>
    private float CalcAvailableFlagsPercent() {
        return (float) flagsMngr.AvailableFlags / flagsMngr.MaxFlags;
    }
}