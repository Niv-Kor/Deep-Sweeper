using System.Collections.Generic;
using UnityEngine;

public static class VectorUtils
{
    /// <summary>
    /// Check if a vector has already reached a certain percentage of the distance it should make.
    /// </summary>
    /// <param name="pos">The current position of the vector</param>
    /// <param name="destPos">The position that the vector should finally reach</param>
    /// <param name="distance">The total distance that's needed to be done</param>
    /// <param name="tolerance">A percentage of the total distance</param>
    /// <returns>True if the vector has already reached the specified percent of the distance.</returns>
    public static bool EffectivelyReached(Vector3 pos, Vector3 destPos, float distance, float tolerance) {
        return EffectivelyReached(pos, destPos, tolerance * distance / 100);
    }

    /// <summary>
    /// Check if a vector has already reached a certain percentage of the distance it should make.
    /// </summary>
    /// <param name="pos">The current position of the vector</param>
    /// <param name="destPos">The position that the vector should finally reach</param>
    /// <param name="toleranceUnits">A distance between the two vectors that can be ignored</param>
    /// <returns>True if the vector has already reached the specified percent of the distance.</returns>
    public static bool EffectivelyReached(Vector3 pos, Vector3 destPos, float toleranceUnits) {
        float dist = Vector3.Distance(pos, destPos);
        return dist <= toleranceUnits;
    }

    /// <summary>
    /// Clamp a vector's coordinates.
    /// Each axis is only compared against itself in the min/max vectors.
    /// </summary>
    /// <param name="vector">The vector to clamp</param>
    /// <param name="min">Minimum vactor values</param>
    /// <param name="max">Maximum vector values</param>
    /// <returns>The given vector with clamped values.</returns>
    public static Vector2 Clamp(Vector2 vector, Vector2 min, Vector2 max) {
        float x = Mathf.Clamp(vector.x, min.x, max.x);
        float y = Mathf.Clamp(vector.y, min.y, max.y);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Clamp a vector's coordinates.
    /// Each axis is only compared against itself in the min/max vectors.
    /// </summary>
    /// <param name="vector">The vector to clamp</param>
    /// <param name="min">Minimum vactor values</param>
    /// <param name="max">Maximum vector values</param>
    /// <returns>The given vector with clamped values.</returns>
    public static Vector3 Clamp(Vector3 vector, Vector3 min, Vector3 max) {
        float x = Mathf.Clamp(vector.x, min.x, max.x);
        float y = Mathf.Clamp(vector.y, min.y, max.y);
        float z = Mathf.Clamp(vector.z, min.z, max.z);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Generate a random rotation.
    /// </summary>
    /// <returns>A random rotation quaternion.</returns>
    public static Quaternion GenerateRotation(Vector2? xLim = null, Vector2? yLim = null, Vector2? zLim = null) {
        Vector2 xLimFinal = xLim ?? new Vector2(0, 360);
        Vector2 yLimFinal = yLim ?? new Vector2(0, 360);
        Vector2 zLimFinal = zLim ?? new Vector2(0, 360);

        float pitch = Random.Range(xLimFinal.x, xLimFinal.y);
        float yaw = Random.Range(yLimFinal.x, yLimFinal.y);
        float roll = Random.Range(zLimFinal.x, zLimFinal.y);
        Vector3 euler = new Vector3(pitch, yaw, roll);
        return Quaternion.Euler(euler);
    }

    public static Vector3Int StrongNormalize(this Vector3 vector) {
        int x = (vector.x > 0) ? 1 : (vector.x < 0) ? -1 : 0;
        int y = (vector.y > 0) ? 1 : (vector.y < 0) ? -1 : 0;
        int z = (vector.z > 0) ? 1 : (vector.z < 0) ? -1 : 0;
        return new Vector3Int(x, y, z);
    }

    public static Vector2Int StrongNormalize(this Vector2 vector) {
        return (Vector2Int) ((Vector3) vector).StrongNormalize();
    }

    public static bool SameSign(this Vector3 v1, Vector3 v2) {
        bool x = (v1.x == 0 && v2.x == 0) || (v1.x != 0) && Mathf.Sign(v1.x) == Mathf.Sign(v2.x);
        bool y = (v1.y == 0 && v2.y == 0) || (v1.y != 0) && Mathf.Sign(v1.y) == Mathf.Sign(v2.y);
        bool z = (v1.z == 0 && v2.z == 0) || (v1.z != 0) && Mathf.Sign(v1.z) == Mathf.Sign(v2.z);
        return x & y & z;
    }

    public static bool SameSign(this Vector2 v1, Vector2 v2) {
        return ((Vector3) v1).SameSign(v2);
    }

    public static bool Greater(this Vector3 v1, Vector3 v2) {
        return v1.Greater(v2, out bool[] _);
    }

    public static bool Greater(this Vector3 v1, Vector3 v2, out bool[] detailedTest) {
        detailedTest = new bool[3] {
            v1.x / v2.x >= 1,
            v1.y / v2.y >= 1,
            v1.z / v2.z >= 1
        };

        bool x = v2.x == 0 || detailedTest[0];
        bool y = v2.y == 0 || detailedTest[1];
        bool z = v2.z == 0 || detailedTest[2];
        return x & y & z;
    }

    public static bool Greater(this Vector2 v1, Vector2 v2) {
        return ((Vector3) v1).Greater(v2);
    }

    public static bool Greater(this Vector2 v1, Vector2 v2, out bool[] detailedTest) {
        return ((Vector3)v1).Greater(v2, out detailedTest);
    }

    public static Vector3 Abs(this Vector3 vector) {
        float x = Mathf.Abs(vector.x);
        float y = Mathf.Abs(vector.y);
        float z = Mathf.Abs(vector.z);
        return new Vector3(x, y, z);
    }

    public static Vector2 Abs(this Vector2 vector) {
        return ((Vector3) vector).Abs();
    }
}