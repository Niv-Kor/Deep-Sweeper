using UnityEngine;

public class MapBorders : ConfinedArea
{
    #region Exposed Editor Parameters
    [Tooltip("True to automatically find the terrain's confines.")]
    [SerializeField] private bool autoFindTerrain;

    [Tooltip("The height of the map's ceiling.")]
    [SerializeField] private float ceilingLimit = 20;

    [Tooltip("The height of the map's floor.")]
    [SerializeField] private float floorLimit = 0;

    [Tooltip("Percentage of the map's radius at which the borders start. "
           + "Setting it to 0 will cause the borders to be established the map's center point, "
           + "while setting it to 1 will cause them to be established at its furthest edges.")]
    [SerializeField] [Range(0f, 1f)] private float mapRadiusPercent = 1;
    #endregion

    #region Constants
    private static readonly float BORDERS_DEPTH = 50f;
    private static readonly Color GIZMOS_CEILING_COLOR = Color.green;
    private static readonly Color GIZMOS_FLOOR_COLOR = Color.red;
    #endregion

    #region Class Members
    private Terrain terrain;
    #endregion

    private void Awake() {
        if (autoFindTerrain) this.terrain = FindObjectOfType<Terrain>();
    }

    private void Start() {
        Vector3 position;
        Vector3 size;

        if (autoFindTerrain) {
            position = terrain.transform.position;
            size = terrain.terrainData.size;
        }
        else {
            position = Confine.Offset;
            size = Confine.Size;
        }

        Vector3 floorOffset = -Vector3.up * floorLimit;
        Vector3 yNullification = Vector3.right + Vector3.forward;
        Vector3 radiusPercentOffset = Vector3.Scale(size * (1 - mapRadiusPercent), yNullification) / 2;
        Confine.Offset = position + floorOffset + radiusPercentOffset;
        Confine.Size = size * mapRadiusPercent;
        Confine.Size.y = ceilingLimit - floorOffset.y;
        BordersCreator.Create(this, true, BORDERS_DEPTH, false);
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        Vector3 ceilingOffset = Vector3.up * ceilingLimit;
        Vector3 floorOffset = -Vector3.up * floorLimit;
        Vector3 ceilingPos = Confine.Offset + ceilingOffset;
        Vector3 floorPos = Confine.Offset + floorOffset;
        Vector3 size = Vector3.Scale(Vector3.one * 20, Vector3.right + Vector3.forward);

        Gizmos.color = GIZMOS_CEILING_COLOR;
        Gizmos.DrawWireCube(ceilingPos, size);

        Gizmos.color = GIZMOS_FLOOR_COLOR;
        Gizmos.DrawWireCube(floorPos, size);
    }
}