using TMPro;
using UnityEngine;

public class MineGrid : MonoBehaviour
{
    private TextMeshPro indicatorText;
    private int m_indicator;

    public bool IsMined { get; set; }
    public int Indicator {
        get { return m_indicator; }
        set {
            m_indicator = value;
            SetIncidcator(m_indicator);
        }
    }

    private void Awake() {
        this.indicatorText = GetComponentInChildren<TextMeshPro>();
        this.Indicator = 0;
        this.IsMined = false;
        DisplayIndicator(false);
    }

    /// <summary>
    /// Set the icon of the grid.
    /// </summary>
    /// <param name="indicator">The number to show as indicator</param>
    private void SetIncidcator(int indicator) {
        indicatorText.text = "" + indicator;
    }

    public void DisplayIndicator(bool flag) {
        indicatorText.gameObject.SetActive(flag);
    }
}