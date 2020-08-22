using UnityEngine;

public static class BordersCreator
{
    /// <summary>
    /// Create a single edge border.
    /// </summary>
    /// <param name="area">The confined area to limit</param>
    /// <param name="parentObj">The object to which the box collider components will be added</param>
    /// <param name="edgeOffset">Position of the edge</param>
    /// <param name="edgeSize">The edge's box size</param>
    /// <param name="isTrigger">True to set the collider as 'trigger'</param>
    /// <returns>The create BoxCollider component</returns>
    private static BoxCollider Edge(ConfinedArea area, GameObject parentObj, Vector3 edgeOffset, Vector3 edgeSize, bool isTrigger) {
        BoxCollider edge = parentObj.AddComponent<BoxCollider>();
        edge.size = edgeSize;
        edge.center = area.Confine.Offset + edgeSize / 2 + edgeOffset;
        edge.isTrigger = isTrigger;
        return edge;
    }

    /// <summary>
    /// Create 4 borders around the sides of a confined area.
    /// </summary>
    /// <param name="area">The confined area to limit</param>
    /// <param name="cubical">True to limit all six cubical edges, or false to only limit the four sides</param>
    /// <param name="excessDepth">
    /// The depth of each border edge
    /// (the more depth, the less mistakes made in identification).
    /// </param>
    /// <param name="triggerBorder">True to set each edge collider as 'trigger'</param>
    /// <param name="parentObj">
    /// The object to which the box collider components will be added.
    /// If null, the confined area ('area' argument) will be defined as parent.
    /// </param>
    /// <returns>A box colliders array of the created borders.</returns>
    public static BoxCollider[] Create(ConfinedArea area, bool cubical, float excessDepth, bool triggerBorder, GameObject parentObj = null) {
        if (parentObj == null) parentObj = area.gameObject;

        int edgesAmount = cubical ? 6 : 4;
        BoxCollider[] borders = new BoxCollider[edgesAmount];

        Vector3 xExcess = new Vector3(excessDepth, 0, 0);
        Vector3 yExcess = new Vector3(0, excessDepth, 0);
        Vector3 zExcess = new Vector3(0, 0, excessDepth);

        Vector3[] boxSizes = {
            Vector3.Scale(area.Confine.Size, Vector3.right + Vector3.up) + zExcess,
            Vector3.Scale(area.Confine.Size, Vector3.forward + Vector3.up) + xExcess,
            Vector3.Scale(area.Confine.Size, Vector3.right + Vector3.up) + zExcess,
            Vector3.Scale(area.Confine.Size, Vector3.forward + Vector3.up) + xExcess,

            //cubical addition
            Vector3.Scale(area.Confine.Size, Vector3.right + Vector3.forward) + yExcess,
            Vector3.Scale(area.Confine.Size, Vector3.right + Vector3.forward) + yExcess
        };

         Vector3[] centerPointsOffset = {
            new Vector3(0, 0, -boxSizes[0].z),
            new Vector3(-boxSizes[1].x, 0, 0),
            new Vector3(0, 0, area.Confine.Size.z),
            new Vector3(area.Confine.Size.x, 0, 0),

            //cubical addition
            new Vector3(0, area.Confine.Size.y, 0),
            new Vector3(0, -boxSizes[5].y, 0)
        };

        for (int i = 0; i < edgesAmount; i++) {
            Vector3 size = boxSizes[i];
            Vector3 centerOffset = centerPointsOffset[i];
            borders[i] = Edge(area, parentObj, centerOffset, size, triggerBorder);
        }

        return borders;
    }
}