using UnityEngine;

public class ConfinedArea : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Area Definition")]
    [Tooltip("The field confines' offset and size.")]
    [SerializeField] public Confine Confine;
    #endregion

    #region Constants
    protected static readonly Color GIZMOS_COLOR = new Color(0xff, 0x0, 0xe8);
    #endregion

    #region Properties
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
    #endregion

    protected virtual void OnDrawGizmos() {
        Gizmos.color = GIZMOS_COLOR;
        Gizmos.DrawWireCube(AreaCenter, Confine.Size);
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