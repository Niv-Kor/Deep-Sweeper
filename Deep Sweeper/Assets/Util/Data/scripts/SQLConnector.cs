using UnityEngine;

namespace GamedevUtil.Data
{
    public class SQLConnector : MonoBehaviour
    {
        private void Start() {
            string connectionString = ConnectionStringManager.Instance.Get("Database");
            SQLController.Connect(connectionString);
        }
    }
}