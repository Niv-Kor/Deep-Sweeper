using DeepSweeper.Characters;
using System.Collections.Generic;

namespace DeepSweeper.Player.ShootingSystem
{
    public class WeaponManager : CharacterAbilityManager<WeaponManager, WeaponAbility>
    {
        #region Class Members
        private List<SubmarineGun> guns;
        #endregion

        protected override void Awake() {
            base.Awake();

            var gunsArr = GetComponentsInChildren<SubmarineGun>();
            guns = new List<SubmarineGun>(gunsArr);

            //disable all guns
            foreach (var gun in guns) gun.Activate(false);
        }

        /// <summary>
        /// Enable of disable a gun.
        /// </summary>
        /// <param name="gunType">The type of gun</param>
        /// <param name="flag">True to enable or false to disable</param>
        private void EnableGun(GunType gunType, OperationType operation, bool flag) {
            SubmarineGun gun = guns.Find(x => x.Type == gunType);
            if (gun != null) gun.Activate(flag, operation);
        }

        /// <summary>
        /// Enable or disable both primary and secondary guns
        /// in a wrapper character ability.
        /// </summary>
        /// <param name="ability">The ability to enable or disable the guns of which</param>
        /// <param name="flag">True to enable or false to disable</param>
        private void EnableGuns(WeaponAbility ability, bool flag) {
            bool sameWeapon = ability.Primary == ability.Secondary;
            EnableGun(ability.Primary, sameWeapon ? OperationType.Both : OperationType.Primary, flag);
            EnableGun(ability.Secondary, sameWeapon ? OperationType.Both : OperationType.Secondary, flag);
        }

        /// <inheritdoc/>
        protected override void StripAbility(WeaponAbility ability) {
            EnableGuns(ability, false);
        }

        /// <inheritdoc/>
        protected override void ApplyAbility(WeaponAbility ability) {
            EnableGuns(ability, true);
        }
    }
}