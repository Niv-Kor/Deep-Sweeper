using GamedevUtil.Data;
using System.Collections.Generic;
using System.Data;

namespace Data
{
    public struct CalculatePhaseGradeRequest : ISQLio
    {
        public SQLValue<string> map;
        public SQLValue<string> difficulty;
        public SQLValue<int> passedTime;

        public CalculatePhaseGradeRequest(string p_map, string p_difficulty, int p_passedTime) {
            map = new SQLValue<string>(SqlDbType.VarChar, p_map);
            difficulty = new SQLValue<string>(SqlDbType.VarChar, p_difficulty);
            passedTime = new SQLValue<int>(SqlDbType.VarChar, p_passedTime);
        }
    }

    public struct CalculatePhaseGradeResponse : ISQLio
    {
        public float Grade;

        public CalculatePhaseGradeResponse(float val) {
            Grade = val;
        }
    }

    public class SQLProcCalculatePhaseGrade : SQLProcedure<CalculatePhaseGradeRequest, CalculatePhaseGradeResponse>
    {
        #region Properties
        protected override string ProcName { get => "CalculatePhaseGrade"; }
        protected override List<SqlDbType> ReturnTypes {
            get {
                return new List<SqlDbType>() {
                    SqlDbType.Decimal
                };
            }
        }
        #endregion

        /// <inheritdoc/>
        protected override CalculatePhaseGradeResponse MapResult(List<object> result) {
            float.TryParse(result[0].ToString(), out float val);
            return new CalculatePhaseGradeResponse(val);
        }
    }
}