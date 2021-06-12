using GamedevUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Player.ShootingSystem
{
    public class Barrel : MonoBehaviour, IFirearmModule
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("Child particles to activate when the barrel fires.")]
        [SerializeField] protected List<ParticleSystem> childBlastParticles;

        [Header("Settings")]
        [Tooltip("True to use firearm animations (this GameObject must contain an animator for this).")]
        [SerializeField] protected bool useFirearmAnimator;

        [Tooltip("The exact location of the barrel hole, from which the bullets are fired.")]
        [SerializeField] protected Vector3 barrelHole;

        [Tooltip("The time it takes to invoke the 'FullyOpen' event after the barrel open, "
               + "so that the gun can be activated [s].")]
        [SerializeField] protected float openDelay;
        #endregion

        #region Constants
        protected static readonly string IN_PARAM = "in";
        protected static readonly string OUT_PARAM = "out";
        protected static readonly string SHOOT_PARAM = "shoot";
        protected static readonly Color GIZMOS_COLOR = Color.yellow;
        protected static readonly float GIZMOS_RADIUS = .5f;
        #endregion

        #region Class Members
        protected Coroutine fullyOpenCoroutine;
        protected Puppeteer puppeteer;
        protected bool useAnimator;
        #endregion

        #region Events
        public event UnityAction FullyOpenEvent;
        #endregion

        #region Properties
        public Firearm Firearm { get; set; }
        #endregion

        protected virtual void Awake() {
            Animator animator = GetComponent<Animator>();
            this.puppeteer = new Puppeteer(animator);
            this.useAnimator = animator != null && useFirearmAnimator;
        }

        protected virtual void OnDrawGizmos() {
            Gizmos.color = GIZMOS_COLOR;
            Gizmos.DrawSphere(transform.position + barrelHole, GIZMOS_RADIUS);
        }

        /// <summary>
        /// Animate the barrel's opening.
        /// </summary>
        public virtual void Open() {
            if (useAnimator) puppeteer.Manipulate(IN_PARAM);
            if (fullyOpenCoroutine != null) StopCoroutine(fullyOpenCoroutine);
            fullyOpenCoroutine = StartCoroutine(InvokeFullyOpenState());
        }

        /// <summary>
        /// Animate the barrel's closing.
        /// </summary>
        public virtual void Close() {
            if (!useAnimator) return;

            puppeteer.Manipulate(OUT_PARAM);
            if (fullyOpenCoroutine != null) StopCoroutine(fullyOpenCoroutine);
        }

        /// <summary>
        /// Locate a bullet at the barrel's exit hole.
        /// </summary>
        /// <param name="bullet">The bullet to locate</param>
        public virtual void LocateBullet(Bullet bullet) {
            bullet.transform.localPosition = barrelHole;
        }

        /// <summary>
        /// Activate the barrel's blast effect.
        /// </summary>
        public virtual void Blast() {
            if (useAnimator) puppeteer.Manipulate(SHOOT_PARAM);

            Vector3 forward = Submarine.Instance.Orientation.Forward;

            foreach (var blastParticle in childBlastParticles) {
                blastParticle.transform.forward = forward;
                blastParticle.Play();
            }
        }

        /// <summary>
        /// Wait for a fixed delay time [s] and then invoke the 'FullyOpen' event.
        /// </summary>
        protected virtual IEnumerator InvokeFullyOpenState() {
            yield return new WaitForSeconds(openDelay);
            FullyOpenEvent?.Invoke();
        }
    }
}