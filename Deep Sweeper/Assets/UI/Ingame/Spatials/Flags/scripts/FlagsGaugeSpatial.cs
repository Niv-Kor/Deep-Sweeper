using DeepSweeper.Flow;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.UI.Ingame.Spatials.Flags
{
    public class FlagsGaugeSpatial : Spatial
    {
        #region Class Members
        private GaugeBar gauge;
        private GaugeIndicator indicator;
        private FlagsManager flagsMngr;
        #endregion

        protected override void Awake() {
            base.Awake();

            this.gauge = GetComponentInChildren<GaugeBar>();
            this.indicator = GetComponentInChildren<GaugeIndicator>();
            this.flagsMngr = FlagsManager.Instance;

            //bind events
            flagsMngr.FlagTakenEvent += OnTakeFlag;
            flagsMngr.FlagReturnedEvent += OnReturnFlag;
            flagsMngr.FlagsAmountUpdateEvent += delegate { ResetValue(null); };
        }

        /// <summary>
        /// Activate this function when the flags state changes.
        /// </summary>
        /// <param name="success">True if the change is successful</param>
        /// <param name="flagDelta">The change in the flags amount (+X to add or -X to subtract)</param>
        private void OnFlagChange(bool success, int flagDelta) {
            if (!success) return;

            //calculate the change percentage
            int max = flagsMngr.MaxFlags;
            int available = flagsMngr.AvailableFlags;
            float prevPercent = (float)(available - flagDelta) / max;
            float percent = (float)available / max;

            //drain or fill the gauge
            float deltaPercent = percent - prevPercent;
            float absPercent = Mathf.Abs(deltaPercent);
            indicator.Add(flagDelta);

            if (deltaPercent < 0) gauge.Fill(absPercent);
            else gauge.Drain(absPercent);
        }

        /// <summary>
        /// Activate this function when a flag is being used.
        /// </summary>
        /// <see cref="FlagsManager.FlagTakenEvent"/>
        private void OnTakeFlag(bool success) { OnFlagChange(success, -1); }

        /// <summary>
        /// Activate this function when a flag is being retrieved.
        /// </summary>
        /// <see cref="FlagsManager.FlagReturnedEvent"/>
        private void OnReturnFlag(bool success) { OnFlagChange(success, 1); }

        /// <inheritdoc/>
        protected override bool Activate(bool flag, float time = -1, UnityAction callback = null) {
            bool success = base.Activate(flag, time, callback);
            if (!success) return false;

            float barActivationTime = (time > 0) ? time : defaultFadeTime;
            gauge.Show(flag, barActivationTime);
            return true;
        }

        /// <inheritdoc/>
        public override void ResetValue(Phase phase) {
            indicator.Set(flagsMngr.MaxFlags);
            gauge.Drain(1, false);
        }

        /// <inheritdoc/>
        public override void OnPhaseStarts(Phase phase) { Activate(true); }

        /// <inheritdoc/>
        public override void OnPhasePauses(Phase phase) { Activate(false); }

        /// <inheritdoc/>
        public override void OnPhaseResumes(Phase phase) { Activate(true); }

        /// <inheritdoc/>
        public override void OnPhaseEnds(Phase phase, bool success) { Activate(false); }
    }
}