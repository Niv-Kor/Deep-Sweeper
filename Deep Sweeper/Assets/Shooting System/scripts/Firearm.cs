using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    [RequireComponent(typeof(Barrel))]
    [RequireComponent(typeof(Cartridge))]
    [RequireComponent(typeof(BulletHitsManager))]
    public class Firearm : MonoBehaviour
    {
        #region Properties
        public Barrel Barrel { get; private set; }
        public Cartridge Cartridge { get; private set; }
        public BulletHitsManager BulletsHitSystem { get; private set; }
        #endregion

        private void Awake() {
            this.Barrel = GetComponent<Barrel>();
            this.Cartridge = GetComponent<Cartridge>();
            this.BulletsHitSystem = GetComponent<BulletHitsManager>();

            Barrel.Firearm = this;
            Cartridge.Firearm = this;
            BulletsHitSystem.Firearm = this;
        }
    }
}