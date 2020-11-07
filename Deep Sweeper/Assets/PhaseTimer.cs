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
        GameFlow.Instance.PhaseUpdatedEvent += delegate(int _) {
            int seconds = GameFlow.Instance.CurrentPhase.TimerSeconds;
            print("TIMER " + seconds);
            StopAllCoroutines();
            StartCoroutine(Countdown(seconds));
        };
    }

    private IEnumerator Countdown(float seconds) {
        text.text = seconds + ":00";
        int maxDigits = seconds.ToString().Length;
        string zeroPad = "";
        for (int i = 0; i < maxDigits; i++) zeroPad += "0";

        while (seconds > 0) {
            seconds -= Time.deltaTime;

            float integer = Mathf.Max((int) seconds, 0);
            int actualIntDigits = integer.ToString().Length;
            string intPrefix = zeroPad.Substring(0, maxDigits - actualIntDigits);

            if (integer < DECIMALS_THRESHOLD) {
                float decimals = (int) (Mathf.Max(seconds % 1f, 0) * 100);
                string decPrefix = (decimals < 10) ? "0" : "";
                text.text = intPrefix + integer + ":" + decPrefix + decimals;
            }
            else text.text = integer.ToString();


            yield return null;
        }

        TimeupEvent?.Invoke();
    }
}