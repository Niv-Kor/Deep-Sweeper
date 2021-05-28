using System.Collections.Generic;
using DeepSweeper.Characters;
using DeepSweeper.Player.ShootingSystem;

namespace DeepSweeper.UI.Ingame.Diegetics.Sight
{
    public class CrosshairManager : CharacterAbilityManager<CrosshairManager, CrosshairAbilityModel>
    {
        #region Properties
        public Crosshair ActiveCrosshair { get; private set; }
        #endregion

        #region Class Members
        private List<Crosshair> crosshairs;
        private bool locked;
        #endregion

        protected override void Awake() {
            base.Awake();
            this.crosshairs = new List<Crosshair>(GetComponentsInChildren<Crosshair>());
        }

        /// <summary>
        /// Lock all crosshairs.
        /// </summary>
        public void Lock() {
            if (locked) return;

            foreach (Crosshair crosshair in crosshairs) crosshair.Lock();
            locked = true;
        }

        /// <summary>
        /// Release all crosshairs.
        /// </summary>
        public void Release() {
            if (!locked) return;

            foreach (Crosshair crosshair in crosshairs) crosshair.Release();
            locked = false;
        }

        /// <summary>
        /// Enable or disable a crosshair.
        /// </summary>
        /// <param name="gun">The crosshair's gun type</param>
        /// <param name="flag">True to enable or false to disable</param>
        /// <returns>The enabled or disabled crosshair.</returns>
        private Crosshair EnableCrosshair(GunType gun, bool flag) {
            Crosshair crosshair = crosshairs.Find(x => x.Gun == gun); 
            if (crosshair != null) crosshair.IsActive = flag;
            return crosshair;
        }

        /// <inheritdoc/>
        protected override void StripAbility(CrosshairAbilityModel ability) {
            Crosshair crosshair = EnableCrosshair(ability.Gun, false);
            if (crosshair == ActiveCrosshair) ActiveCrosshair = null;
        }

        /// <inheritdoc/>
        protected override void ApplyAbility(CrosshairAbilityModel ability) {
            Crosshair crosshair = EnableCrosshair(ability.Gun, true);
            if (crosshair != null) ActiveCrosshair = crosshair;
        }
    }
}