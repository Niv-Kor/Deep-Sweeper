using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ContinuousBullet : Bullet
    {
        #region Class Members
        private ParticleSystem particles;
        private bool isActive;
        #endregion

        #region Properties
        #endregion

        private void Awake() {
            this.particles = GetComponent<ParticleSystem>();
            this.isActive = false;
        }

        /// <summary>
        /// Activate of deactivate the particle system
        /// </summary>
        /// <param name="flag">True to activate or false to deactivate</param>
        public void Activate(bool flag) {
            if (isActive == flag) return;

            if (flag) particles.Play();
            else particles.Stop();
            isActive = flag;
        }
    }
}