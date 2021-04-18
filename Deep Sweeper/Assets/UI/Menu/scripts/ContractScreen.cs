using UnityEngine.Events;

namespace DeepSweeper.Menu.Contract
{
    public class ContractScreen : UIScreen
    {
        #region Events
        public event UnityAction<Region> ContextRegionChangeEvent;
        public event UnityAction<DifficultyLevel> ContextDifficultyChangeEvent;
        #endregion

        #region Properties
        public Region Region { get; private set; }
        public DifficultyLevel Difficulty { get; private set; }
        #endregion

        /// <summary>
        /// Set a regional context to the content of the contract.
        /// </summary>
        /// <param name="region">The current context region</param>
        public void SetContext(Region region) {
            Region = region;
            ContextRegionChangeEvent?.Invoke(region);
        }

        /// <summary>
        /// Set a difficulty context to the content of the contract.
        /// </summary>
        /// <param name="region">The current context difficulty level</param>
        public void SetContext(DifficultyLevel diff) {
            Difficulty = diff;
            ContextDifficultyChangeEvent?.Invoke(diff);
        }

        /// <inheritdoc/>
        protected override void OnScreenUp(UIScreen prevScreen) {}

        /// <inheritdoc/>
        protected override void OnScreenOff(UIScreen nextScreen) {}
    }
}