using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Menu.UI.Campaign.Sandbox.Ring
{
    public class RingMarker : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Animation")]
        [Tooltip("The length of the animation's Y axis.")]
        [SerializeField] private float axisLength = 1;

        [Tooltip("The time it takes the marker to finish a full animation cycle.")]
        [SerializeField] private float movementSpeed = 1;
        #endregion

        #region Class Members
        private bool m_displayed;
        private Vector3 startPos;
        private ParticleSystem particle;
        private Coroutine animationCoroutine;
        #endregion

        #region Events
        /// <param type="bool">True if the marker is displayed or false if it's hidden</param>
        public event UnityAction<bool> MarkerDisplayedEvent;
        #endregion

        #region Properties
        public bool Displayed {
            get => m_displayed;
            set {
                if (m_displayed != value) {
                    m_displayed = value;
                    MarkerDisplayedEvent?.Invoke(value);
                    var emissionModule = particle.emission;

                    if (value) {
                        particle.Clear();
                        transform.localPosition = Vector3.zero;
                        emissionModule.burstCount = 1;
                        ParticleSystem.Burst burst = new ParticleSystem.Burst(0, 1);
                        emissionModule.SetBurst(0, burst);
                        particle.Emit(1);
                        animationCoroutine = StartCoroutine(Animate());
                    }
                    else {
                        StopCoroutine(animationCoroutine);
                        emissionModule.burstCount = 0;
                        particle.Clear();
                    }
                }
            }
        }
        #endregion

        private void Start() {
            LevelRing ring = GetComponentInParent<LevelRing>();
            this.particle = GetComponent<ParticleSystem>();
            this.startPos = transform.position;
            ring.SelectedEvent += delegate(bool flag) { Displayed = flag; };
        }

        /// <summary>
        /// Move the marker up and down.
        /// </summary>
        private IEnumerator Animate() {
            float timer = 0;

            while (true) {
                timer += Time.deltaTime;
                Vector3 pos = transform.position;
                float sineWave = Mathf.Sin(timer * movementSpeed);
                float targetHeight = startPos.y + axisLength * sineWave;
                transform.position = new Vector3(pos.x, targetHeight, pos.z);
                yield return null;
            }
        }
    }
}