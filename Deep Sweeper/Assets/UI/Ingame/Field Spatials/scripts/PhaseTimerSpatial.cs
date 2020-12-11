using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PhaseTimerSpatial : PhaseSpatial<PhaseTimerSpatial>
{
    #region Exposed Editor Parameters
    [Tooltip("The text component that consists of the timer's seconds.")]
    [SerializeField] private TextMeshProUGUI modifiableText;
    #endregion

    #region Constants
    private static readonly int DECIMALS_THRESHOLD = 100;
    #endregion

    #region Events
    public event UnityAction TimeupEvent;
    #endregion

    /// <summary>
    /// Activate the clock and start counting towards 0.
    /// </summary>
    /// <param name="seconds">Amount of seconds to start with</param>
    private IEnumerator Countdown(float seconds) {
        modifiableText.text = (seconds + 1) + ":00";

        while (seconds > 0) {
            float integer = Mathf.Max((int) seconds, 0);

            if (integer < DECIMALS_THRESHOLD) {
                float decimals = (int) (Mathf.Max(seconds % 1f, 0) * 100);
                string decPrefix = (decimals < 10) ? "0" : "";
                modifiableText.text = integer + ":" + decPrefix + decimals;
            }
            else modifiableText.text = integer.ToString();

            yield return new WaitForSeconds(1);
            seconds--;
        }

        TimeupEvent?.Invoke();
    }

    /// <summary>
    /// Set and start the timer.
    /// </summary>
    /// <param name="seconds">Initial clock setting</param>
    public void Set(int seconds) {
        Stop();
        Enabled = true;
        StartCoroutine(Countdown(seconds));
    }

    /// <summary>
    /// Stop the timer.
    /// </summary>
    public void Stop() {
        StopAllCoroutines();
        Enabled = false;
    }
}