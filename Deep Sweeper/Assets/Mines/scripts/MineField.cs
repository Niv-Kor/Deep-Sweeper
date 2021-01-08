﻿using Constants;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MineField : ConfinedArea
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The grid object.")]
    [SerializeField] private GameObject gridPrefab;

    [Tooltip("Space between grids.")]
    [SerializeField] private float gridSpace;
    #endregion

    #region Constants
    private static readonly float COIN_VALUE_EPSILON_PERCENT = .5f;
    #endregion

    #region Class Members
    private Terrain terrain;
    private MineGrid[,] gridsMatrix;
    private Vector3 gridSize;
    private float raycastHeight;
    private int gridsAmount;
    #endregion

    #region Events
    public event UnityAction FieldActivatedEvent;
    #endregion

    #region Public Properties
    public List<MineGrid> Grids { get; private set; }
    public int MinesAmount { get; private set; }
    public int TotalReward { get; private set; }
    public bool IsActivated { get; private set; }
    public Vector2Int MatrixSize { get; private set; }
    public Vector3 Center {
        get {
            float height = terrain.terrainData.size.y;
            return Confine.Offset + Confine.Size / 2 + Vector3.up * height / 2;
        }
    }
    #endregion

    private void Awake() {
        MeshRenderer gridRenderer = gridPrefab.GetComponent<MeshRenderer>();
        this.gridSize = gridRenderer.bounds.size;
        this.terrain = WaterPhysics.Instance.Terrain;
        this.Grids = new List<MineGrid>();
        this.raycastHeight = terrain.terrainData.size.y;
        this.IsActivated = false;
    }

    /// <summary>
    /// Set the usable area of the field.
    /// </summary>
    /// <param name="area">A confined area that can be used by the field</param>
    public void DefineArea(Confine area) {
        this.Confine = area;
        this.MatrixSize = CalcMatrixSize();
        this.gridsMatrix = new MineGrid[MatrixSize.x, MatrixSize.y];
        this.gridsAmount = MatrixSize.x * MatrixSize.y;
        LayoutMatrix();
    }

    /// <summary>
    /// Initialize all field components.
    /// This method only works after defining the field's area.
    /// <see cref="DefineArea(Confine)"/>
    /// </summary>
    /// <param name="difficultyConfig">The difficulty configuration of this field's phase</param>
    public void Init(PhaseDifficultyConfig difficultyConfig) {
        //find mines amount
        float minesPercent = difficultyConfig.MinesPercent / 100f;
        MinesAmount = (int) (minesPercent * gridsAmount);

        //init field
        SpreadMines(MinesAmount);
        CountNeighbours();
        OpenInitial();
        GenerateLootValues();
        CarpetBounce();
    }

    /// <summary>
    /// Activate the field.
    /// If this method is not called, the field grids
    /// will not be interactable by the player.
    /// </summary>
    public void Activate() {
        ActivateGrids();
        IsActivated = true;
        FieldActivatedEvent?.Invoke();
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
    private Vector2Int CalcMatrixSize() {
        int xAmount = (int) (Confine.Size.x / (gridSize.x + gridSpace));
        int zAmount = (int) (Confine.Size.z / (gridSize.z + gridSpace));
        return new Vector2Int(zAmount, xAmount);
    }

    /// <summary>
    /// Layout the matrix upon the terrain.
    /// </summary>
    private void LayoutMatrix() {
        gridSize.y = 0;
        Vector3 startPoint = terrain.transform.position + Confine.Offset + gridSize / 2;

        for (int i = 0; i < MatrixSize.x; i++) {
            for (int j = 0; j < MatrixSize.y; j++) {
                Vector3 diffVector = new Vector3(gridSize.x + gridSpace, 0, 0);
                Vector3 point = startPoint + diffVector * j;

                //get the terrain height at the specified point
                Vector3 raycastPos = point + Vector3.up * raycastHeight;
                Physics.Raycast(raycastPos, Vector3.down, out RaycastHit rayHit, raycastHeight * 1.2f, Layers.GROUND);
                point.y = rayHit.point.y;

                GameObject obj = Instantiate(gridPrefab);
                obj.transform.position = point;
                obj.transform.SetParent(transform);

                MineGrid mineGrid = obj.GetComponent<MineGrid>();
                mineGrid.Field = this;
                mineGrid.Position = new Vector2Int(i, j);

                //save grid
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
        for (int i = 0; i < MatrixSize.x; i++) {
            for (int j = 0; j < MatrixSize.y; j++) {
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
        if (gridsAmount == 0 || MinesAmount >= gridsAmount) return;

        //fill indices pool
        List<int> indicesPool = new List<int>();
        for (int i = 0; i < gridsAmount; i++) indicesPool.Add(i);

        MineGrid grid;
        bool lowerStandard = false;

        do {
            //fill pool again and now ignore the mined neighbours condition
            if (!lowerStandard && indicesPool.Count == 0) {
                for (int i = 0; i < gridsAmount; i++) indicesPool.Add(i);
                lowerStandard = true;
            }

            int poolIndex = Random.Range(0, indicesPool.Count);
            int gridIndex = indicesPool[poolIndex];
            indicesPool.RemoveAt(poolIndex);
            grid = Grids[gridIndex];
        }
        while (grid.IsMined || (grid.MinesIndicator.MinedNeighbours != 0 && !lowerStandard));
        grid.TriggerHit(BulletHitType.SingleHit, false, false);
    }

    /// <summary>
    /// Activate the field grids so the player can interact with them.
    /// </summary>
    private void ActivateGrids() {
        int mineLayer = Layers.GetLayerValue(Layers.MINE);

        foreach (MineGrid grid in Grids)
            grid.Avatar.gameObject.layer = mineLayer;
    }

    /// <summary>
    /// Bounce the mine using a carpet pattern.
    /// </summary>
    private void CarpetBounce() {
        List<MineBouncer> bouncers = new List<MineBouncer>();

        foreach (MineGrid grid in Grids)
            bouncers.Add(grid.GetComponentInChildren<MineBouncer>());

        //bounce all mines with a fixed delay between them
        float delay = 0;
        foreach (MineBouncer mine in bouncers) {
            mine.Bounce(delay);
            delay += .1f;
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
        bool internalRow = row >= 0 && row < MatrixSize.x;
        bool internalCol = col >= 0 && col < MatrixSize.y;

        if (internalRow && internalCol) return gridsMatrix[row, col];
        else return null;
    }

    /// <summary>
    /// Check if this mine field contains a certain grid.
    /// </summary>
    /// <param name="grid">The grid to check</param>
    /// <returns>True if this mine field contains the given grid.</returns>
    public bool ContainsGrid(MineGrid grid) {
        Vector2Int gridLayout = grid.Position;
        MineGrid detectedGrid = GetGrid(gridLayout.x, gridLayout.y);
        return grid != null && detectedGrid == grid;
    }

    /// <summary>
    /// Generate and distribute coin values across the grids.
    /// </summary>
    public void GenerateLootValues() {
        List<MineField> allFields = (from Phase phase in GameFlow.Instance.Phases
                                     select phase.Field).ToList();
        int allMissionGrids = allFields.Sum(x => x.gridsAmount);
        float fieldGridsPercent = (float) gridsAmount / allMissionGrids;
        TotalReward = (int) (fieldGridsPercent * Contract.Instance.BasePayment);

        //copunt the grids that will drop a loot
        List<MineGrid> droppingGrids = new List<MineGrid>();
        while (droppingGrids.Count == 0) {
            foreach (MineGrid grid in Grids) {
                if (grid.IsMined || grid.Sweeper.IsDismissed) continue;

                LootGeneratorObject generator = grid.LootGenerator;
                if (generator.WillDrop) droppingGrids.Add(grid);
            }

            //reroll all
            if (droppingGrids.Count == 0) {
                foreach (MineGrid grid in Grids) {
                    LootGeneratorObject generator = grid.LootGenerator;
                    generator.RerollChance();
                }
            }
        }

        //each grid should drop a coin with a value of at least 1
        TotalReward = Mathf.Max(TotalReward, droppingGrids.Count);

        //distribute reward values unevenly
        int remainAmount = TotalReward;
        int droppingGridsAmount = droppingGrids.Count;
        int avgAmount = remainAmount / droppingGridsAmount;
        bool positiveEpsilon = true;
        float selectedEpsilon = 0;

        for (int i = 0; i < droppingGridsAmount; i++) {
            int selectedAmount = 0;

            //last value to distribute
            if (i == droppingGridsAmount - 1) selectedAmount = remainAmount;
            else {
                //generate new epsilon
                if (positiveEpsilon)
                    selectedEpsilon = Random.Range(0, COIN_VALUE_EPSILON_PERCENT);

                positiveEpsilon ^= true;
                int epsilonMultiplier = positiveEpsilon ? 1 : -1;
                int epsilon = (int) (selectedEpsilon * avgAmount);
                selectedAmount = avgAmount + epsilon * epsilonMultiplier;
                remainAmount -= selectedAmount;
            }

            droppingGrids[i].LootGenerator.ItemValue = selectedAmount;
        }
    }

    /// <summary>
    /// Check if the field is clear, and only left with real mines.
    /// </summary>
    /// <returns>True if all dismissable mines are indeed dismissed.</returns>
    public bool IsClear() {
        if (!IsActivated) return false;

        foreach (MineGrid grid in Grids)
            if (!grid.IsMined && !grid.Sweeper.IsDismissed) return false;

        return true;
    }

    /// <summary>
    /// Destroy this field object.
    /// </summary>
    public void DestroyField() { Destroy(gameObject); }
}