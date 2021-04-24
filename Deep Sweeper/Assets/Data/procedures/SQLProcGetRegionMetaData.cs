using GamedevUtil.Data;
using System.Collections.Generic;
using System.Data;

namespace DeepSweeper.Data
{
    public struct GetRegionMetaDataRequest : ISQLio
    {
        public SQLValue<string> region;

        public GetRegionMetaDataRequest(string p_region) {
            this.region = new SQLValue<string>(SqlDbType.VarChar, p_region);
        }
    }

    public struct GetRegionMetaDataResponse : ISQLio
    {
        public int Index;
        public string Brief;

        public GetRegionMetaDataResponse(int index, string brief) {
            this.Index = index;
            this.Brief = brief;
        }
    }

    public class SQLProcGetRegionMetaData : SQLProcedure<GetRegionMetaDataRequest, GetRegionMetaDataResponse>
    {
        #region Properties
        protected override string ProcName { get => "GetRegionMetaData"; }
        protected override List<SQLOutput> ReturnTypes {
            get {
                return new List<SQLOutput>() {
                    new SQLOutput("index", SqlDbType.Int),
                    new SQLOutput("mission_brief", SqlDbType.VarChar),
                };
            }
        }
        #endregion

        /// <inheritdoc/>
        protected override GetRegionMetaDataResponse MapResult(List<List<object>> result) {
            int.TryParse(result[0][0].ToString(), out int index);
            string brief = result[0][1].ToString();
            return new GetRegionMetaDataResponse(index, brief);
        }
    }
}