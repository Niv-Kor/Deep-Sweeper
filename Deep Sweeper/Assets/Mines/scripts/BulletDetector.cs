using Constants;
using DeepSweeper.Player.ShootingSystem;
using System.Collections;
using UnityEngine;

namespace DeepSweeper.Level.Mine
{
    public class BulletDetector : MonoBehaviour
    {
        #region Constants
        private static readonly float DETECTION_COOLDOWN = .05f;
        #endregion

        #region Class Members
        private MineGrid grid;
        private bool canDetect;
        #endregion

        private void Awake() {
            this.canDetect = true;
        }

        private void Start() {
            this.grid = GetComponentInParent<MineGrid>();
        }

        private void OnParticleCollision(GameObject obj) {
            if (!canDetect) return;

            if (Layers.ContainedInMask(obj.layer, Layers.BULLET)) {
                TargetDetector detector = obj.GetComponent<TargetDetector>();
                Bullet bullet = detector.Bullet;
                grid.DetonationSystem.TriggerHit(bullet, true);

                //temporarily disable detection
                canDetect = false;
                StartCoroutine(RunDetectionCooldown());
            }
        }

        /// <summary>
        /// Run the detection cooldown clock and enable detection again once it's done.
        /// </summary>
        private IEnumerator RunDetectionCooldown() {
            yield return new WaitForSeconds(DETECTION_COOLDOWN);
            canDetect = true;
        }
    }
}