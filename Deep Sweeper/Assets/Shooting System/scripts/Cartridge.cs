using GamedevUtil;
using System.Collections;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public class Cartridge : Pool<Bullet>, IFirearmModule
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("A prefab of a the cannon's bullet.")]
        [SerializeField] private Bullet bulletPrefab;

        [Header("Timing")]
        [Tooltip("The time it takes to load the cannon between bullets (in seconds).")]
        [SerializeField] private float loadingTime;
        #endregion

        #region Constants
        private static readonly string BARREL_PARENT_NAME = "Barrel";
        #endregion

        #region Class Members
        private Transform cartridgeParent;
        private bool slideable;
        #endregion

        #region Properties
        public Firearm Firearm { get; set; }
        public bool IsLoading => !slideable;
        #endregion

        protected override void Awake() {
            base.Awake();
            this.slideable = true;

            //create barrel
            this.cartridgeParent = new GameObject(BARREL_PARENT_NAME).transform;
            cartridgeParent.SetParent(transform);
            cartridgeParent.localPosition = Vector3.zero;
        }

        private void Start() {
            int initialAmount = (int) Mathf.Ceil(bulletPrefab.TimeToLive / loadingTime);
            Insert(initialAmount);
        }

        private void OnValidate() {
            loadingTime = Mathf.Max(0, loadingTime);
        }

        /// <summary>
        /// Load a new bullet into the barrel.
        /// </summary>
        private IEnumerator LoadCannon() {
            slideable = false;
            yield return new WaitForSeconds(loadingTime);
            slideable = true;
        }

        /// <summary>
        /// Activate when a bullet hits a target.
        /// This function instantiates a hit effect at the place of impact.
        /// </summary>
        /// <param name="position">The position of the bullet's impact</param>
        /// <param name="rotation">The roatation of the bullet at the time of impact</param>
        private void OnBulletImpact(Vector3 position, Vector3 rotation) {
            Firearm.BulletsHitSystem.Activate(position, rotation);
        }

        /// <summary>
        /// Get a bullet ready to shoot.
        /// </summary>
        /// <param name="direction">The shooting direction</param>
        /// <param name="ignoreLoadingTime">True to ignore the loading time and slide a bullet anyway</param>
        /// <returns>
        /// The first bullet in the stack
        /// (or null if the cannon has not finished loadeding yet).
        /// </returns>
        public Bullet SlideBullet(Vector3 direction, bool ignoreLoadingTime = false) {
            if (!slideable && !ignoreLoadingTime) return null;

            Bullet bullet = Take();

            //face bullet towards the specified direction
            bullet.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            //reload cannon
            if (loadingTime > 0) {
                StopAllCoroutines();
                StartCoroutine(LoadCannon());
            }

            return bullet;
        }

        /// <summary>
        /// Create a new bullet.
        /// </summary>
        /// <returns>The newly created bullet.</returns>
        public override Bullet Make() {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.transform.SetParent(cartridgeParent.transform);
            bullet.transform.localPosition = Vector3.zero;
            bullet.InactiveEvent += delegate { Return(bullet); };
            bullet.HitEvent += OnBulletImpact;
            return bullet;
        }
    }
}