using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class SectorHighlighter : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [SerializeField] private List<RectTransform> onSelectionGrow;
        [SerializeField] private List<RectTransform> onSelectionShrink;
        [SerializeField] private RawImage characterSprite;
        [SerializeField] private Image segmentSprite;

        [Header("Size")]
        [SerializeField] private float growTime = 1;
        [SerializeField] private float growRate;

        [Header("Color")]
        [SerializeField] private float colorizationTime = 1;
        [SerializeField] private Color segmentColor;
        [SerializeField] private Color characterColor;
        #endregion

        #region Class Members
        private Color originSegmentColor;
        private Color originCharacterColor;
        private List<RectTransform> dynamicSprites;
        private IDictionary<RectTransform, Vector3> originScales;
        #endregion

        private void Awake() {
            this.originSegmentColor = segmentSprite.color;
            this.originCharacterColor = characterSprite.color;
            this.dynamicSprites = new List<RectTransform>();
            dynamicSprites.AddRange(onSelectionShrink);
            dynamicSprites.AddRange(onSelectionGrow);

            //cache the original scales of all dynamic sprites
            this.originScales = new Dictionary<RectTransform, Vector3>();
            foreach (RectTransform image in dynamicSprites)
                originScales.Add(image, image.localScale);
        }

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

        private IEnumerator Colorize(bool highlight) {
            float timer = 0;
            Color segStartColor = segmentSprite.color;
            Color charStartColor = characterSprite.color;
            Color segTargetColor = highlight ? segmentColor : originSegmentColor;
            Color charTargetColor = highlight ? characterColor : originCharacterColor;

            while (timer <= colorizationTime) {
                timer += Time.deltaTime;
                float step = timer / colorizationTime;
                segmentSprite.color = Color.Lerp(segStartColor, segTargetColor, step);
                characterSprite.color = Color.Lerp(charStartColor, charTargetColor, step);

                yield return null;
            }
        }

        public void Highlight(bool flag) {
            StopAllCoroutines();
            StartCoroutine(Resize(flag));
            StartCoroutine(Colorize(flag));
        }
    }
}