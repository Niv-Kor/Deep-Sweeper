using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PhaseTimer : MonoBehaviour
{
    #region Constants
    private static readonly int DECIMALS_THRESHOLD = 100;
    #endregion

    #region Class Members
    private TextMeshProUGUI text;
    #endregion

    #region Events
    public event UnityAction TimeupEvent;
    #endregion

    private void Start() {
        this.text = GetComponent<TextMeshProUGUI>();
    }

    private IEnumerator Countdown(float seconds) {
        text.text = seconds + ":00";

        while (seconds > 0) {
            seconds -= Time.deltaTime;
            float integer = Mathf.Max((int) seconds, 0);

            if (integer < DECIMALS_THRESHOLD) {
                float decimals = (int) (Mathf.Max(seconds % 1f, 0) * 100);
                string decPrefix = (decimals < 10) ? "0" : "";
                text.text = integer + ":" + decPrefix + decimals;
            }
            else text.text = integer.ToString();
            yield return null;
        }

        TimeupEvent?.Invoke();
    }

    /// <summary>
    /// Set and start the timer.
    /// </summary>
    /// <param name="seconds">Initial clock setting</param>
    public void Set(int seconds) {
        Stop();
        StartCoroutine(Countdown(seconds));
    }

    /// <summary>
    /// Stop the timer.
    /// </summary>
    public void Stop() {
        StopAllCoroutines();
    }
}