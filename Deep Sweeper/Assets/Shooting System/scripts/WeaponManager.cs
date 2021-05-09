using System.Collections.Generic;

namespace DeepSweeper.ShootingSystem
{
    public class WeaponManager<G> : CharacterAbilityManager<GunSubType> where G : SubmarineGun
    {
        #region Class Members
        private List<G> guns;
        #endregion

        protected override void Awake() {
            base.Awake();

            var gunsArr = GetComponentsInChildren<G>();
            guns = new List<G>(gunsArr);

            //disable all guns
            foreach (var gun in guns) gun.gameObject.SetActive(false);
        }

        /// <summary>
        /// Enable of disable a gun.
        /// </summary>
        /// <param name="gunType">The type of gun</param>
        /// <param name="flag">True to enable or false to disable</param>
        private void EnableGun(GunSubType gunType, bool flag) {
            G gun = guns.Find(x => x.SubType == gunType);
            gun?.gameObject.SetActive(flag);
        }

        /// <inheritdoc/>
        protected override void StripAbility(GunSubType ability) {
            EnableGun(ability, false);
        }

        /// <inheritdoc/>
        protected override void ApplyAbility(GunSubType ability) {
            EnableGun(ability, true);
        }
    }
}