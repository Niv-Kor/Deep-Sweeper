namespace DeepSweeper.Player.ShootingSystem
{
    public class FlaggerGun : SecondarySubmarineGun
    {
        #region Properties
        public override GunSubType SubType => GunSubType.Flagger;
        #endregion

        /// <inheritdoc/>
        protected override void FireAtIndicator(TargetInfo target) {}

        /// <inheritdoc/>
        protected override void FireAtMine(TargetInfo target) {
            target.Selector.ToggleFlag();
        }

        /// <inheritdoc/>
        protected override void FireAtNull() {}
    }
}