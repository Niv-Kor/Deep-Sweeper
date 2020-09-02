using UnityEngine;

public static class RangeMath
{
    /// <summary>
    /// Calculate the percentage of a number in a range.
	/// Eg: 49 ('number') within the range of [5, 50] is 97.77%.
    /// </summary>
    /// <param name="number">The number of which to take the percentage</param>
    /// <param name="range">Minimum and maximum values of the range</param>
    /// <param name="scale100">
    /// True to scale the result to fit within the range of [0:100].
    /// Default is within the range of [0:1].
    /// </param>
    /// <returns>The percentage of the specified number within the given range.</returns>
    public static float NumberOfRange(float number, Vector2 range, bool scale100 = false) {
        return NumberOfRange(number, range.x, range.y, scale100);
    }

    /// <summary>
    /// Calculate the percentage of a number in a range.
	/// Eg: 49 ('number') within the range of 5-50 is 97.77%.
    /// </summary>
    /// <param name="number">The number of which to take the percentage</param>
    /// <param name="rangeMin">Minimum number in the range</param>
    /// <param name="rangeMax">Maximum number in the range</param>
    /// <param name="scale100">
    /// True to scale the result to fit within the range of [0:100].
    /// Default is within the range of [0:1].
    /// </param>
    /// <returns>The percentage of the specified number within the given range.</returns>
    public static float NumberOfRange(float number, float rangeMin, float rangeMax, bool scale100 = false) {
        float scale = scale100 ? 100f : 1;
        float diff = rangeMax - rangeMin;
        return (number - rangeMin) / diff * scale;
    }

    /// <summary>
    /// Calculate the number that is X% of a range.
	/// Eg: 0% ('percent') of the range [12, 100] is the number 12.
    /// </summary>
    /// <param name="percent">The percentage to take from the range</param>
    /// <param name="range">Minimum and maximum values of the range</param>
    /// <param name="scale100">
    /// True if the percentage is within the range of [0:100],
    /// or false if it's within the range of [0:1].
    /// </param>
    /// <returns>The number that specifies the exact given percentage of the range.</returns>
    public static float PercentOfRange(float percent, Vector2 range, bool scale100 = false) {
        return PercentOfRange(percent, range.x, range.y, scale100);
    }

    /// /// <summary>
    /// Calculate the number that is X% of a range.
	/// Eg: 0% ('percent') of the range [12, 100] is the number 12.
    /// </summary>
    /// <param name="percent">The percentage to take from the range</param>
    /// <param name="rangeMin">Minimum number in the range</param>
    /// <param name="rangeMax">Maximum number in the range</param>
    /// <param name="scale100">
    /// True if the percentage is within the range of [0:100],
    /// or false if it's within the range of [0:1].
    /// </param>
    /// <returns>The number that specifies the exact given percentage of the range.</returns>
    public static float PercentOfRange(float percent, float rangeMin, float rangeMax, bool scale100 = false) {
        float scale = scale100 ? 100f : 1;
        float diff = rangeMax - rangeMin;
        return percent * diff / scale + rangeMin;
    }

    /// /// <summary>
    /// Calculate the number that is X% of a vector range.
	/// Eg: 50% ('percent') of the vectors range (10, 4, 20) and (50, 40, 50) is the vector (30, 22, 35).
    /// </summary>
    /// <param name="percent">The percentage to take from the range</param>
    /// <param name="rangeMin">Minimum vector in the range</param>
    /// <param name="rangeMax">Maximum vector in the range</param>
    /// <param name="scale100">
    /// True if the percentage is within the range of [0:100],
    /// or false if it's within the range of [0:1].
    /// </param>
    /// <returns>The number that specifies the exact given percentage of the range.</returns>
    public static Vector3 PercentOfVectorRange(float percent, Vector3 rangeMin, Vector3 rangeMax, bool scale100 = false) {
        Vector2 xRange = new Vector2(rangeMin.x, rangeMax.x);
        Vector2 yRange = new Vector2(rangeMin.y, rangeMax.y);
        Vector2 zRange = new Vector2(rangeMin.z, rangeMax.z);
        float x = PercentOfRange(percent, xRange, scale100);
        float y = PercentOfRange(percent, yRange, scale100);
        float z = PercentOfRange(percent, zRange, scale100);
        return new Vector3(x, y, z);
    }
}