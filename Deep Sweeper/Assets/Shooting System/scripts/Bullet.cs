using GamedevUtil;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Player.ShootingSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Bullet : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Settings")]
        [Tooltip("The relative power of the bullet. 0 does no damage and 1 explodes a mine in 1 hit.")]
        [SerializeField] [Range(0f, 1f)] protected float power;

        [Header("Active Time")]
        [Tooltip("The overall time the bullet can fly in the air before it's automatically destroyed (in seconds).\n"
               + "0 if it should stay active indefinitely.")]
        [SerializeField] private float timeToLive = 5;
        #endregion

        #region Constants
        private static readonly float MIN_TTL = .1f;
        #endregion

        #region Class Members
        private ParticleSystem m_particleSys;
        private ParticleSystem warheadParticles;
        private Jukebox jukebox;
        private float? m_animationTime;
        #endregion

        #region Events
        public event UnityAction InactiveEvent;

        /// <param type=typeof(Vector3)>Position of impact</param>
        /// <param type=typeof(Vector3)>Rotation at the time of impact</param>
        public event UnityAction<Vector3, Vector3> HitEvent;
        #endregion

        #region Properties
        public float Power => power;
        public float TimeToLive => timeToLive;
        public bool IsActive { get; private set; }
        private ParticleSystem ParticleSys {
            get {
                m_particleSys ??= GetComponent<ParticleSystem>();
                return m_particleSys;
            }
        }

        private float AnimationTime {
            get {
                if (m_animationTime != null) return (float) m_animationTime;
                else {
                    m_animationTime = ParticlesUtil.CountTotalAnimationTime(ParticleSys);
                    return (float) m_animationTime;
                }
            }
        }
        #endregion

        private void Awake() {
            this.jukebox = GetComponent<Jukebox>();
            TargetDetector detector = GetComponentInChildren<TargetDetector>();
            this.warheadParticles = detector?.GetComponent<ParticleSystem>();
        }

        private void Start() {
            jukebox?.PlayRandom();
        }

        private void OnValidate() {
            timeToLive = Mathf.Max(timeToLive, MIN_TTL);
        }

        /// <summary>
        /// Countdown until the bullet is no longer considered active.
        /// </summary>
        /// <param name="time">The time of cooldown (in seconds)</param>
        private IEnumerator CountdownActivity(float time) {
            yield return new WaitForSeconds(time);
            Stop();
            InactiveEvent?.Invoke();
        }

        /// <summary>
        /// Trigger a hit event.
        /// </summary>
        /// <param name="position">Position of impact</param>
        /// <param name="rotation">Rotation at the time of impact</param>
        public void ReportHit(Vector3 position, Vector3 rotation) {
            HitEvent?.Invoke(position, rotation);
            Stop();
        }

        /// <summary>
        /// Stop and deactivate the bullet.
        /// </summary>
        /// <param name="includeAll">
        /// True to stop all child particles including bullet trails.
        /// If false, only the warhead is stopped.
        /// </param>
        public void Stop(bool includeAll = false) {
            IsActive = false;

            if (includeAll || warheadParticles is null) ParticleSys.Stop();
            else warheadParticles.Stop();
        }

        /// <summary>
        /// Release the bullet.
        /// </summary>
        public void Release() {
            ParticleSys.Play();
            IsActive = true;
            StopAllCoroutines();

            float cooldownTime = Mathf.Max(timeToLive, AnimationTime);
            StartCoroutine(CountdownActivity(cooldownTime));
        }
    }
}