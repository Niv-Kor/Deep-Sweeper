using DeepSweeper.Data;
using DeepSweeper.Flow;
using GamedevUtil.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DataPool
{
    private struct PhaseData
    {
        public string MapName;
        public float MinePercent;
        public int Reward;
    }

    private struct RegionMetaData
    {
        public Region Region;
        public int Index;
        public string MissionBrief;
    }

    private struct LevelData
    {
        public Region Region;
        public DifficultyLevel Difficulty;
        public List<PhaseData> Phases;
        public int TotalReward;
    }

    #region Class Members
    private List<LevelData> levelData;
    private List<RegionMetaData> regionMetaData;
    private List<Procedure> procedures;
    #endregion

    /// <summary>
    /// Load all of the game's data.
    /// </summary>
    /// <param name="procedures">An object consisting of procedure components as its children</param>
    public async Task Load(GameObject procedures) {
        var procsArr = procedures.GetComponentsInChildren<Procedure>();
        this.procedures = new List<Procedure>(procsArr);
        await LoadRegions();
    }

    /// <summary>
    /// Get a reference to a specific procedure.
    /// </summary>
    /// <param name="type">The procedure's class</param>
    /// <returns>A reference to the procedure object</returns>
    private Procedure GetProcedure(Type type) {
        return procedures.Find(x => x.GetType() == type);
    }

    /// <summary>
    /// Load all regional data.
    /// </summary>
    /// <returns></returns>
    private async Task LoadRegions() {
        levelData = new List<LevelData>();
        regionMetaData = new List<RegionMetaData>();
        var getPhasesProc = GetProcedure(typeof(SQLProcGetPhasesData)) as SQLProcGetPhasesData;
        var getRegionMetaProc = GetProcedure(typeof(SQLProcGetRegionMetaData)) as SQLProcGetRegionMetaData;

        foreach (Region region in Enum.GetValues(typeof(Region))) {
            if (region == Region.None) continue;
            string regionStr = region.ToString().Replace('_', ' ');

            //fetch region meta data
            var regionMetaReq = new GetRegionMetaDataRequest(regionStr);
            var regionMetaRes = await getRegionMetaProc.Run(regionMetaReq);
            RegionMetaData regionMeta;
            regionMeta.Region = region;
            regionMeta.Index = regionMetaRes.Index;
            regionMeta.MissionBrief = regionMetaRes.Brief;
            regionMetaData.Add(regionMeta);

            foreach (DifficultyLevel diff in Enum.GetValues(typeof(DifficultyLevel))) {
                string diffStr = diff.ToString().ToLower();
                LevelData regionalData;
                regionalData.Region = region;
                regionalData.Difficulty = diff;
                regionalData.Phases = new List<PhaseData>();
                regionalData.TotalReward = 0;

                //fetch phases data
                var phaseDataReq = new GetPhasesDataRequest(regionStr, diffStr);
                var pahseDataRes = await getPhasesProc.Run(phaseDataReq);

                foreach (GetPhasesDataPhaseInfo phase in pahseDataRes.Phases) {
                    PhaseData phaseData;
                    phaseData.MapName = phase.MapName;
                    phaseData.MinePercent = phase.MinePercent;
                    phaseData.Reward = phase.Reward;
                    regionalData.TotalReward += phaseData.Reward;
                    regionalData.Phases.Add(phaseData);
                }
                
                levelData.Add(regionalData);
            }
        }
    }

    /// <param name="region">The context region</param>
    /// <param name="difficulty">The context level of difficulty</param>
    /// <returns>The total reward money for the entire region level</returns>
    public int GetRegionTotalReward(Region region, DifficultyLevel difficulty) {
        try {
            LevelData dataSet = levelData.Find(x => x.Region == region && x.Difficulty == difficulty);
            return dataSet.TotalReward;
        }
        catch (Exception) { return 0; }
    }

    /// <param name="region">The context region</param>
    /// <returns>The index of the region within its district [0:inf).</returns>
    public int GetRegionIndex(Region region) {
        try {
            RegionMetaData dataSet = regionMetaData.Find(x => x.Region == region);
            return dataSet.Index;
        }
        catch (Exception) { return -1; }
    }

    /// <param name="region">The context region</param>
    /// <returns>The region's mission brief text.</returns>
    public string GetRegionMissionBrief(Region region) {
        try {
            RegionMetaData dataSet = regionMetaData.Find(x => x.Region == region);
            return dataSet.MissionBrief;
        }
        catch (Exception) { return ""; }
    }
}