using DeepSweeper.Camera;
using DeepSweeper.Menu.Contract;
using DeepSweeper.Menu.Map;
using UnityEngine;

namespace DeepSweeper.Menu
{
    public class CampaignScreen : UIScreen
    {
        #region Exposed Editor Parameters
        [Header("Transition")]
        [Tooltip("The time it takes the background to completely blur after this screen is up.")]
        [SerializeField] private float blurBackgroundTime;
        #endregion

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

        /// <summary>
        /// Activate when a level on the sandbox is clicked.
        /// </summary>
        /// <param name="region">The region of the clicked level</param>
        public void OpenContract(Region region) {
            if (!Contract.IsPresent) {
                Contract.SetContext(region);
                UI.SwitchScreens(ScreenLayout.Contract);
            }
        }

        /// <summary>
        /// Show or hide the sandbox.
        /// </summary>
        /// <param name="flag">True to show or false to hide</param>
        private void DisplaySandbox(bool flag) {
            MenuCameraManager camMngr = MenuCameraManager.Instance;
            var postProcess = camMngr.BackgroundPostProcess;
            var sandboxCam = camMngr.SandboxCam;
            postProcess.Activate(flag, blurBackgroundTime);
            camMngr.Switch(sandboxCam);
        }

        /// <inheritdoc/>
        protected override void OnScreenUp(UIScreen prevScreen) {
            if (prevScreen.Layout == ScreenLayout.MainMenu) DisplaySandbox(true);
        }

        /// <inheritdoc/>
        protected override void OnScreenOff(UIScreen nextScreen) {
            if (nextScreen.Layout == ScreenLayout.MainMenu) DisplaySandbox(false);
        }
    }
}