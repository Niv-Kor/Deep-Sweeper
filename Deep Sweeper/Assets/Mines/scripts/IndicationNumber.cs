using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Level.Mine
{
    public class IndicationNumber : MonoBehaviour
    {
        [Serializable]
        private struct OverallColor
        {
            [Tooltip("The inner vertex color of the number.")]
            [SerializeField] public Color Inner;

            [Tooltip("Outline color of the number.")]
            [SerializeField] public Color Outline;
        }

        [Tooltip("The color of each indicative number.")]
        [SerializeField] private OverallColor[] numberColors;

        [Tooltip("The time it takes the number to dissolve in after revelation.")]
        [SerializeField] private float alphaLerpTime;

        private static readonly Color TRANSPARENT = new Color(0x0, 0x0, 0x0, 0x0);
        private static readonly Color WHITE = new Color(0xff, 0xff, 0xff);
        private static readonly Color BLACK = new Color(0x0, 0x0, 0x0);

        private TextMeshPro textMesh;
        private float m_alpha, maxAlpha;

        public event UnityAction<Color> FaceColorChange;
        public event UnityAction<Color> OutlineColorChange;
        public event UnityAction<float> AlphaChange;
        public event UnityAction<string> ValueChange;

        public Color FaceColor {
            get { return textMesh.faceColor; }
            set {
                textMesh.faceColor = value;
                FaceColorChange?.Invoke(value);
            }
        }

        public Color OutlineColor {
            get { return textMesh.outlineColor; }
            set {
                textMesh.outlineColor = value;
                OutlineColorChange?.Invoke(value);
            }
        }

        public float Alpha {
            get { return m_alpha; }
            set {
                if (value >= 0 && value <= 1 && maxAlpha > 0) {
                    float opacity = RangeMath.NumberOfRange(value, 0, maxAlpha);
                    m_alpha = opacity;
                    AlphaChange?.Invoke(value);
                    StopAllCoroutines();
                    StartCoroutine(LerpAlpha(m_alpha));
                }
            }
        }

        public int Value {
            get {
                try { return int.Parse(textMesh.text); }
                catch (Exception) { return 0; }
            }
            set {
                textMesh.text = value.ToString();
                bool colorDefined = value >= 0 && value < numberColors.Length;
                Color face = colorDefined ? numberColors[value].Inner : WHITE;
                Color line = colorDefined ? numberColors[value].Outline : BLACK;
                FaceColor = new Color(face.r, face.g, face.b, Alpha);
                OutlineColor = new Color(line.r, line.g, line.b, Alpha);
                maxAlpha = face.a;
                ValueChange?.Invoke(value.ToString());
            }
        }

        private void Awake() {
            this.textMesh = GetComponent<TextMeshPro>();
            this.FaceColor = TRANSPARENT;
            this.OutlineColor = TRANSPARENT;
            this.maxAlpha = 0;
            this.Alpha = 0;
        }

        /// <summary>
        /// Lerp the alpha value of the indication number.
        /// </summary>
        /// <param name="alpha">Target alpha value</param>
        private IEnumerator LerpAlpha(float alpha) {
            float timer = 0;
            Color startFace = FaceColor;
            Color startOutline = OutlineColor;
            Color targetFace = FaceColor;
            Color targetOutline = OutlineColor;
            targetFace.a = alpha;
            targetOutline.a = alpha;

            while (timer < alphaLerpTime) {
                timer += Time.deltaTime;
                Color nextFace = Color.Lerp(startFace, targetFace, timer / alphaLerpTime);
                Color nextOutline = Color.Lerp(startOutline, targetOutline, timer / alphaLerpTime);
                FaceColor = nextFace;
                OutlineColor = nextOutline;
                yield return null;
            }
        }
    }
}