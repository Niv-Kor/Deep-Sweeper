using TMPro;
using UnityEngine;

public class MineGrid : MonoBehaviour
{
    public class Indicator
    {
        private TextMeshPro text;
        private MeshRenderer renderer;
        private int m_minedNeighbours;

        public int MinedNeighbours {
            get { return m_minedNeighbours; }
            set {
                m_minedNeighbours = value;
                text.text = "" + value;
            }
        }

        public bool Enabled {
            get { return renderer.enabled; }
            set { renderer.enabled = value; }
        }

        /// <param name="obj">A game object consisting of TextMeshPro and MeshRenderer components</param>
        public Indicator(GameObject obj) {
            this.text = obj.GetComponent<TextMeshPro>();
            this.renderer = obj.GetComponent<MeshRenderer>();
        }
    }

    [Header("Prefabs")]
    [Tooltip("Child object that indicates the amount of mined neighbours.")]
    [SerializeField] private GameObject indicatorObject;

    [Tooltip("Flag cane child object")]
    [SerializeField] private GameObject flagCane;

    public bool IsMined { get; set; }
    public Indicator MinesIndicator { get; private set; }

    public bool IsFlagged {
        get { return flagCane.activeSelf; }
        set { flagCane.SetActive(value); }
    }

    private void Awake() {
        this.MinesIndicator = new Indicator(indicatorObject);
        this.IsMined = false;
        MinesIndicator.Enabled = true;
    }
}