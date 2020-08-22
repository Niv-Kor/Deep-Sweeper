using System.Collections.Generic;
using UnityEngine;

public class MineField : ConfinedArea
{
    [Header("Prefabs")]
    [Tooltip("The grid object.")]
    [SerializeField] private GameObject gridPrefab;

    [Tooltip("Space between grids.")]
    [SerializeField] private float gridSpace;

    [Header("Mines")]
    [Tooltip("Percentage of randomly spreaded mines.")]
    [SerializeField] private int minesPercent;

    private static readonly string GRIDS_PARENT_NAME = "Field";

    private static int fieldIndex = 0;
    private Terrain terrain;
    private MineGrid[,] gridsMatrix;
    private Vector3 gridSize;
    private Vector2 matrixSize;
    private int gridsAmount, minesAmount;

    public List<MineGrid> Grids { get; private set; }

    private void Start() {
        MeshRenderer gridRenderer = gridPrefab.GetComponent<MeshRenderer>();
        this.gridSize = gridRenderer.bounds.size;
        this.terrain = FindObjectOfType<Terrain>();
        this.Grids = new List<MineGrid>();
        this.matrixSize = CalcMatrixSize();
        this.gridsMatrix = new MineGrid[(int) matrixSize.x ,(int) matrixSize.y];
        this.gridsAmount = (int) matrixSize.x * (int) matrixSize.y;
        this.minesAmount = (int) (minesPercent * gridsAmount / 100);

        LayoutMatrix();
        SpreadMines(minesAmount);
        CountNeighbours();
        OpenInitial();
    }

    /// <summary>
    /// Calculate the horizontal and vertical amount of mine grids that
    /// can fit inside the specified field confines.
    /// </summary>
    /// <returns>
    /// A 2-dimensional matrix, where the x coordinates represent
    /// the amount of grids that can fit horizontally,
    /// and the y coordinates represent the amount of grids
    /// that can fit vertically in the field.
    /// </returns>
    private Vector2 CalcMatrixSize() {
        int xAmount = (int) (Confine.Size.x / (gridSize.x + gridSpace));
        int zAmount = (int) (Confine.Size.z / (gridSize.z + gridSpace));
        return new Vector2(zAmount, xAmount);
    }

    /// <summary>
    /// Layout the matrix upon the terrain.
    /// </summary>
    private void LayoutMatrix() {
        gridSize.y = 0;
        Vector3 startPoint = terrain.transform.position + Confine.Offset + gridSize / 2;
        string parentObjName = GRIDS_PARENT_NAME + " (" + (fieldIndex++) + ")";
        GameObject gridsObj = new GameObject(parentObjName);
        gridsObj.transform.SetParent(transform);

        for (int i = 0; i < matrixSize.x; i++) {
            for (int j = 0; j < matrixSize.y; j++) {
                Vector3 diffVector3 = new Vector3(gridSize.x + gridSpace, 0, 0);
                Vector3 point = startPoint + diffVector3 * j;
                GameObject obj = Instantiate(gridPrefab);
                obj.transform.position = point;
                obj.transform.SetParent(gridsObj.transform);

                MineGrid mineGrid = obj.GetComponent<MineGrid>();
                mineGrid.Field = this;
                mineGrid.Position = new Vector2(j, i);
                gridsMatrix[i, j] = mineGrid;
                Grids.Add(mineGrid);
            }

            startPoint += new Vector3(0, 0, gridSize.z + gridSpace);
        }
    }

    /// <summary>
    /// Spread mines randomly in the matrix grids.
    /// </summary>
    /// <param name="amount">Amount of mines to spread</param>
    private void SpreadMines(int amount) {
        amount = Mathf.Min(amount, gridsAmount);
        List<int> numsStock = new List<int>();

        //fill stock with [0:amount) sequence
        for (int i = 0; i < gridsAmount; i++) numsStock.Add(i);

        while (amount-- > 0) {
            int randomIndex = Random.Range(0, numsStock.Count);
            int num = numsStock[randomIndex];
            numsStock.Remove(num);
            Grids[num].IsMined = true;
        }
    }

    /// <summary>
    /// Count the mined neighbours of each grid.
    /// </summary>
    private void CountNeighbours() {
        for (int i = 0; i < matrixSize.x; i++) {
            for (int j = 0; j < matrixSize.y; j++) {
                MineGrid grid = gridsMatrix[i, j];
                List<MineGrid> section = GetSection(i, j);
                int minedNeighbours = section.FindAll(x => x != null && x.IsMined).Count;
                grid.MinesIndicator.MinedNeighbours = minedNeighbours;
            }
        }
    }

    /// <summary>
    /// Intially open a random section of mines-free grids.
    /// </summary>
    private void OpenInitial() {
        if (gridsAmount == 0 || minesAmount >= gridsAmount) return;

        //fill indices pool
        List<int> indicesPool = new List<int>();
        for (int i = 0; i < gridsAmount; i++) indicesPool.Add(i);

        MineGrid grid;
        bool lowerStandard = false;

        do {
            if (!lowerStandard && indicesPool.Count <= 0) {
                //fill pool again and now ignore the mined neighbours condition
                for (int i = 0; i < gridsAmount; i++) indicesPool.Add(i);
                lowerStandard = true;
            }

            int poolIndex = Random.Range(0, indicesPool.Count);
            int gridIndex = indicesPool[poolIndex];
            indicesPool.RemoveAt(poolIndex);
            grid = Grids[gridIndex];
        }
        while (grid.IsMined || (grid.MinesIndicator.MinedNeighbours != 0 && !lowerStandard));

        grid.Reveal(false);
    }

    /// <summary>
    /// Get a section of 8 neighbours around a grid.
    /// If a neighbour does not exist, it's replaced with null.
    /// </summary>
    /// <param name="row">Row index of the center grid in the section</param>
    /// <param name="col">Column index of the center grid in the section</param>
    /// <returns>
    /// A list of the section, indexed as followed:
    /// [0][1][2]
    /// [3]   [4]
    /// [5][6][7]
    /// </returns>
    public List<MineGrid> GetSection(int row, int col) {
        return new List<MineGrid> {
            GetGrid(row - 1, col - 1),
            GetGrid(row - 1, col),
            GetGrid(row - 1, col + 1),
            GetGrid(row, col - 1),
            GetGrid(row, col + 1),
            GetGrid(row + 1, col - 1),
            GetGrid(row + 1, col),
            GetGrid(row + 1, col + 1)
        };
    }

    /// <summary>
    /// Get a specific grid from the matrix.
    /// </summary>
    /// <param name="row">Row index of the grid</param>
    /// <param name="col">Column index of the grid</param>
    /// <returns>The specified grid.</returns>
    private MineGrid GetGrid(int row, int col) {
        bool innerRow = row >= 0 && row < matrixSize.x;
        bool innerCol = col >= 0 && col < matrixSize.y;

        if (innerRow && innerCol) return gridsMatrix[row, col];
        else return null;
    }
}