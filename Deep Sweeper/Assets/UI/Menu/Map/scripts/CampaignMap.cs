using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Menu.Map
{
    public class CampaignMap : MonoBehaviour
    {
        #region Class Members
        private CampaignScreen campaignScreen;
        #endregion

        #region Events
        public event UnityAction<Region> TileClickedEvent;
        #endregion

        private void Awake() {
            this.campaignScreen = GetComponentInParent<CampaignScreen>();
        }

        /// <summary>
        /// Report when a tile of the map is clicked.
        /// This function opens the contract if available.
        /// </summary>
        /// <param name="tile">The tile component</param>
        public void ReportTileClicked(MapTile tile) {
            Region region = tile.Region;
            TileClickedEvent?.Invoke(region);
            campaignScreen.OpenContract(region);
        }
    }
}