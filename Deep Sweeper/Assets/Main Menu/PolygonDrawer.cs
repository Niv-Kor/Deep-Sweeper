using UnityEngine;

/// <summary>
/// draw polygon
/// TODO: There is a problem with drawing when two points overlap
/// </summary>
public class PolygonDrawer : MonoBehaviour
{
    public Material material;
    public Transform[] vertices;
    private MeshRenderer mRenderer;
    private MeshFilter mFilter;

    void Start() {
        Draw();
    }

    void Update() {
        Draw();
    }

    [ContextMenu("Draw")]
    public void Draw() {
        Vector2[] vertices2D = new Vector2[vertices.Length];
        Vector3[] vertices3D = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++) {
            Vector3 vertice = vertices[i].localPosition;
            vertices2D[i] = new Vector2(vertice.x, vertice.y);
            vertices3D[i] = vertice;
        }

        Triangulator tr = new Triangulator(vertices2D);
        int[] triangles = tr.Triangulate();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices3D;
        mesh.triangles = triangles;

        if (mRenderer == null) {
            mRenderer = gameObject.GetOrAddComponent<MeshRenderer>();
        }
        mRenderer.material = material;
        if (mFilter == null) {
            mFilter = gameObject.GetOrAddComponent<MeshFilter>();
        }
        mFilter.mesh = mesh;
    }
}