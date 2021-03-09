using System.Data;

namespace GamedevUtil.Data
{
    public struct SQLInput
    {
        public string Name;
        public SqlDbType Type;
        public object Value;

        public SQLInput(string name, SqlDbType type, object val) {
            this.Name = name;
            this.Type = type;
            this.Value = val;
        }
    }
}