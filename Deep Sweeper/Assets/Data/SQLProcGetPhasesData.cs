using GamedevUtil.Data;
using System.Collections.Generic;
using System.Data;

namespace DeepSweeper.Data
{
    public struct GetPhasesDataRequest : ISQLio
    {
        public SQLValue<string> region;
        public SQLValue<string> difficulty;

        public GetPhasesDataRequest(string p_region, string p_difficulty) {
            region = new SQLValue<string>(SqlDbType.VarChar, p_region);
            difficulty = new SQLValue<string>(SqlDbType.VarChar, p_difficulty);
        }
    }

    public struct GetPhasesDataPhaseInfo
    {
        public int Index;
        public string MapName;
        public float MinePercent;
        public int Reward;

        public GetPhasesDataPhaseInfo(int index, string map, float minePercent, int reward) {
            Index = index;
            MapName = map;
            MinePercent = minePercent;
            Reward = reward;
        }
    }

    public struct GetPhasesDataResponse : ISQLio
    {
        public GetPhasesDataPhaseInfo[] Phases;

        public GetPhasesDataResponse(List<GetPhasesDataPhaseInfo> phases) {
            Phases = phases.ToArray();
        }
    }

    public class SQLProcGetPhasesData : SQLProcedure<GetPhasesDataRequest, GetPhasesDataResponse>
    {
        #region Properties
        protected override string ProcName { get => "GetPhasesData"; }
        protected override List<SQLOutput> ReturnTypes {
            get {
                return new List<SQLOutput>() {
                    new SQLOutput("phase", SqlDbType.Int),
                    new SQLOutput("map_name", SqlDbType.VarChar),
                    new SQLOutput("mine_percent", SqlDbType.Decimal),
                    new SQLOutput("reward", SqlDbType.Int)
                };
            }
        }
        #endregion

        /// <inheritdoc/>
        protected override GetPhasesDataResponse MapResult(List<List<object>> result) {
            var phasesInfo = new List<GetPhasesDataPhaseInfo>();

            foreach (List<object> row in result) {
                int phase = (int) row[0];
                string map = (string) row[1];
                float.TryParse(row[2].ToString(), out float minePercent);
                int reward = (int) row[3];

                var phaseInfo = new GetPhasesDataPhaseInfo(phase, map, minePercent, reward);
                phasesInfo.Add(phaseInfo);
            }

            return new GetPhasesDataResponse(phasesInfo);
        }
    }
}