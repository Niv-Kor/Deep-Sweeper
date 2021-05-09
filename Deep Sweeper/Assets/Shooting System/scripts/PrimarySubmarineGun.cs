using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public abstract class PrimarySubmarineGun : SubmarineGun
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("A prefab of a bullet.")]
        [SerializeField] protected GameObject bulletPrefab;

        [Tooltip("A list of all particles that should be executed at the moment of shooting.")]
        [SerializeField] protected List<ParticleSystem> richochet;

        [Header("Settings")]
        [Tooltip("The force in which the fire recoil takes place.")]
        [SerializeField] protected float recoilForce;

        [Tooltip("The time it takes to load a new bullet into the barrel (in seconds).")]
        [SerializeField] protected float loadingTime = .5f;
        #endregion

        #region Constants
        private static readonly string BARREL_NAME = "Barrel";
        #endregion

        #region Class Members
        private Coroutine loadCoroutine;
        private GameObject barrel;
        private bool loadable;
        #endregion

        #region Properties
        public override GunType Type => GunType.Primary;
        public int BarrelContent {
            get {
                Bullet[] bullets = barrel.GetComponentsInChildren<Bullet>();
                IEnumerable<Bullet> active = from bullet in bullets
                                             where bullet.IsActive
                                             select bullet;

                return active.Count();
            }
        }
        #endregion

        protected virtual void Awake() {
            this.loadable = true;
        }

        protected override void Start() {
            base.Start();

            //create barrel
            this.barrel = new GameObject(BARREL_NAME);
            barrel.transform.SetParent(transform);
            barrel.transform.localPosition = Vector3.zero;
        }

        protected virtual void OnValidate() {
            loadingTime = Mathf.Max(0, loadingTime);
        }

        /// <summary>
        /// Load a new bullet into the barrel.
        /// </summary>
        protected virtual IEnumerator LoadBullet() {
            float timer = loadingTime;

            while (timer > 0) {
                timer -= Time.deltaTime;
                yield return null;
            }

            loadable = true;
        }

        /// <summary>
        /// Launch a missile from the center of the sight.
        /// </summary>
        /// <param name="fwdDir">The direction on which the bullet is fired</param>
        /// <param name="recoil">True to apply a recoil effect and richochet animations</param>
        /// <param name="targetGrid">The grid that's in the center of the sight at the time of launch</param>
        /// <param name="ignoreBarrelContent">
        /// True to ignore the fact that the barrel
        /// might already consist of a bullet.
        /// If set to false, a bullet will not be fired
        /// when the barrel is not empty.
        /// </param>
        /// <returns>True if a bullet has been fired successfully.</returns>
        protected void Fire(Vector3 fwdDir, bool recoil, MineGrid targetGrid, bool ignoreBarrelContent = false) {
            if (!loadable && !ignoreBarrelContent) return;

            //instantiate bullet
            GameObject bulletInstance = Instantiate(bulletPrefab);
            bulletInstance.transform.SetParent(barrel.transform);
            bulletInstance.transform.localPosition = Vector3.zero;
            bulletInstance.transform.rotation = Quaternion.LookRotation(fwdDir, Vector3.up);

            //temporarily set the mine's layer as a terget mine
            if (targetGrid != null) {
                Bullet bulletCmp = bulletInstance.GetComponent<Bullet>();
                targetGrid.TempTarget = true;

                //ensure the mine has been detonated and revert its layer
                bulletCmp.BulletHitEvent += delegate {
                    targetGrid.DetonationSystem.TriggerHit(bulletCmp, true);
                    targetGrid.TempTarget = false;
                };
            }

            //recoil and richochet
            if (recoil) {
                foreach (ParticleSystem particle in richochet) particle.Play();
                Recoil(recoilForce);
            }

            //load
            loadable = loadingTime == 0;

            if (!loadable) {
                if (loadCoroutine != null) StopCoroutine(loadCoroutine);
                loadCoroutine = StartCoroutine(LoadBullet());
            }
        }
    }
}