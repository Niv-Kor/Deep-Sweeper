namespace DeepSweeper.Player.ShootingSystem
{
    public class FlaggerGun : SubmarineGun
    {
        #region Properties
        public override GunType Type => GunType.Flagger;
        public override GunMechanism Mechanism => GunMechanism.SemiAutomatic;
        #endregion

        /// <inheritdoc/>
        protected override void OnGunTriggerStop() {}

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