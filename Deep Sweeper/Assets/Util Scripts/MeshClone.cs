using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MeshClone : MonoBehaviour
{
    [Tooltip("Source mesh whose movement will be mimicked.")]
    [SerializeField] protected GameObject source;

    protected Material material;
    protected Renderer render;

    protected virtual void Awake() {
        this.render = GetComponent<Renderer>();
        this.material = render.material;
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
        if (!flag || material != null) {
            render.materials = new Material[flag ? 1 : 0];
            if (flag) render.material = material;
        }
    }
}