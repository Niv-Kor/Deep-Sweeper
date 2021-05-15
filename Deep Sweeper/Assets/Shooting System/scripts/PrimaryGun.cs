using DeepSweeper.Level.Mine;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public abstract class PrimaryGun : SubmarineGun
    {
        #region Exposed Editor Parameters
        [Header("Recoil")]
        [Tooltip("The force in which the fire recoil takes place.")]
        [SerializeField] protected float recoilForce;
        #endregion

        #region Class Members
        protected List<CannonBarrel> cannonBarrels;
        #endregion

        #region Properties
        public override GunType Type => GunType.Primary;
        protected bool CanLoad {
            get {
                bool bulletsAvailable = true;

                foreach (CannonBarrel barrel in cannonBarrels)
                    if (barrel.IsLoading) return false;

                return bulletsAvailable;
            }
        }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.cannonBarrels = new List<CannonBarrel>(GetComponentsInChildren<CannonBarrel>());
        }

        /// <summary>
        /// Pull the gun's trigger.
        /// This function fires the configured amount of bullets
        /// and then ensures their target indeed receives a hit.
        /// </summary>
        /// <param name="dir">The direction towards which the bullets are facing</param>
        /// <param name="targetGrid">The grid that's in the center of the sight at the time of pulling the trigger</param>
        protected virtual void PullTrigger(Vector3 dir, MineGrid targetGrid, bool ricochet, bool recoil, bool ignoreBarrelContent = false) {
            if (!ignoreBarrelContent && !CanLoad) return;

            List<Bullet> bullets = FireBullets(dir);

            if (bullets.Count > 0) {
                foreach (Bullet bullet in bullets) EnsureTargetHit(bullet, targetGrid);
                if (ricochet) ActivateRicochet(true);
                if (recoil) {
                    Recoil(recoilForce);
                    camShaker.Shake(cameraShake);
                }
            }
        }

        /// <summary>
        /// Ensure a fired bullet indeed hit the sight's target (if it existed).
        /// </summary>
        /// <param name="bullet">The fired bullet</param>
        /// <param name="targetGrid">The target grid in the center of the sight (can be null)</param>
        protected virtual void EnsureTargetHit(Bullet bullet, MineGrid targetGrid) {
            //temporarily set the mine's layer as a terget mine
            if (targetGrid != null) {
                targetGrid.TempTarget = true;

                void OnTargetHit(Vector3 pos, Vector3 rot) {
                    targetGrid.DetonationSystem.TriggerHit(bullet, true);
                    targetGrid.TempTarget = false;
                    bullet.HitEvent -= OnTargetHit;
                }

                //ensure the mine has been hit
                bullet.HitEvent += OnTargetHit;
            }
        }

        /// <summary>
        /// Fire a bullet from each of the available cannons towards a specific direction.
        /// </summary>
        /// <param name="dir">The direction towards which the bullets are facing</param>
        /// <returns>A list of all fired bullets.</returns>
        protected virtual List<Bullet> FireBullets(Vector3 dir) {
            List<Bullet> releasedBullets = new List<Bullet>();

            foreach (CannonBarrel barrel in cannonBarrels) {
                Bullet bullet = barrel.SlideBullet(dir);

                if (bullet != null) {
                    releasedBullets.Add(bullet);
                    bullet.Release();
                }
            }

            return releasedBullets;
        }
    }
}