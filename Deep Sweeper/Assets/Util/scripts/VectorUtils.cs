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
}