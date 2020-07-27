using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixLayout : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("The grid object.")]
    [SerializeField] private GameObject gridPrefab = null;

    [Header("Grid Configurations")]
    [Tooltip("The size of the matrix [rows/columns].")]
    [SerializeField] private Vector2 matrixSize;

    [Tooltip("The offset of the top left grid on the terrain.")]
    [SerializeField] private Vector2 firstOffset;

    [Tooltip("Space between grids.")]
    [SerializeField] private float gridSpace;

    [Tooltip("The height of the grid above the ground. Should idealy be slightly larger than 0.")]
    [SerializeField] private float gridHeight = .01f;

    [Header("Mines")]
    [Tooltip("Amount of randomly spreaded mines")]
    [SerializeField] private int minesAmount;

    private static readonly string GRIDS_PARENT_NAME = "Grids";

    private Terrain terrain;
    private MineGrid[,] gridsMatrix;

    public List<MineGrid> Grids { get; private set; }

    private void Start() {
        this.terrain = GetComponent<Terrain>();
        this.Grids = new List<MineGrid>();
        this.gridsMatrix = new MineGrid[(int) matrixSize.x ,(int) matrixSize.y];
        LayoutMatrix();
        SpreadMines(minesAmount);
        CountNeighbours();
    }

    /// <summary>
    /// Layout the matrix upon the terrain.
    /// </summary>
    private void LayoutMatrix() {
        MeshRenderer gridRenderer = gridPrefab.GetComponent<MeshRenderer>();
        Vector3 offset = new Vector3(firstOffset.x, gridHeight, firstOffset.y);
        Vector3 gridSize = gridRenderer.bounds.size;
        gridSize.y = 0;
        Vector3 startPoint = terrain.transform.position + offset + gridSize / 2;
        GameObject gridsObj = new GameObject(GRIDS_PARENT_NAME);
        gridsObj.transform.SetParent(terrain.transform);

        for (int i = 0; i < matrixSize.x; i++) {
            for (int j = 0; j < matrixSize.y; j++) {
                Vector3 diffVector3 = new Vector3(gridSize.x + gridSpace, 0, 0);
                Vector3 point = startPoint + diffVector3 * j;
                GameObject obj = Instantiate(gridPrefab);
                obj.transform.position = point;
                obj.transform.SetParent(gridsObj.transform);

                MineGrid mineGrid = obj.GetComponent<MineGrid>();
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
        int gridsAmount = Grids.Count;
        amount = Mathf.Min(amount, gridsAmount);
        HashSet<int> randomSet = new HashSet<int>();

        while (amount-- > 0) {
            int randomIndex;
            do randomIndex = Random.Range(0, gridsAmount - 1);
            while (randomSet.Contains(randomIndex));
            randomSet.Add(randomIndex);
            Grids[randomIndex].IsMined = true;
        }
    }

    /// <summary>
    /// Count the mined neighbours of each grid.
    /// </summary>
    private void CountNeighbours() {
        for (int i = 0; i < matrixSize.x; i++) {
            for (int j = 0; j < matrixSize.y; j++) {
                MineGrid grid = gridsMatrix[i, j];
                if (grid.IsMined) continue;

                List<MineGrid> section = GetSection(i, j);
                int minedNeighbours = section.FindAll(x => x != null && x.IsMined).Count;
                grid.Indicator = minedNeighbours;
            }
        }
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
    private List<MineGrid> GetSection(int row, int col) {
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