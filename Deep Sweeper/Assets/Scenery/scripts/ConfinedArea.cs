using UnityEngine;

public class ConfinedArea : MonoBehaviour
{
    [Header("Area Definition")]
    [Tooltip("The field confines' offset and size.")]
    [SerializeField] public Confine Confine;

    protected static readonly Color GIZMOS_COLOR = new Color(0xff, 0x0, 0xe8);

    public Vector3 AreaCenter {
        get { return Confine.Offset + Confine.Size / 2; }
    }

    public float AreaPerimeter {
        get {
            float x = Confine.Size.x;
            float y = Confine.Size.y;
            float z = Confine.Size.z;
            return Mathf.Max(x, y, z);
        }
    }

    public float AreaRadius {
        get { return AreaPerimeter / 2; }
    }

    protected virtual void OnDrawGizmos() {
        Vector3 ptA = Confine.Offset;
        Vector3 ptB = ptA + new Vector3(0, 0, Confine.Size.z);
        Vector3 ptC = ptB + new Vector3(Confine.Size.x, 0, 0);
        Vector3 ptD = ptA + new Vector3(Confine.Size.x, 0, 0);
        Vector3 ptE = ptA + new Vector3(0, Confine.Size.y, 0);
        Vector3 ptF = ptB + new Vector3(0, Confine.Size.y, 0);
        Vector3 ptG = ptC + new Vector3(0, Confine.Size.y, 0);
        Vector3 ptH = ptD + new Vector3(0, Confine.Size.y, 0);

        //upper horizontal
        Debug.DrawLine(ptA, ptB, GIZMOS_COLOR);
        Debug.DrawLine(ptB, ptC, GIZMOS_COLOR);
        Debug.DrawLine(ptC, ptD, GIZMOS_COLOR);
        Debug.DrawLine(ptD, ptA, GIZMOS_COLOR);

        //bottom horizontal
        Debug.DrawLine(ptE, ptF, GIZMOS_COLOR);
        Debug.DrawLine(ptF, ptG, GIZMOS_COLOR);
        Debug.DrawLine(ptG, ptH, GIZMOS_COLOR);
        Debug.DrawLine(ptH, ptE, GIZMOS_COLOR);

        //vertical
        Debug.DrawLine(ptA, ptE, GIZMOS_COLOR);
        Debug.DrawLine(ptB, ptF, GIZMOS_COLOR);
        Debug.DrawLine(ptC, ptG, GIZMOS_COLOR);
        Debug.DrawLine(ptD, ptH, GIZMOS_COLOR);
    }

    /// <returns>A random position across the terrain</returns>
    protected virtual Vector3 GeneratePosition() {
        float xLen = Confine.Offset.x + Confine.Size.x;
        float yLen = Confine.Offset.y + Confine.Size.y;
        float zLen = Confine.Offset.z + Confine.Size.z;
        float x = Random.Range(Confine.Offset.x, xLen);
        float y = Random.Range(Confine.Offset.y, yLen);
        float z = Random.Range(Confine.Offset.z, zLen);
        return new Vector3(x, y, z);
    }
}