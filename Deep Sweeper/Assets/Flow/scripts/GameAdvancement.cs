using System.Collections.Generic;

public class GameAdvancement : Singleton<GameAdvancement>
{
    #region Class Members
    private List<int> openLevels;
    #endregion

    private void Start() {
        this.openLevels = LoadLevels();
    }

    /// <summary>
    /// Load the already open levels' indices.
    /// </summary>
    private List<int> LoadLevels() {
        List<int> indices = new List<int>();
        indices.Add(0);
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
}