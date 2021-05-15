namespace DeepSweeper.Player.ShootingSystem
{
    public abstract class PrimaryAutomaticGun : PrimaryGun
    {
        #region Properties
        public override GunMechanism Mechanism => GunMechanism.Automatic;
        #endregion

        /// <inheritdoc/>
        protected override void OnGunTriggerStop() {}
    }
}