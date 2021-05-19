using DeepSweeper.Characters;

namespace DeepSweeper.Player
{
    public class MobilityManager : CharacterAbilityManager<MobilityManager, MobilityConfig>
    {
        #region Class Members
        private MobilityController submarine;
        #endregion

        protected override void Start() {
            this.submarine = Submarine.Instance.Controller;
            base.Start();
        }

        /// <inheritdoc/>
        protected override void StripAbility(MobilityConfig ability) {}

        /// <inheritdoc/>
        protected override void ApplyAbility(MobilityConfig ability) {
            submarine.MobilitySettings = ability;
        }
    }
}