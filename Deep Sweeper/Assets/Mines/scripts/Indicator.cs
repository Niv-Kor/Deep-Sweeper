using TMPro;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    private TextMeshPro text;
    private MeshRenderer render;
    private int m_minedNeighbours;

    public int MinedNeighbours {
        get { return m_minedNeighbours; }
        set {
            m_minedNeighbours = value;
            text.text = (value != 0) ? value.ToString() : "";
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