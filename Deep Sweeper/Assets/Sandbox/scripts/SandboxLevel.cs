using DeepSweeper.Data;
using DeepSweeper.Flow;
using DeepSweeper.Menu.UI.Campaign.Sandbox.Ring;
using UnityEngine;

namespace DeepSweeper.Menu.UI.Campaign.Sandbox
{
    public class SandboxLevel : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Settings")]
        [Tooltip("The region at which this level ring points.")]
        [SerializeField] private Region region;
        #endregion

        #region Properties
        public LevelRing Ring { get; private set; }
        public Region Region { get => region; }
        #endregion

        private void Start() {
            var LevelsMngr = GetComponentInParent<RingsManager>();
            this.Ring = LevelsMngr.CreateRing(transform);

            //select the ring if its index within the district is the first
            int levelIndex = DatabaseHandler.Instance.Pool.GetRegionIndex(Region);
            if (levelIndex == 0) Ring.InitializedEvent += delegate { Ring.Selected = true; };
        }
    }
}