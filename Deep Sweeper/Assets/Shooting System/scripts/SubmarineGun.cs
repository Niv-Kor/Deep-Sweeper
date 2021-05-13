using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public abstract class SubmarineGun : MonoBehaviour
    {
        #region Class Members
        private Rigidbody submarineRB;
        private SubmarineOrientation submarine;
        #endregion

        #region Properties
        public abstract GunType Type { get; }
        public abstract GunSubType SubType { get; }
        #endregion

        protected virtual void Start() {
            this.submarineRB = Submarine.Instance.GetComponent<Rigidbody>();
            this.submarine = Submarine.Instance.Orientation;

            //bind fire events
            SightRay.Instance.PrimaryHitEvent += delegate (SightTargetType targetType, TargetInfo target) {
                if (Type == GunType.Primary) OperateTarget(targetType, target);
            };

            SightRay.Instance.SecondaryHitEvent += delegate (SightTargetType targetType, TargetInfo target) {
                if (Type == GunType.Secondary) OperateTarget(targetType, target);
            };
        }

        /// <summary>
        /// respond to the a shoot.
        /// </summary>
        /// <param name="targetType">Type of target</param>
        /// <param name="target">Target info</param>
        protected virtual void OperateTarget(SightTargetType targetType, TargetInfo target) {
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