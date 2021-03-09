using System.Data;

namespace GamedevUtil.Data
{
    public struct SQLOutput
    {
        public int Column;
        public SqlDbType Type;

        public SQLOutput(int col, SqlDbType type) {
            this.Column = col;
            this.Type = type;
        }
    }
}