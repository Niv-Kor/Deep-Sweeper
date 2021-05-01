using UnityEngine;

public abstract class MineSystem : MonoBehaviour
{
    #region Properties
    protected MineGrid Grid;
    #endregion

    /// <param name="grid">This system's grid</param>
    public void SetParentGrid(MineGrid grid) { Grid = grid; }
}