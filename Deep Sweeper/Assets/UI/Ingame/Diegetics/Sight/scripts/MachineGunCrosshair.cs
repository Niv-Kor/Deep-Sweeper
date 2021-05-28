using DeepSweeper.Player.ShootingSystem;

namespace DeepSweeper.UI.Ingame.Diegetics.Sight
{
    public class MachineGunCrosshair : Crosshair
    {
        #region Properties
        public override GunType Gun => GunType.MachineGun;
        #endregion

        /// <inheritdoc/>
        protected override void OnCrosshairIn() {}

        /// <inheritdoc/>
        protected override void OnCrosshairOut() {}

        /// <inheritdoc/>
        protected override void OnCrosshairLock() {}

        /// <inheritdoc/>
        protected override void OnCrossharirRelease() {}

        /// <inheritdoc/>
        protected override void OnCrossharirShoot() {}
    }
}