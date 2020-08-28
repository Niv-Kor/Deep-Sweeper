using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class IndicationLight : MonoBehaviour
{
    [Serializable]
    private struct NumeralIndication
    {
        [Tooltip("The color of the number.")]
        [SerializeField] public Color color;

        [Tooltip("Numeral sprite used as the spotlight's cookie image.")]
        [SerializeField] public Sprite sprite;
    }

    [Tooltip("The color of each indicative number.")]
    [SerializeField] private NumeralIndication[] numbers;

    [Tooltip("The time it takes the number to dissolve in after revelation.")]
    [SerializeField] private float dissolveInTime;

    private static readonly Color TRANSPARENT = new Color(0x0, 0x0, 0x0, 0x0);
    private static readonly Color WHITE = new Color(0xff, 0xff, 0xff);

    private TextMeshPro textMesh;
    private MeshRenderer render;
    private Color m_faceColor;

    public Color FaceColor {
        get { return textMesh.faceColor; }
        set { textMesh.faceColor = value; }
    }

    public Color OutlineColor {
        get { return textMesh.outlineColor; }
        set { textMesh.outlineColor = value; }
    }

    public bool Enabled {
        get { return render.enabled; }
        set { render.enabled = value; }
    }

    public int Value {
        get {
            try { return int.Parse(textMesh.text); }
            catch (Exception) { return 0; }
        }
        set {
            textMesh.text = value.ToString();
            bool colorDefined = value >= 0 && value < numbers.Length;
            m_faceColor = colorDefined ? numbers[value].color : WHITE;
        }
    }

    private void Awake() {
        this.textMesh = GetComponent<TextMeshPro>();
        this.render = GetComponent<MeshRenderer>();
        this.FaceColor = TRANSPARENT;
        this.OutlineColor = TRANSPARENT;
        this.m_faceColor = TRANSPARENT;
        this.Enabled = false;

        Indicator indicator = GetComponentInParent<Indicator>();
        indicator.IndicatorRevealEvent += delegate (bool instant) {
            StartCoroutine(DissolveIn(instant));
        };
    }

    /// <summary>
    /// Dissolve the number in (from transparent to opaque).
    /// </summary>
    /// <param name="instant">True to dissolve instantly</param>
    private IEnumerator DissolveIn(bool instant) {
        float lerpedTime = 0;

        //dissolve the pile away
        while (lerpedTime < dissolveInTime) {
            lerpedTime += instant ? dissolveInTime : Time.deltaTime;
            FaceColor = Color.Lerp(TRANSPARENT, m_faceColor, lerpedTime / dissolveInTime);
            yield return null;
        }
    }
}