using System.Collections;
using TMPro;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Spatials.Flags
{
    public class GaugeIndicator : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Settings")]
        [Tooltip("The percentage by which the scale of the icon multiplies when it pumps (1:Inf).")]
        [SerializeField] private float pumpPercent = 1.1f;

        [Header("Timing")]
        [Tooltip("The time it takes the flag icon to pump once (in seconds).")]
        [SerializeField] private float pumpTime = .5f;
        #endregion

        #region Constants
        private static readonly int MIN_VALUE = 0;
        private static readonly int MAX_VALUE = 999;
        #endregion

        #region Class Members
        private TextMeshProUGUI numberCmp;
        private Coroutine changeNumberCoroutine;
        private int m_number;
        #endregion

        #region Properties
        public int Number {
            get => m_number;
            private set {
                int num = Mathf.Clamp(value, MIN_VALUE, MAX_VALUE);
                if (num == m_number) return;

                //change the number
                m_number = num;
                numberCmp.text = num.ToString();

                //pump effect
                if (changeNumberCoroutine != null) StopCoroutine(changeNumberCoroutine);
                changeNumberCoroutine = StartCoroutine(ChangeNumber());
            }
        }
        #endregion

        private void Awake() {
            this.numberCmp = GetComponentInChildren<TextMeshProUGUI>();
            this.m_number = 0;
        }

        /// <summary>
        /// Change the indicator's number.
        /// </summary>
        private IEnumerator ChangeNumber() {
            Vector3 startingScale = numberCmp.rectTransform.localScale;
            Vector3 targetScale = startingScale * pumpPercent;
            float halfTime = pumpTime / 2;
            float timer = 0;

            //scale up
            while (timer <= halfTime) {
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
        }

        /// <summary>
        /// Set the indicator's number.
        /// </summary>
        /// <param name="num">The number to set</param>
        public void Set(int num) { Number = num; }

        /// <summary>
        /// Add a number to the indicator.
        /// </summary>
        /// <param name="amount">The amount to add</param>
        public void Add(int amount = 1) { Number += amount; }

        /// <summary>
        /// Subtract a number from the indicator.
        /// </summary>
        /// <param name="amount">The amount to subtract</param>
        public void Subtract(int amount = 1) { Number -= amount; }
    }
}