using UnityEngine;

public static class ChanceUtils
{
    /// <summary>
    /// Generate a random boolean output.
    /// </summary>
    /// <param name="chance">The chance of the answer being True [0:1]</param>
    /// <returns>A random boolean output, based on the specified chance.</returns>
    public static bool UnstableCondition(float chance) {
        if (chance > 1) chance /= 100f;

        switch (chance) {
            case 0: return false;
            case 1: return true;
            default: return Random.value <= chance;
        }
    }

    /// <summary>
    /// Generate a random boolean answer.
    /// </summary>
    /// <returns>A completely random boolean output.</returns>
    public static bool UnstableCondition() {
        return UnstableCondition(Random.value);
    }
}