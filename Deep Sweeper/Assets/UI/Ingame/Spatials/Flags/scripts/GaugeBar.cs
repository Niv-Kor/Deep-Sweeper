using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Flags
{
    public class GaugeBar : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The bar's sprite")]
        [SerializeField] private Image barSprite;

        [Tooltip("The left vacuum sprite of the bar.")]
        [SerializeField] private Image leftVacuumSprite;

        [Tooltip("The right vacuum sprite of the bar.")]
        [SerializeField] private Image rightVacuumSprite;

        [Header("Timing")]
        [Tooltip("The time it takes to drain or fill the bar gauge (in seconds).")]
        [SerializeField] private float drainTime = .2f;
        #endregion

        #region Constants
        private static readonly string MATERIAL_ALPHA_PROPERTY = "_NoiseIntensity";
        private static readonly float MATERIAL_ALPHA_DEFAULT = -.35f;
        private static readonly float MATERIAL_ALPHA_NONE = -3;
        #endregion

        #region Class Members
        private List<RectTransform> vacuums;
        private Coroutine vacuumReleaseCoroutine;
        private Coroutine showBarCoroutine;
        private float fullBarLength;
        #endregion

        #region Properties
        public float BarPercent { get; private set; }
        #endregion

        private void Awake() {
            this.fullBarLength = barSprite.rectTransform.sizeDelta.x * 1.1f;
            this.vacuums = new List<RectTransform>() {
                leftVacuumSprite.rectTransform,
                rightVacuumSprite.rectTransform
            };

            //initially drain the bar
            this.BarPercent = 1;
            Drain(1, false);
        }

        /// <summary>
        /// Change the width of the vacuum sprites to match the current state of the bar.
        /// </summary>
        private IEnumerator ReleaseVacuum(bool instant) {
            float from = vacuums[0].rect.width;
            float to = fullBarLength * (1 - BarPercent) / 2;
            float time = instant ? 0 : drainTime;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;

                foreach (var vacuum in vacuums) {
                    Vector2 maskedSize = Vector2.Scale(vacuum.sizeDelta, Vector2.up);
                    float width = Mathf.Lerp(from, to, timer / time);
                    vacuum.sizeDelta = maskedSize + Vector2.right * width;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Fade the bar sprite in or out.
        /// </summary>
        /// <param name="flag">True to fade in or false to fade out</param>
        /// <param name="time">The time it takes to finish the process (in seconds)</param>
        private IEnumerator FadeBar(bool flag, float time) {
            Material barMat = barSprite.material;
            float from = barMat.GetFloat(MATERIAL_ALPHA_PROPERTY);
            float to = flag ? MATERIAL_ALPHA_DEFAULT : MATERIAL_ALPHA_NONE;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(from, to, timer / time);
                barMat.SetFloat(MATERIAL_ALPHA_PROPERTY, alpha);

                yield return null;
            }
        }

        /// <summary>
        /// Show or hide the bar.
        /// </summary>
        /// <param name="flag">True to show or false to hide</param>
        /// <param name="time">The time it takes to animation to finish (in seconds)</param>
        public void Show(bool flag, float time = 0) {
            if (showBarCoroutine != null) StopCoroutine(showBarCoroutine);
            showBarCoroutine = StartCoroutine(FadeBar(flag, time));
        }

        /// <summary>
        /// Drain or fill the bar.
        /// </summary>
        /// <param name="delta">The bar's change value [0:1]</param>
        private void ChangeBarContent(float delta, bool animate = true) {
            if (delta == 0) return;

            float nextPercent = Mathf.Clamp(BarPercent + delta, 0, 1);

            if (nextPercent != BarPercent) {
                BarPercent = nextPercent;
                if (vacuumReleaseCoroutine != null) StopCoroutine(vacuumReleaseCoroutine);
                vacuumReleaseCoroutine = StartCoroutine(ReleaseVacuum(!animate));
            }
        }

        /// <summary>
        /// Drain the bar.
        /// </summary>
        /// <param name="percent">The percentage of the bar to drain [0:1]</param>
        public void Drain(float percent = 1, bool animate = true) {
            ChangeBarContent(-percent, animate);
        }

        /// <summary>
        /// Fill the bar.
        /// </summary>
        /// <param name="percent">The percentage of the bar to fill [0:1]</param>
        public void Fill(float percent = 1, bool animate = true) {
            ChangeBarContent(percent, animate);
        }
    }
}