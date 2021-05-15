using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public abstract class SubmarineGun : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Tooltip("A list of child ricochet effects to activate when the gun is triggered.")]
        [SerializeField] protected List<ParticleSystem> childRicochet;
        #endregion

        #region Class Members
        protected Rigidbody submarineRB;
        protected SubmarineOrientation submarine;
        #endregion

        #region Properties
        public abstract GunType Type { get; }
        public abstract GunMechanism Mechanism { get; }
        public abstract GunSubType SubType { get; }
        public bool IsActive { get; set; }
        #endregion

        protected virtual void Awake() {
            this.IsActive = false;
        }

        protected virtual void Start() {
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
                        if (!IsActive || persistent) return;
                        if (Type == GunType.Primary) OnGunTriggerStart(targetType, target);
                    };

                    SightRay.Instance.SecondaryTriggerEvent += delegate (SightTargetType targetType, TargetInfo target, bool persistent) {
                        if (!IsActive || persistent) return;
                        if (Type == GunType.Secondary) OnGunTriggerStart(targetType, target);
                    };

                    break;

                case GunMechanism.Automatic:
                    SightRay.Instance.PrimaryTriggerEvent += delegate (SightTargetType targetType, TargetInfo target, bool persistent) {
                        if (!IsActive) return;
                        if (Type == GunType.Primary) OnGunTriggerStart(targetType, target);
                    };

                    SightRay.Instance.PrimaryStopEvent += delegate {
                        if (!IsActive) return;
                        if (Type == GunType.Primary) OnGunTriggerStop();
                    };

                    SightRay.Instance.SecondaryTriggerEvent += delegate (SightTargetType targetType, TargetInfo target, bool persistent) {
                        if (!IsActive) return;
                        if (Type == GunType.Secondary) OnGunTriggerStart(targetType, target);
                    };

                    SightRay.Instance.SecondaryStopEvent += delegate {
                        if (!IsActive) return;
                        if (Type == GunType.Secondary) OnGunTriggerStop();
                    };

                    break;
            }
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
        /// Activate or deactivate the gun's ricochet effects.
        /// </summary>
        /// <param name="flag">True to activate or false to deactivate</param>
        protected virtual void ActivateRicochet(bool flag) {
            foreach (var particles in childRicochet) {
                if (flag) particles.Play();
                else particles.Stop();
            }
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