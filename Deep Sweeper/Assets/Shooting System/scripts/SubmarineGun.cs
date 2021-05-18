using DeepSweeper.CameraSet;
using DeepSweeper.Level.Mine;
using DeepSweeper.UI.Ingame.Sight;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public abstract class SubmarineGun : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Recoil")]
        [Tooltip("The force in which the fire recoil takes place.")]
        [SerializeField] protected float recoilForce;

        [Tooltip("The camera's shake instensity at the time of launch")]
        [SerializeField] [Range(0f, 1f)] protected float cameraShake;
        #endregion

        #region Class Members
        protected Rigidbody submarineRB;
        protected SubmarineOrientation submarine;
        protected List<Firearm> firearms;
        protected CameraShaker camShaker;
        #endregion

        #region Properties
        public abstract GunType Type { get; }
        public abstract GunMechanism Mechanism { get; }
        public OperationType OperationType { get; private set; }
        public bool IsActive { get; private set; }
        protected bool CanLoad {
            get {
                bool bulletsAvailable = true;

                foreach (Firearm firearm in firearms)
                    if (firearm.Cartridge.IsLoading) return false;

                return bulletsAvailable;
            }
        }
        #endregion

        protected virtual void Awake() {
            this.IsActive = false;
        }

        protected virtual void Start() {
            this.firearms = new List<Firearm>(GetComponentsInChildren<Firearm>());
            this.camShaker = CameraManager.Instance.GetCamera(CameraRole.Main).GetComponent<CameraShaker>();
            this.submarineRB = Submarine.Instance.GetComponent<Rigidbody>();
            this.submarine = Submarine.Instance.Orientation;
            BindTriggerEventes();
        }

        /// <summary>
        /// Bind the trigger events based on the gun's type and mechanism.
        /// </summary>
        private void BindTriggerEventes() {
            switch (Mechanism) {
                case GunMechanism.SemiAutomatic:
                    SightRay.Instance.PrimaryTriggerEvent += delegate (SightTargetType targetType, TargetInfo target, bool persistent) {
                        if (IsActive && !persistent && IsOperation(OperationType.Primary)) {
                            OnGunTriggerStart(targetType, target);
                            OnGunTriggerStop();
                        }
                    };

                    SightRay.Instance.SecondaryTriggerEvent += delegate (SightTargetType targetType, TargetInfo target, bool persistent) {
                        if (IsActive && !persistent && IsOperation(OperationType.Secondary)) {
                            OnGunTriggerStart(targetType, target);
                            OnGunTriggerStop();
                        }
                    };

                    break;

                case GunMechanism.Automatic:
                    SightRay.Instance.PrimaryTriggerEvent += delegate (SightTargetType targetType, TargetInfo target, bool persistent) {
                        if (IsActive && IsOperation(OperationType.Primary)) OnGunTriggerStart(targetType, target);
                    };

                    SightRay.Instance.PrimaryStopEvent += delegate {
                        if (IsActive && IsOperation(OperationType.Primary)) OnGunTriggerStop();
                    };

                    SightRay.Instance.SecondaryTriggerEvent += delegate (SightTargetType targetType, TargetInfo target, bool persistent) {
                        if (IsActive && IsOperation(OperationType.Secondary)) OnGunTriggerStart(targetType, target);
                    };

                    SightRay.Instance.SecondaryStopEvent += delegate {
                        if (IsActive && IsOperation(OperationType.Secondary)) OnGunTriggerStop();
                    };

                    break;
            }
        }

        /// <param name="type">A type of operation against which to check this gun's temporary operation</param>
        /// <returns>True if the gun's operation type matches the given operation</returns>
        private bool IsOperation(OperationType type) {
            return OperationType == type || OperationType == OperationType.Both;
        }

        /// <summary>
        /// Activate or deactivate this gun.
        /// </summary>
        /// <param name="flag">True to activate or false to deactivate</param>
        /// <param name="operation">
        /// The type of operation that this gun fulfills
        /// (only relevant when activating).
        /// </param>
        public virtual void Activate(bool flag, OperationType operation = OperationType.None) {
            IsActive = flag;
            if (IsActive) OperationType = operation;
        }

        /// <summary>
        /// Activate when the gun trigger starts.
        /// </summary>
        /// <param name="targetType">Type of target</param>
        /// <param name="target">Target info</param>
        protected virtual void OnGunTriggerStart(SightTargetType targetType, TargetInfo target) {
            if (target == null) {
                FireAtNull();
                return;
            }
            else {
                switch (targetType) {
                    case SightTargetType.Mine: FireAtMine(target); break;
                    case SightTargetType.Indicator: FireAtIndicator(target); break;
                    default: FireAtNull(); break;
                }
            }
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

            List<Bullet> bullets = FireBullets(dir, ricochet, ignoreBarrelContent);

            if (bullets.Count > 0) {
                CrosshairManager.Instance.ActiveCrosshair.Shoot();

                foreach (Bullet bullet in bullets) EnsureTargetHit(bullet, targetGrid);
                if (recoil) Recoil(recoilForce);
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
        /// <param name="blastEffect">True to activate a blast effect for each of the successfully fired bullets</param>
        /// <param name="ignoreLoadingTime">True to ignore the loading time and slide a bullet anyway</param>
        /// <returns>A list of all fired bullets.</returns>
        protected virtual List<Bullet> FireBullets(Vector3 dir, bool blastEffect, bool ignoreLoadingTime = false) {
            List<Bullet> releasedBullets = new List<Bullet>();

            foreach (Firearm firearm in firearms) {
                Bullet bullet = firearm.Cartridge.SlideBullet(dir, ignoreLoadingTime);

                if (bullet != null) {
                    releasedBullets.Add(bullet);
                    if (blastEffect) firearm.Barrel.Blast();
                    bullet.Release();
                }
            }

            return releasedBullets;
        }

        /// <summary>
        /// Activate when the gun trigger stops.
        /// </summary>
        protected abstract void OnGunTriggerStop();

        /// <summary>
        /// Move the submarine backwards with a recoil shock.
        /// </summary>
        /// <param name="force">Recoil force</param>
        protected virtual void Recoil(float force) {
            Vector3 backwards = submarine.Forward * -1;
            submarineRB.AddForce(backwards * force);
            camShaker.Vibrate(cameraShake);
        }

        /// <summary>
        /// Fire at a grid indicator.
        /// </summary>
        /// <param name="target">Target grid's info (never null)</param>
        protected abstract void FireAtIndicator(TargetInfo target);

        /// <summary>
        /// Fire at a mine.
        /// </summary>
        /// <param name="target">Target grid's info (never null)</param>
        protected abstract void FireAtMine(TargetInfo target);

        /// <summary>
        /// Activate when the gun shoots at a null target.
        /// </summary>
        protected abstract void FireAtNull();
    }
}