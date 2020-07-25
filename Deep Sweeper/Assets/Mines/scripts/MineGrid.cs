using TMPro;
using UnityEngine;

public class MineGrid : MonoBehaviour
{
    private static readonly int FLAGGED = -1;

    private TextMeshPro textMesh;
    private SpriteRenderer mineImage;
    private int m_surroundingMines;
    private bool m_isMined;

    public int SurroundingMines {
        get { return m_surroundingMines; }
        set {
            m_surroundingMines = value;
            SetIcon(m_surroundingMines);
        }
    }

    public bool IsMined {
        get { return m_isMined; }
        set {
            m_isMined = value;
            if (value) SetIcon(FLAGGED);
        }
    }

    private void Awake() {
        this.textMesh = GetComponentInChildren<TextMeshPro>();
        this.mineImage = GetComponentInChildren<SpriteRenderer>();
        this.SurroundingMines = 0;
        this.IsMined = false;
    }

    /// <summary>
    /// Set the icon of the grid.
    /// </summary>
    /// <param name="indicator">
    /// The icon's text.
    /// Use FLAGGED to set the mine icon.
    /// </param>
    private void SetIcon(int indicator) {
        textMesh.text = "" + indicator;
        textMesh.gameObject.SetActive(indicator >= 0);
        mineImage.gameObject.SetActive(indicator == FLAGGED);
    }
}