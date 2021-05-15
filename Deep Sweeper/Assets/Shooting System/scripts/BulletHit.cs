using GamedevUtil;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Player.ShootingSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    public class BulletHit : MonoBehaviour
    {
        #region Events
        public event UnityAction FadeEvent;
        #endregion

        #region Class Members
        private ParticleSystem particles;
        private float animationTime;
        #endregion

        private void Awake() {
            this.particles = GetComponent<ParticleSystem>();
            this.animationTime = ParticlesUtil.CountTotalAnimationTime(particles);
        }

        /// <summary>
        /// Invoke fade event after the total animation time has passed.
        /// </summary>
        private IEnumerator CountdownToFade() {
            yield return new WaitForSeconds(animationTime);
            FadeEvent?.Invoke();
        }

        /// <summary>
        /// Activate the hit particles effect.
        /// </summary>
        public void Activate() {
            particles.Play();
            StopAllCoroutines();
            StartCoroutine(CountdownToFade());
        }
    }
}