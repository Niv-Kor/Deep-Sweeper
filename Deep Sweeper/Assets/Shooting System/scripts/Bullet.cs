using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.ShootingSystem
{
    public class Bullet : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Timing")]
        [Tooltip("The time (in seconds) it takes the bullet to be inactive from the moment it's fired.")]
        [SerializeField] private float activityTime = 1;

        [Tooltip("The overall time the bullet can fly in the air before it's automatically destroyed.")]
        [SerializeField] private float timeToLive = 5;
        #endregion

        #region Constants
        private static readonly float MIN_TTL = .1f;
        #endregion

        #region Class Members
        private float activityTimer;
        #endregion

        #region Events
        public event UnityAction BulletHitEvent;
        #endregion

        #region Properties
        public bool IsActive => activityTimer < activityTime;
        public BulletWarhead Warhead { get; private set; }
        #endregion

        private void Awake() {
            this.Warhead = GetComponentInChildren<BulletWarhead>();
            this.activityTimer = 0;
        }

        private void OnValidate() {
            activityTime = Mathf.Max(activityTime, MIN_TTL);
        }

        private void Update() {
            activityTimer += Time.deltaTime;
            if (activityTimer >= timeToLive) Deconstruct();
        }

        /// <summary>
        /// Destroy this bullet.
        /// </summary>
        public void Deconstruct() {
            BulletHitEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}