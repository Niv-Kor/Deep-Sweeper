using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.Menu.Map
{
    public class TileHighlighter : TileAttribute
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The background image of the this tile.")]
        [SerializeField] private RawImage tileBackground;

        [Header("Color")]
        [Tooltip("The color of an highlighted tile's frame.")]
        [SerializeField] private Color frameColor;

        [Tooltip("Overall mask color to apply on the tile's texture.")]
        [SerializeField] private Color maskColor;

        [Tooltip("The color of the highlight under the mask.")]
        [SerializeField] private Color highlightColor;

        [Header("Timing")]
        [Tooltip("The speed of the tile's highlight animation (0 to prevent any animation).")]
        [SerializeField] private float speed = 1;
        #endregion

        #region Constants
        private static readonly Color TRANSPARENT = new Color(0x0, 0x0, 0x0, 0x0);
        #endregion

        #region Class Members
        private RawImage texture;
        #endregion

        #region Properties
        protected override TileAttributeState DefaultState => TileAttributeState.Off;
        #endregion

        private void Awake() {
            this.texture = GetComponent<RawImage>();
            tileBackground.color = TRANSPARENT;
        }

        private void OnValidate() {
            if (State == TileAttributeState.On) {
                tileBackground.color = highlightColor;
            }
        }

        /// <summary>
        /// Highlight the tile.
        /// </summary>
        private IEnumerator Highlight() {
            Color defColor = texture.color;
            float duration = 1;
            float timer = 0;

            while (true) {
                timer += Time.deltaTime * speed;
                float wave = timer * Mathf.PI / duration;
                wave = (Mathf.Cos(wave + Mathf.PI) + 1) / 2;
                float step = (speed > 0) ? wave : 1;
                texture.color = Color.Lerp(defColor, maskColor, step);
                yield return null;
            }
        }

        /// <inheritdoc/>
        protected override void SetState(TileAttributeState state) {
            StopAllCoroutines();

            if (state == TileAttributeState.On) {
                tileBackground.color = highlightColor;
                StartCoroutine(Highlight());
            }
            else tileBackground.color = TRANSPARENT;
        }
    }
}