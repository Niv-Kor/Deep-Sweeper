using UnityEngine;

namespace DeepSweeper.Level.Mine
{
    [RequireComponent(typeof(Jukebox))]
    public class MineExplosionEffect : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The mine's sensors manager.")]
        [SerializeField] private SensorsManager sensorsMngr;

        [Tooltip("The mine's indication system.")]
        [SerializeField] private IndicationSystem indicationSys;
        #endregion

        #region Constants
        private static readonly string DIG_SFX = "dig";
        private static readonly string EXPLOSION_SFX = "explosion";
        #endregion

        #region Class Members
        private ParticleSystem[] particles;
        private Jukebox jukebox;
        private bool isFatal;
        #endregion

        private void Awake() {
            this.particles = GetComponentsInChildren<ParticleSystem>();
            this.jukebox = GetComponent<Jukebox>();
            this.isFatal = indicationSys.IsFatal;
        }

        private void Start() {
            sensorsMngr.AllSensorsBrokenEvent += OnAllSensorsBroken;
        }

        /// <summary>
        /// Activate when all of the mine's sensors break.
        /// This function activates the mine's explosion particles.
        /// </summary>
        private void OnAllSensorsBroken() {
            foreach (ParticleSystem particle in particles) particle.Play();
            if (isFatal) jukebox.Play(EXPLOSION_SFX);
            else jukebox.Play(DIG_SFX);
        }
    }
}