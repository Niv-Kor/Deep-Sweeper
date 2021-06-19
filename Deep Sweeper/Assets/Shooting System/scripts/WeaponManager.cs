using DeepSweeper.Characters;
using DeepSweeper.Player.Input;
using System.Collections.Generic;

namespace DeepSweeper.Player.ShootingSystem
{
    public class WeaponManager : CharacterAbilityManager<WeaponManager, WeaponAbilityModel>, ILockedInput
    {
        #region Class Members
        private List<SubmarineGun> guns;
        private InputLocker inputLocker;
        #endregion

        #region Properties
        public bool InputEnabled { get; private set; }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.InputEnabled = true;
            this.inputLocker = new InputLocker(this);

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
        private void EnableGuns(WeaponAbilityModel ability, bool flag) {
            bool sameWeapon = ability.Primary == ability.Secondary;
            EnableGun(ability.Primary, sameWeapon ? OperationType.Both : OperationType.Primary, flag);
            EnableGun(ability.Secondary, sameWeapon ? OperationType.Both : OperationType.Secondary, flag);
        }

        /// <inheritdoc/>
        protected override void StripAbility(WeaponAbilityModel ability) {
            EnableGuns(ability, false);
        }

        /// <inheritdoc/>
        protected override void ApplyAbility(WeaponAbilityModel ability) {
            EnableGuns(ability, true);
        }

        /// <inheritdoc/>
        public void OnEnableInput() { InputEnabled = true; }

        /// <inheritdoc/>
        public void OnDisableInput() { InputEnabled = false; }

        /// <inheritdoc/>
        public void Enable(bool flag, bool force = false) {
            inputLocker.Enable(flag, force);
        }
    }
}