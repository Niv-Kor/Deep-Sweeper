using TMPro;
using UnityEngine;
using DeepSweeper.Menu.UI.Campaign.Sandbox;
using DeepSweeper.Flow;
using DeepSweeper.Data;
using GamedevUtil.Enums;

namespace DeepSweeper.Menu.UI.Campaign
{
    public class CampaignRegionInfo : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The text component that indicates the name of the selected region.")]
        [SerializeField] private TextMeshProUGUI regionNameCmp;

        [Tooltip("The text component that explains the selected region's mission brief.")]
        [SerializeField] private TextMeshProUGUI missionBriefCmp;
        #endregion

        #region Class Members
        private IEnumNameFilter<Region> regionNameFilter;
        #endregion

        #region Properties
        public string RegionNameText {
            get => regionNameCmp.text;
            set { regionNameCmp.text = value; }
        }

        public string MissionBriefText {
            get => missionBriefCmp.text;
            set { missionBriefCmp.text = value; }
        }
        #endregion

        private void Start() {
            this.regionNameFilter = new UIRegionNameFilter();
            SandboxMap.Instance.RegionSelectedEvent += OnRegionSelected;
        }

        /// <summary>
        /// Activate when the selected region in the sandbox is changed.
        /// This function changes the name of the selected region,
        /// as well as its mission brief text.
        /// </summary>
        /// <param name="region">The newly selected region</param>
        private void OnRegionSelected(Region region) {
            RegionNameText = EnumNameFilter<Region>.Filter(region, regionNameFilter);
            MissionBriefText = DatabaseHandler.Instance.Pool.GetRegionMissionBrief(region);
        }
    }
}