using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class SectorHighlighter : MonoBehaviour
    {
        [Serializable]
        private enum HighlightState
        {
            Available,
            Selected,
            Dead
        }

        [Serializable]
        private struct StateColor
        {
            [Tooltip("The configured sector state.")]
            [SerializeField] public HighlightState State;

            [Tooltip("The color of the segment during the defined state.")]
            [SerializeField] public Color SegmentColor;

            [Tooltip("The color of the character's sprite during the defined state.")]
            [SerializeField] public Color CharacterColor;
        }

        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("A list of sprites that need to grow bigger while the sector is selected.")]
        [SerializeField] private List<RectTransform> onSelectionGrow;

        [Tooltip("A list of sprites that need to shrink down while the sector is selected.")]
        [SerializeField] private List<RectTransform> onSelectionShrink;

        [Tooltip("The character's sprite component.")]
        [SerializeField] private RawImage characterSprite;

        [Tooltip("The segment's sprite component.")]
        [SerializeField] private Image segmentSprite;

        [Header("Size")]
        [Tooltip("The time it takes the affected sprites to grow or shrink [s].")]
        [SerializeField] private float growTime = 1;

        [Tooltip("The percentage at which the affected sprites will grow or shrink.")]
        [SerializeField] private float growRate;

        [Header("Color")]
        [Tooltip("A list of color configurations for each available highlight state.")]
        [SerializeField] private List<StateColor> statesConfig;

        [Tooltip("The time it takes a sprite to be fully colorized [s].")]
        [SerializeField] private float colorizationTime = 1;
        #endregion

        #region Constants
        private static readonly Color DEFAULT_COLOR = Color.white;
        #endregion

        #region Class Members
        private List<RectTransform> dynamicSprites;
        private IDictionary<RectTransform, Vector3> originScales;
        private HighlightState currentState;
        #endregion

        private void Awake() {
            this.currentState = HighlightState.Available;
            this.dynamicSprites = new List<RectTransform>();
            dynamicSprites.AddRange(onSelectionShrink);
            dynamicSprites.AddRange(onSelectionGrow);

            //cache the original scales of all dynamic sprites
            this.originScales = new Dictionary<RectTransform, Vector3>();
            foreach (RectTransform image in dynamicSprites)
                originScales.Add(image, image.localScale);

            //update the origin scale of the sprite
            SpriteEmbedder spriteEmbedder = GetComponentInChildren<SpriteEmbedder>();
            spriteEmbedder.SpriteEmbeddedEvent += delegate (RectTransform rect) {
                if (originScales.ContainsKey(rect)) originScales[rect] = rect.localScale;
            };
        }

        private void OnValidate() {
            //assert highlight states are well configured
            foreach (HighlightState state in Enum.GetValues(typeof(HighlightState))) {
                if (statesConfig.FindIndex(x => x.State == state) == -1) {
                    statesConfig.Add(new StateColor {
                        State = state,
                        SegmentColor = DEFAULT_COLOR,
                        CharacterColor = DEFAULT_COLOR
                    });
                }
            }
        }

        /// <summary>
        /// Resize the affected sprites.
        /// </summary>
        /// <param name="grow">True to grow them bigger or false to shrink them down</param>
        private IEnumerator Resize(bool grow) {
            float timer = 0;
            float scaleRate;
            Vector3 targetScale;
            var startScales = new Dictionary<RectTransform, Vector3>();

            //measure start scales
            foreach (RectTransform image in dynamicSprites)
                startScales.Add(image, image.localScale);

            while (timer <= growTime) {
                timer += Time.deltaTime;
                float step = timer / growTime;

                if (grow) {
                    //scale the growable sprites up
                    scaleRate = grow ? growRate : 1f / growRate;

                    foreach (RectTransform image in onSelectionGrow) {
                        if (startScales.TryGetValue(image, out Vector3 startScale)) {
                            targetScale = startScale * scaleRate;
                            image.localScale = Vector3.Lerp(startScale, targetScale, step);
                        }
                    }

                    //scale the shrinkable sprites down
                    scaleRate = grow ? 1f / growRate : growRate;

                    foreach (RectTransform image in onSelectionShrink) {
                        if (startScales.TryGetValue(image, out Vector3 startScale)) {
                            targetScale = startScale * scaleRate;
                            image.localScale = Vector3.Lerp(startScale, targetScale, step);
                        }
                    }
                }
                else {
                    //scale all sprites back to their original value
                    foreach (RectTransform image in dynamicSprites) {
                        bool originExists = startScales.TryGetValue(image, out Vector3 startScale);
                        bool destExists = originScales.TryGetValue(image, out targetScale);

                        if (originExists && destExists)
                            image.localScale = Vector3.Lerp(startScale, targetScale, step);
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// Colorize the affected sprites.
        /// </summary>
        /// <param name="state">The highlight state to apply</param>
        private IEnumerator Colorize(HighlightState state) {
            currentState = state;
            float timer = 0;
            StateColor config = statesConfig.Find(x => x.State == state);
            Color segStartColor = segmentSprite.color;
            Color charStartColor = characterSprite.color;
            Color segTargetColor = config.SegmentColor;
            Color charTargetColor = config.CharacterColor;

            while (timer <= colorizationTime) {
                timer += Time.deltaTime;
                float step = timer / colorizationTime;
                segmentSprite.color = Color.Lerp(segStartColor, segTargetColor, step);
                characterSprite.color = Color.Lerp(charStartColor, charTargetColor, step);

                yield return null;
            }
        }

        /// <summary>
        /// Highlight or cancel the sector's highlight.
        /// </summary>
        /// <param name="flag">True to highlight the sector or false to cancel it</param>
        public void Highlight(bool flag) {
            StopAllCoroutines();

            HighlightState state = flag ? HighlightState.Selected : HighlightState.Available;
            StartCoroutine(Colorize(state));
            StartCoroutine(Resize(flag));
        }

        /// <summary>
        /// Highlight the sector's character as dead.
        /// </summary>
        public void Kill() {
            Highlight(false);
            StartCoroutine(Colorize(HighlightState.Dead));
        }

        /// <summary>
        /// Restore the sector's character from its dead state.
        /// </summary>
        public void Resurrect() {
            if (currentState == HighlightState.Dead) {
                Highlight(false);
                StartCoroutine(Colorize(HighlightState.Available));
            }
        }
    }
}