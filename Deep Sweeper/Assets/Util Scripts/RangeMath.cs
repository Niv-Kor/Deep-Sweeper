using UnityEngine;

public static class RangeMath
{
    /// <summary>
    /// Calculate the number that is X% of a range.
	/// Eg: 0% ('percent') of the range [12, 100] is the number 12.
    /// </summary>
    /// <param name="percent">The percentage to take from the range</param>
    /// <param name="range">Minimum and maximum values of the range</param>
    /// <param name="scale100">
    /// True to scale the result from 0 to 100.
    /// Default is 0 to 1.
    /// </param>
    /// <returns>The number that specifies the exact given percentage of the range.</returns>
    public static float PercentOfRange(float percent, Vector2 range, bool scale100 = false) {
        return PercentOfRange(percent, range.x, range.y, scale100);
    }

    /// <summary>
    /// Calculate the number that is X% of a range.
	/// Eg: 0% ('percent') of the range [12, 100] is the number 12.
    /// </summary>
    /// <param name="percent">The percentage to take from the range</param>
    /// <param name="rangeMin">Minimum number in the range</param>
    /// <param name="rangeMax">Maximum number in the range</param>
    /// <param name="scale100">
    /// True to scale the result from 0 to 100.
    /// Default is 0 to 1.
    /// </param>
    /// <returns>The number that specifies the exact given percentage of the range.</returns>
    public static float PercentOfRange(float percent, float rangeMin, float rangeMax, bool scale100 = false) {
        float scale = scale100 ? 100f : 1;
        float diff = rangeMax - rangeMin;
        return (percent - rangeMin) / diff * scale;
    }

    /**
	 * Calculate the number that X% of the entire range is.
	 * (Ex. 0% (percent) of 12-100 is 12).
	 * 
	 * @param percent - The percentage
	 * @return a number that is X% of the entire range. 
	 */

    /// <summary>
    /// Calculate the percentage of a number in a range.
	/// Eg: 49 ('number') within the range of [5, 50] is 97.77%.
    /// </summary>
    /// <param name="number">The number of which to take the percentage</param>
    /// <param name="range">Minimum and maximum values of the range</param>
    /// <param name="scale100">
    /// True to scale the result from 0 to 100.
    /// Default is 0 to 1.
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
    /// True to scale the result from 0 to 100.
    /// Default is 0 to 1.
    /// </param>
    /// <returns>The percentage of the specified number within the given range.</returns>
    public static float NumberOfRange(float number, float rangeMin, float rangeMax, bool scale100 = false) {
        float scale = scale100 ? 100f : 1;
        float diff = rangeMax - rangeMin;
        return number * diff / scale + rangeMin;
    }
}