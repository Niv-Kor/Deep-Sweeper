using GamedevUtil.Data;
using System.Collections.Generic;
using System.Data;

namespace DeepSweeper.Data
{
    public struct CalculatePhaseGradeRequest : ISQLio
    {
        public SQLValue<string> map;
        public SQLValue<string> difficulty;
        public SQLValue<int> passedTime;

        public CalculatePhaseGradeRequest(string p_map, string p_difficulty, int p_passedTime) {
            this.map = new SQLValue<string>(SqlDbType.VarChar, p_map);
            this.difficulty = new SQLValue<string>(SqlDbType.VarChar, p_difficulty);
            this.passedTime = new SQLValue<int>(SqlDbType.VarChar, p_passedTime);
        }
    }

    public struct CalculatePhaseGradeResponse : ISQLio
    {
        public float Grade;

        public CalculatePhaseGradeResponse(float val) {
            this.Grade = val;
        }
    }

    public class SQLProcCalculatePhaseGrade : SQLProcedure<CalculatePhaseGradeRequest, CalculatePhaseGradeResponse>
    {
        #region Properties
        protected override string ProcName { get => "CalculatePhaseGrade"; }
        protected override List<SQLOutput> ReturnTypes {
            get {
                return new List<SQLOutput>() {
                    new SQLOutput("grade", SqlDbType.Decimal)
                };
            }
        }
        #endregion

        /// <inheritdoc/>
        protected override CalculatePhaseGradeResponse MapResult(List<List<object>> result) {
            float.TryParse(result[0][0].ToString(), out float val);
            return new CalculatePhaseGradeResponse(val);
        }
    }
}