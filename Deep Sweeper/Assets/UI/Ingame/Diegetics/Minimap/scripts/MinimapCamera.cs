using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MinimapCamera : MonoBehaviour
{
    #region Class Members
    private Transform player;
    private Terrain terrain;
    private Camera cam;
    private float height;
    private Vector3 positionMask;
    #endregion

    #region Properties
    public Vector3 Position {
        get {
            Vector3 mask = Vector3.Scale(player.position, positionMask);
            return mask + Vector3.up * height;
        }
    }
    #endregion

    private void Start() {
        this.player = Submarine.Instance.transform;
        this.terrain = FindObjectOfType<Terrain>();
        this.cam = GetComponent<Camera>();
        this.positionMask = Vector3.right + Vector3.forward;
        this.height = terrain.terrainData.size.y * 2;
        float terrainHeight = terrain.transform.position.y;
        cam.farClipPlane = height - terrainHeight;
        transform.position += Vector3.up * height / 2;
    }
}