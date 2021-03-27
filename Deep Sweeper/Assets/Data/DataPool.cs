using DeepSweeper.Data;
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

    private struct RegionalData
    {
        public Region Region;
        public DifficultyLevel Difficulty;
        public List<PhaseData> Phases;
        public int TotalReward;
    }

    #region Class Members
    private List<RegionalData> regions;
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
        regions = new List<RegionalData>();
        var getPhasesProc = GetProcedure(typeof(SQLProcGetPhasesData)) as SQLProcGetPhasesData;

        foreach (Region region in Enum.GetValues(typeof(Region))) {
            if (region == Region.None) continue;
            string regionStr = region.ToString().Replace('_', ' ');

            foreach (DifficultyLevel diff in Enum.GetValues(typeof(DifficultyLevel))) {
                string diffStr = diff.ToString().ToLower();
                RegionalData regionalData;
                regionalData.Region = region;
                regionalData.Difficulty = diff;
                regionalData.Phases = new List<PhaseData>();
                regionalData.TotalReward = 0;

                //fetch phases data
                var req = new GetPhasesDataRequest(regionStr, diffStr);
                var res = await getPhasesProc.Run(req);

                foreach (GetPhasesDataPhaseInfo phase in res.Phases) {
                    PhaseData phaseData;
                    phaseData.MapName = phase.MapName;
                    phaseData.MinePercent = phase.MinePercent;
                    phaseData.Reward = phase.Reward;
                    regionalData.TotalReward += phaseData.Reward;
                    regionalData.Phases.Add(phaseData);
                }
                
                regions.Add(regionalData);
            }
        }
    }
    
    /// <param name="region">The context region</param>
    /// <param name="difficulty">The context level of difficulty</param>
    /// <returns>The total reward money for the entire region level</returns>
    public int GetRegionTotalReward(Region region, DifficultyLevel difficulty) {
        try {
            RegionalData dataSet = regions.Find(x => x.Region == region && x.Difficulty == difficulty);
            return dataSet.TotalReward;
        }
        catch (Exception) { return 0; }
    }
}