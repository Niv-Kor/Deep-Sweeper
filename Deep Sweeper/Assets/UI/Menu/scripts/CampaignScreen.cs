using DeepSweeper.Menu.Contract;
using DeepSweeper.Menu.Map;

namespace DeepSweeper.Menu
{
    public class CampaignScreen : UIScreen
    {
        #region Properties
        public CampaignMap Map { get; private set; }
        public ContractScreen Contract { get; private set; }
        #endregion

        protected override void Start() {
            base.Start();
            this.Map = GetComponentInChildren<CampaignMap>();
            this.Contract = GetComponentInChildren<ContractScreen>();
        }

        /// <summary>
        /// Activat when the previous button is clicked.
        /// </summary>
        public void PreviousButton() {
            UI.SwitchScreens(ScreenLayout.MainMenu);
        }

        public void OpenContract(Region region) {
            if (!Contract.IsPresent) {
                Contract.SetContext(region);
                UI.SwitchScreens(ScreenLayout.Contract);
            }
        }
    }
}