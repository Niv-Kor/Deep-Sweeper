using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public class Barrel : MonoBehaviour, IFirearmModule
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("Child particles to activate when the barrel fires.")]
        [SerializeField] private List<ParticleSystem> childBlastParticles;
        #endregion

        #region Properties
        public Firearm Firearm { get; set; }
        #endregion

        /// <summary>
        /// Activate the barrel's blast effect.
        /// </summary>
        public void Blast() {
            foreach (var blastParticle in childBlastParticles) blastParticle.Play();
        }
    }
}