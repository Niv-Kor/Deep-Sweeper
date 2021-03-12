using System.Data;

namespace GamedevUtil.Data
{
    public class SQLValue<T>
    {
        public SqlDbType Type;
        public T Value;

        public SQLValue(SqlDbType type, T value) {
            Type = type;
            Value = value;
        }
    }
}