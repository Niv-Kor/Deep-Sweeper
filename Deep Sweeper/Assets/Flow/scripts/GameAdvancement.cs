using System.Collections.Generic;

public class GameAdvancement : Singleton<GameAdvancement>
{
    private class IndicesComparer : Comparer<int>
    {
        /// <inheritdoc/>
        public override int Compare(int x, int y) => x > y ? 1 : -1;
    }

    #region Class Members
    private List<int> openLevels;
    #endregion

    private void Awake() {
        this.openLevels = LoadLevels();
    }

    /// <summary>
    /// Load the game's available levels.
    /// </summary>
    /// <returns>A list of the already open levels' indices.</returns>
    private List<int> LoadLevels() {
        List<int> indices = new List<int>();
        indices.Add(0);
        IComparer<int> comparer = new IndicesComparer();
        indices.Sort(comparer);
        return indices;
    }

    /// <summary>
    /// Check if a level is already open and available.
    /// </summary>
    /// <param name="index">The index of the level</param>
    /// <returns>True if the level is available.</returns>
    public bool IsLevelOpen(int index) {
        return openLevels.Contains(index);
    }

    /// <summary>
    /// Check if a level is the next objective level,
    /// that is, it's the next level in the campaign.
    /// </summary>
    /// <param name="index">The index of the level</param>
    /// <returns>True if the level is the objective level.</returns>
    public bool IsObjectiveLevel(int index) {
        int lastIndex = openLevels.Count - 1;
        return lastIndex >= 0 && openLevels[lastIndex] == index;
    }
}