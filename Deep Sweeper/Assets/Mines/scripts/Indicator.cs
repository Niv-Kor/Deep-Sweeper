using System;
using TMPro;
using UnityEngine;

public class Indicator : MonoBehaviour
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

    private static readonly Color WHITE = new Color(0xff, 0xff, 0xff);
    private static readonly Color BLACK = new Color(0x0, 0x0, 0x0);

    private TextMeshPro text;
    private MeshRenderer render;
    private int m_minedNeighbours;

    public int MinedNeighbours {
        get { return m_minedNeighbours; }
        set {
            m_minedNeighbours = value;
            text.text = value.ToString();

            //change color
            bool colorDefined = value >= 0 && value < numberColors.Length;
            Color innerColor = colorDefined ? numberColors[value].Inner : WHITE;
            Color outlineColor = colorDefined ? numberColors[value].Outline : BLACK;
            text.faceColor = innerColor;
            text.outlineColor = outlineColor;
        }
    }

    public bool Enabled {
        get { return render.enabled; }
        set { render.enabled = value; }
    }

    private void Awake() {
        this.render = GetComponent<MeshRenderer>();
        this.text = GetComponent<TextMeshPro>();
        this.Enabled = false;
    }
}