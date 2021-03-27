using GamedevUtil.Data;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Data
{
    public class DatabaseHandler : Singleton<DatabaseHandler>, IPassiveLoadingProcess
    {
        #region Exposed Editor Components
        [Tooltip("An object consisting of procedure components as its children.")]
        [SerializeField] private GameObject procedures;
        #endregion

        #region Events
        public event UnityAction ProcessFinishedEvent;
        #endregion

        #region Properties
        public DataPool Pool { get; private set; }
        #endregion

        /// <summary>
        /// Initialize the data base connection and available game data.
        /// </summary>
        public void Init() {
            this.Pool = new DataPool();
            Connect();
            FetchAllData();
        }

        /// <summary>
        /// Connect to the database server.
        /// </summary>
        private void Connect() {
            string connectionString = ConnectionStringManager.Instance.Get("Database");
            SQLController.Connect(connectionString);
        }

        /// <summary>
        /// Fetch all of the game's data and save it in a pool.
        /// </summary>
        private async void FetchAllData() {
            await Pool.Load(procedures);
            ProcessFinishedEvent?.Invoke();
        }
    }
}