using System.Collections.Generic;
using UnityEngine;

namespace GamedevUtil
{
    public static class ParticlesUtil
    {
        /// <summary>
        /// Count the total amount of a parent particle's animation time.
        /// </summary>
        /// <param name="parentParticles">The parent particles system</param>
        /// <returns>The parent particles total animation time in seconds.</returns>
        public static float CountTotalAnimationTime(ParticleSystem parentParticles) {
            var childSystemsArr = parentParticles.GetComponentsInChildren<ParticleSystem>();
            var childSystemsList = new List<ParticleSystem>(childSystemsArr);
            childSystemsList.Remove(parentParticles);
            float time = 0;

            foreach (var system in childSystemsList) {
                float duration = system.main.startLifetime.constantMax;
                if (duration > time) time = duration;
            }

            return time;
        }
    }
}