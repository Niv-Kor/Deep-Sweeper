using DeepSweeper.Player.ShootingSystem;
using GamedevUtil;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Sight
{
    [RequireComponent(typeof(Animator))]
    public abstract class Crosshair : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Settings")]
        [Tooltip("A multiplier by which the crosshair shooting animation's speed multiplies.")]
        [SerializeField] private float shootingSpeed = 1;
        #endregion

        #region Constants
        private static readonly string LOCK_PARAM = "locked";
        private static readonly string SHOOT_PARAM = "shoot";
        private static readonly string IN_PARAM = "in";
        private static readonly string OUT_PARAM = "out";
        private static readonly string SHOOTING_SPEED_PARAM = "shooting_speed";
        #endregion

        #region Class Members
        private Puppeteer puppeteer;
        private bool m_isActive;
        #endregion

        #region Properties
        public abstract GunType Gun { get; }
        public bool IsLocked { get; private set; }
        public bool IsActive {
            get => m_isActive;
            set {
                if (value == m_isActive) return;

                if (value) {
                    puppeteer.Manipulate(IN_PARAM);
                    OnCrosshairIn();
                }
                else {
                    puppeteer.Manipulate(OUT_PARAM);
                    OnCrosshairOut();
                }

                IsLocked = false;
                m_isActive = value;
            }
        }
        #endregion

        protected virtual void Awake() {
            Animator animator = GetComponent<Animator>();
            this.puppeteer = new Puppeteer(animator);
        }

        protected virtual void Start() {
            puppeteer.Manipulate(SHOOTING_SPEED_PARAM, shootingSpeed);
        }

        protected virtual void OnValidate() {
            try { puppeteer.Manipulate(SHOOTING_SPEED_PARAM, shootingSpeed); }
            catch {}
        }

        /// <summary>
        /// Lock the sight on a target.
        /// </summary>
        public void Lock() {
            puppeteer.Manipulate(LOCK_PARAM, true);
            IsLocked = true;
            OnCrosshairLock();
        }

        /// <summary>
        /// Release the sight's lock on a target.
        /// </summary>
        public void Release() {
            if (!IsLocked) return;

            puppeteer.Manipulate(LOCK_PARAM, false);
            IsLocked = false;
            OnCrossharirRelease();
        }

        /// <summary>
        /// Shoot the crosshair's weapon.
        /// </summary>
        public void Shoot() {
            puppeteer.Manipulate(SHOOT_PARAM);
            OnCrossharirShoot();
        }

        /// <summary>
        /// Activate when the crosshair changes and animates in.
        /// </summary>
        protected abstract void OnCrosshairIn();

        /// <summary>
        /// Activate when the crosshair changes and animates out.
        /// </summary>
        protected abstract void OnCrosshairOut();

        /// <summary>
        /// Activate when the crosshair locks on a target.
        /// </summary>
        protected abstract void OnCrosshairLock();

        /// <summary>
        /// Activate when the crosshair releases its target lock.
        /// </summary>
        protected abstract void OnCrossharirRelease();

        /// <summary>
        /// Activate when the crosshair's gun shoots a bullet.
        /// </summary>
        protected abstract void OnCrossharirShoot();
    }
}