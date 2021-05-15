using DeepSweeper.Level.Mine;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public abstract class PrimarySemiautomaticGun : PrimaryGun
    {
        #region Properties
        public override GunMechanism Mechanism => GunMechanism.SemiAutomatic;
        #endregion

        /// <inheritdoc/>
        protected override void OnGunTriggerStop() {}
    }
}