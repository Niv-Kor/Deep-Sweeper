using GamedevUtil.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GamedevUtil.Data
{
    public static class SQLController
    {
        /*private string conString = "Server=den1.mssql8.gear.host;"
                                 + "Database=deepsweeper;"
                                 + "User ID=deepsweeper;"
                                 + "Password=P2413567cu221!;"
                                 + "Integrated Security=False";
    */
        private static string cs;

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
        public static List<object> ExecProcedure(string procedure, Queue<SQLInput> parameters, Queue<SQLOutput> returnValues) {
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
                SqlDataReader reader = command.ExecuteReader();
                List<object> res = new List<object>();

                List<SQLOutput> returnValuesList = returnValues.ToList();
                for (int readColumn = 0; reader.Read(); readColumn++) {
                    SQLOutput output = returnValuesList.Find(x => x.Column == readColumn);
                    object value = null;

                    switch (output.Type) {
                        case SqlDbType.VarChar:
                            value = reader.GetString(readColumn);
                            break;

                        case SqlDbType.Int:
                            value = reader.GetInt32(readColumn);
                            break;

                        case SqlDbType.Decimal:
                            value = reader.GetDecimal(readColumn);
                            break;

                        case SqlDbType.TinyInt:
                            value = reader.GetBoolean(readColumn);
                            break;
                    }

                    res.Add(value);
                }

                connection.Close();
                return res;
            }
        }
    }
}