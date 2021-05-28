using DeepSweeper.Flow;
using TMPro;
using UnityEngine;

namespace DeepSweeper.UI.Ingame
{
    public class PhaseNameSpatial : Spatial
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The text component that consists of the phase's number.")]
        [SerializeField] private TextMeshProUGUI phaseNumberCmp;

        [Tooltip("The text component that consists of the map name.")]
        [SerializeField] private TextMeshProUGUI mapNameCmp;
        #endregion

        /// <inheritdoc/>
        public override void ResetValue(Phase phase) {
            phaseNumberCmp.text = NumericUtils.Romanify(phase.Index + 1);
            mapNameCmp.text = phase.Config.MapName;
        }

        /// <inheritdoc/>
        public override void OnPhaseStarts(Phase phase) { Activate(true); }

        /// <inheritdoc/>
        public override void OnPhasePauses(Phase phase) {}

        /// <inheritdoc/>
        public override void OnPhaseResumes(Phase phase) {}

        /// <inheritdoc/>
        public override void OnPhaseEnds(Phase phase, bool success) { Activate(false); }
    }
}