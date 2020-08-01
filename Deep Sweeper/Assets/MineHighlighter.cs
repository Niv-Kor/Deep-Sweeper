using UnityEngine;

public class MineHighlighter : MonoBehaviour
{
    [Tooltip("The default mine shader.")]
    [SerializeField] private Material defaultShader;

    [Tooltip("The shader of the mine while being raycasted.")]
    [SerializeField] private Material rayShader;

    private MeshRenderer render;
    private bool highlighted;

    void Start() {
        this.render = GetComponent<MeshRenderer>();
        Highlight(false);
    }

    /// <summary>
    /// Highlight the mine.
    /// </summary>
    /// <param name="flag">True to highlight or false to cancel</param>
    public void Highlight(bool flag) {
        if (highlighted == flag) return;

        Material targetMat = flag ? rayShader : defaultShader;
        render.material = targetMat;
        highlighted = flag;
    }
}