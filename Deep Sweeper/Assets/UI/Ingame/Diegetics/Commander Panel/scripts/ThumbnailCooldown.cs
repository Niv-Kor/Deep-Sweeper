using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DeepSweeper.Gameplay.UI.Diegetics.Commander
{
    public class ThumbnailCooldown : MonoBehaviour
    {
        #region Class Members
        private Image radialClock;
        private TextMeshProUGUI remainTime;
        private bool paused;
        #endregion

        #region Events
        public event UnityAction CooldownOverEvent;
        #endregion

        #region Properties
        public bool CooldownActive => RadialFill > 0;
        public float RadialFill {
            get => radialClock.fillAmount;
            set { radialClock.fillAmount = Mathf.Clamp(value, 0, 1); }
        }
        #endregion

        private void Awake() {
            this.radialClock = GetComponentInChildren<Image>();
            this.remainTime = GetComponentInChildren<TextMeshProUGUI>();
            this.RadialFill = 0;
            this.paused = false;
        }

        /// <summary>
        /// Slowly reduce the size of the clock's sector until it reaches size 0.
        /// </summary>
        /// <param name="time">The time it takes to fully reduce it to 0</param>
        private IEnumerator ReduceClockSector(float time) {
            RadialFill = 1;
            float timer = 0;

            while (timer <= time) {
                if (paused) yield return null;

                timer += Time.deltaTime;
                RadialFill = Mathf.Lerp(1, 0, timer / time);
                yield return null;
            }
        }

        /// <summary>
        /// Reduce the timer second by second until it reaches 0.
        /// </summary>
        /// <param name="seconds">The amount of seconds with which to start the timer.</param>
        private IEnumerator Countdown(int seconds) {
            do {
                if (paused) yield return null;
                remainTime.text = seconds.ToString();
                yield return new WaitForSeconds(1);
            }
            while (--seconds > 0);

            remainTime.text = "";
            CooldownOverEvent?.Invoke();
        }

        /// <summary>
        /// Set and start the cooldown clock.
        /// </summary>
        /// <param name="seconds">Cooldown time</param>
        public void Set(int seconds) {
            StopAllCoroutines();
            Resume();
            StartCoroutine(ReduceClockSector(seconds));
            StartCoroutine(Countdown(seconds));
        }

        /// <summary>
        /// Pause the cooldown clock.
        /// </summary>
        public void Pause() { paused = true; }

        /// <summary>
        /// Resume the cooldown clock;
        /// </summary>
        public void Resume() { paused = false; }

        /// <summary>
        /// Stop the clock and release the cooldown immediately.
        /// </summary>
        public void Stop() {
            StopAllCoroutines();
            Resume();
            RadialFill = 0;
        }
    }
}