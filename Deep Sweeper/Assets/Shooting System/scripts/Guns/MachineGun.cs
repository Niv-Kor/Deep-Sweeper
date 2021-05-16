namespace DeepSweeper.Player.ShootingSystem
{
    public class MachineGun : SubmarineGun
    {
        #region Properties
        public override GunType Type => GunType.MachineGun;
        public override GunMechanism Mechanism => GunMechanism.Automatic;
        #endregion

        /// <inheritdoc/>
        protected override void FireAtIndicator(TargetInfo target) {
            PullTrigger(transform.forward, target.Grid, true, true);
        }

        /// <inheritdoc/>
        protected override void FireAtMine(TargetInfo target) {
            PullTrigger(transform.forward, target.Grid, true, true);
        }

        /// <inheritdoc/>
        protected override void FireAtNull() {
            PullTrigger(transform.forward, null, true, true);
        }

        /// <inheritdoc/>
        protected override void OnGunTriggerStop() {}
    }
}