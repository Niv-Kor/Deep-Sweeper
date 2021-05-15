using System.Collections;
using System.Collections.Generic;
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
            this.animationTime = CountAnimationTime();
        }

        /// <summary>
        /// Count the total amount of the hit's animation time.
        /// </summary>
        /// <returns>The hit's total animation time in seconds.</returns>
        private float CountAnimationTime() {
            var systems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
            systems.Remove(particles);
            float time = 0;

            foreach (var system in systems) {
                float duration = system.main.startLifetime.constantMax;
                if (duration > time) time = duration;
            }

            return time;
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