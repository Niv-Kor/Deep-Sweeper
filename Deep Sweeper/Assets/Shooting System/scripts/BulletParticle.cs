using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public class BulletParticle : MonoBehaviour
    {
        #region Class Members
        private BulletHit hitParent;
        #endregion

        private void Start() {
            this.hitParent = GetComponentInParent<BulletHit>();
        }

        private void OnParticleSystemStopped() {
            hitParent.NotifyParticlesDeath();
        }
    }
}