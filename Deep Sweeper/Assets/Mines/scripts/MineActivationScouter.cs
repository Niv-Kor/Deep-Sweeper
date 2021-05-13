using DeepSweeper.Player;

namespace DeepSweeper.Level.Mine
{
    public class MineActivationScouter : ActivationScouter
    {
        #region Class Members
        private MineGrid grid;
        private SubmarineOrientation submarine;
        #endregion

        protected override void Start() {
            base.Start();
            this.grid = GetComponentInParent<MineGrid>();
            this.submarine = FindObjectOfType<SubmarineOrientation>();
        }

        /// <inheritdoc/>
        protected override bool InDeciderRange() {
            Phase phase = submarine.CurrentPhase;
            bool inRangeGrid = phase != null && phase.Field.ContainsGrid(grid);
            return inRangeGrid && base.InDeciderRange();
        }
    }
}