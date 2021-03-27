using GamedevUtil.Data;
using System.Collections.Generic;
using System.Data;

namespace DeepSweeper.Data
{
    public struct GetTotalRegionRewardRequest : ISQLio
    {
        public SQLValue<string> region;
        public SQLValue<string> difficulty;

        public GetTotalRegionRewardRequest(string p_region, string p_difficulty) {
            region = new SQLValue<string>(SqlDbType.VarChar, p_region);
            difficulty = new SQLValue<string>(SqlDbType.VarChar, p_difficulty);
        }
    }

    public struct GetTotalRegionRewardResponse : ISQLio
    {
        public int Total;

        public GetTotalRegionRewardResponse(int value) {
            Total = value;
        }
    }

    public class SQLProcGetTotalRegionReward : SQLProcedure<GetTotalRegionRewardRequest, GetTotalRegionRewardResponse>
    {
        #region Properties
        protected override string ProcName { get => "GetTotalRegionReward"; }
        protected override List<SQLOutput> ReturnTypes {
            get {
                return new List<SQLOutput>() {
                    new SQLOutput("total", SqlDbType.Int),
                };
            }
        }
        #endregion

        /// <inheritdoc/>
        protected override GetTotalRegionRewardResponse MapResult(List<List<object>> result) {
            int.TryParse(result[0][0].ToString(), out int total);
            return new GetTotalRegionRewardResponse(total);
        }
    }
}