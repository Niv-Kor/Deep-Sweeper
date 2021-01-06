using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MeshClone : MonoBehaviour
{
    [Tooltip("Source mesh whose movement will be mimicked.")]
    [SerializeField] protected GameObject source;

    protected Material currentMaterial;
    protected Renderer render;
    protected bool isShown;

    protected virtual void Awake() {
        this.render = GetComponent<Renderer>();
        this.currentMaterial = render.material;
        this.isShown = true;
    }

    protected virtual void Update() {
        if (source != null && source.activeSelf) {
            transform.position = source.transform.position;
            transform.rotation = source.transform.rotation;
        }
    }

    /// <summary>
    /// Display or hide the mesh.
    /// </summary>
    /// <param name="flag">True to display or false to hide</param>
    public virtual void DisplayMesh(bool flag) {
        if (!flag || currentMaterial != null) {
            isShown = flag;
            render.materials = new Material[flag ? 1 : 0];
            if (flag) render.material = currentMaterial;
        }
    }
}