using System.Data;

namespace GamedevUtil.Data
{
    public struct SQLOutput
    {
        public string ColumnName;
        public SqlDbType Type;

        public SQLOutput(string colName, SqlDbType type) {
            this.ColumnName = colName;
            this.Type = type;
        }
    }
}