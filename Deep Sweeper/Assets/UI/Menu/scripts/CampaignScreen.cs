using DeepSweeper.Camera;
using DeepSweeper.Flow;
using DeepSweeper.Menu.Contract;
using DeepSweeper.Menu.Map;
using System.Collections;
using UnityEngine;

namespace DeepSweeper.Menu
{
    public class CampaignScreen : UIScreen
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [SerializeField] private CanvasGroup regionInfoCanvas;

        [Header("Transition")]
        [Tooltip("The time it takes the background to completely blur after this screen is up.")]
        [SerializeField] private float blurBackgroundTime;

        [Tooltip("The time to wait before the region info appears when the screen is on (in seconds).")]
        [SerializeField] private float regionInfoDelay = 0;

        [Tooltip("The time it takes the region info to fade into the screen (in seconds).")]
        [SerializeField] private float regionInfoFadeInTime = 1;

        [Tooltip("The time it takes the region info to fade out of the screen (in seconds).")]
        [SerializeField] private float regionInfoFadeOutTime = 1;
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
        public void OnPreviousButton() {
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
        /// Slowly fade the region info texts.
        /// </summary>
        /// <param name="fadeIn">True to fade in or false to fade out</param>
        /// <param name="time">The time it takes the process to be done</param>
        private IEnumerator FadeRegioInfo(bool fadeIn, float time = 0) {
            yield return new WaitForSeconds(regionInfoDelay);
            float timer = 0;
            float from = regionInfoCanvas.alpha;
            float to = fadeIn ? 1 : 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                regionInfoCanvas.alpha = Mathf.Lerp(from, to, timer / time);
                yield return null;
            }
        }

        /// <summary>
        /// Show or hide the sandbox.
        /// </summary>
        /// <param name="flag">True to show or false to hide</param>
        private void DisplaySandbox(bool flag) {
            MenuCameraManager camMngr = MenuCameraManager.Instance;
            var postProcess = camMngr.BackgroundPostProcess;
            var mainCam = camMngr.BackgroundCam;
            var sandboxCam = camMngr.SandboxCam;
            var nextCam = flag ? sandboxCam : mainCam;
            postProcess.Activate(flag, blurBackgroundTime);
            camMngr.Switch(nextCam);
        }

        /// <inheritdoc/>
        protected override void OnScreenUp(UIScreen prevScreen) {
            if (prevScreen.Layout == ScreenLayout.MainMenu) {
                DisplaySandbox(true);
                StartCoroutine(FadeRegioInfo(true, regionInfoFadeInTime));
            }
        }

        /// <inheritdoc/>
        protected override void OnScreenOff(UIScreen nextScreen) {
            if (nextScreen.Layout == ScreenLayout.MainMenu) {
                DisplaySandbox(false);
                StartCoroutine(FadeRegioInfo(false, regionInfoFadeOutTime));
            }
        }
    }
}