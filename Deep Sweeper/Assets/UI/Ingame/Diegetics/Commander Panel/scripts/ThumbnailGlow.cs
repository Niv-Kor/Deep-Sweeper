using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.Gameplay.UI.Diegetics.Commander
{
    public class ThumbnailGlow : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The background image of a glowing thumbnail.")]
        [SerializeField] private RawImage background;

        [Tooltip("The glowing effect around a glowing thumbnail.")]
        [SerializeField] private RawImage glowEffect;

        [Header("Timing")]
        [Tooltip("The time it takes to apply the glow effect.")]
        [SerializeField] private float glowTime;
        #endregion

        #region Constants
        private static readonly Color DEFAULT_GLOW_COLOR = Color.white;
        #endregion

        #region Class Members
        private CanvasGroup canvas;
        private Color m_color;
        #endregion

        #region Properties
        public bool IsGlowing { get; private set; }
        public Color Color {
            get => m_color;
            set {
                m_color = value;
                background.color = value;
                glowEffect.color = value;
            }
        }
        #endregion

        private void Awake() {
            this.m_color = DEFAULT_GLOW_COLOR;
            this.canvas = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Slowly fade the glowing components in or out.
        /// </summary>
        /// <param name="fadeIn">True to fade them in or false to fade them out</param>
        private IEnumerator FadeCanvas(bool fadeIn) {
            float from = canvas.alpha;
            float to = fadeIn ? 1 : 0;
            float timer = 0;

            while (timer <= glowTime) {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(from, to, timer / glowTime);
                yield return null;
            }
        }
        
        /// <summary>
        /// Apply or remove the glow effect.
        /// </summary>
        /// <param name="flag">True to apply or false to remove</param>
        public void Apply(bool flag) {
            IsGlowing = flag;
            StopAllCoroutines();
            StartCoroutine(FadeCanvas(flag));
        }
    }
}