using DeepSweeper.Characters;

namespace DeepSweeper.Player
{
    public class MobilityManager : CharacterAbilityManager<MobilityManager, MobilityAbilityModel>
    {
        #region Class Members
        private MobilityController submarine;
        #endregion

        protected override void Start() {
            this.submarine = Submarine.Instance.Controller;
            base.Start();
        }

        /// <inheritdoc/>
        protected override void StripAbility(MobilityAbilityModel ability) {}

        /// <inheritdoc/>
        protected override void ApplyAbility(MobilityAbilityModel ability) {
            submarine.MobilitySettings = ability;
        }
    }
}