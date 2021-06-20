using DeepSweeper.Level;
using GamedevUtil;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public class BulletHitsManager : Pool<BulletHit>, IFirearmModule
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("Hit particles prefab to instantiate.")]
        [SerializeField] private BulletHit bulletHitPrefab;
        #endregion

        #region Constants
        private static readonly int INITIAL_HITS = 10;
        #endregion

        #region Properties
        public Firearm Firearm { get; set; }
        #endregion

        protected override void Awake() {
            base.Awake();
        }

        private void Start() {
            Insert(INITIAL_HITS);
        }

        /// <summary>
        /// Activate hit particles.
        /// </summary>
        /// <param name="position">Position of impact</param>
        /// <param name="rotation">Particles' rotation</param>
        public void Activate(Vector3 position, Vector3 rotation) {
            BulletHit hit = Take();
            hit.transform.position = position;
            hit.transform.rotation = Quaternion.Euler(rotation);
            hit.Activate();
        }

        /// <inheritdoc/>
        public override BulletHit Make() {
            BulletHit hit = Instantiate(bulletHitPrefab);
            FXManager.Instance.Adopt(hit.transform);
            hit.FadeEvent += delegate { Return(hit); };
            return hit;
        }
    }
}