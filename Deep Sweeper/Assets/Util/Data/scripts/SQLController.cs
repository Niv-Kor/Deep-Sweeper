using GamedevUtil.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GamedevUtil.Data
{
    public static class SQLController
    {
        private static string cs;

        /// <summary>
        /// Setup a connection for the database.
        /// </summary>
        /// <param name="connectionString">The database's connection string</param>
        public static void Connect(string connectionString) {
            cs = connectionString;
        }

        /// <summary>
        /// Execute a procedure.
        /// </summary>
        /// <param name="procedure">Procedure name</param>
        /// <param name="parameters">Input parameters</param>
        /// <param name="returnValues">Expected returned values</param>
        /// <returns>A list of the expected return values as generic objects</returns>
        public static async Task<List<List<object>>> ExecProcedure(string procedure, Queue<SQLInput> parameters, Queue<SQLOutput> returnValues) {
            using (SqlConnection connection = new SqlConnection(cs)) {
                connection.Open();
                SqlCommand command = new SqlCommand {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = procedure
                };

                //insert parameters
                foreach (SQLInput input in parameters) {
                    string paramName = "@" + input.Name;
                    SqlParameter p = command.Parameters.Add(paramName, input.Type);
                    p.Value = input.Value;
                }

                //execute and read
                SqlDataReader reader = await command.ExecuteReaderAsync();
                List<List<object>> resultSet = new List<List<object>>();

                while (reader.Read()) {
                    List<object> row = new List<object>();

                    foreach (SQLOutput output in returnValues.ToList()) {
                        object value = reader[output.ColumnName];
                        row.Add(value);
                    }

                    resultSet.Add(row);
                }

                connection.Close();
                return resultSet;
            }
        }
    }
}