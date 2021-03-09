using System.Collections.Generic;
using UnityEngine;

namespace GamedevUtil.Data
{
    public class ConnectionStringManager : Singleton<ConnectionStringManager>
    {
        #region Exposed Editor Parameters
        [Tooltip("A list of saved connection strings.")]
        [SerializeField] private List<ConnectionString> connectionStrings;
        #endregion

        /// <param name="name">The name of the connection string</param>
        /// <returns>The connection string object.</returns>
        private ConnectionString FindConnectionString(string name) {
            return connectionStrings.Find(x => x.Name == name);
        }

        /// <param name="csName">The name of the connection string</param>
        /// <returns>The formatted connection string.</returns>
        public string Get(string csName) {
            ConnectionString cs = FindConnectionString(csName);
            return cs?.AsString;
        }

        /// <param name="csName">The name of the connection string</param>
        /// <returns>A list of the connection string's parameters.</returns>
        public List<ConnectionString.Entry> GetParameters(string csName) {
            ConnectionString cs = FindConnectionString(csName);
            return cs?.AsList;
        }
    }
}