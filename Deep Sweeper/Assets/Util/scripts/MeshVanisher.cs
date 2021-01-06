using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MeshVanisher : MonoBehaviour
{
    private Material material;
    private Renderer render;

    private void Start() {
        this.render = GetComponent<Renderer>();
        this.material = render.material;
    }

    /// <summary>
    /// Display or hide the mesh.
    /// </summary>
    /// <param name="flag">True to display or false to hide</param>
    public void DisplayMesh(bool flag) {
        render.materials = new Material[flag ? 1 : 0];
        if (flag) render.material = material;
    }
}