using System.Collections;
using UnityEngine;

namespace DeepSweeper.Menu.UI.Campaign.Sandbox.Ring
{
    public class RingCircle : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Animation")]
        [Tooltip("The angle at which the circle rotates while not selected.")]
        [SerializeField] private float rotAngle = 1;

        [Tooltip("The angle at which the circle rotates while selected.")]
        [SerializeField] private float selectedRotAngle = 1;

        [Tooltip("The speed the circle's expansion animation (only when selected).")]
        [SerializeField] private float expansionSpeed = 1;

        [Tooltip("The scale of the circle's expansion (only when selected).")]
        [SerializeField] private float expansionScale = 1;

        [Tooltip("The time it takes the ring to collapse back to its original scale [s].")]
        [SerializeField] private float collapseTime = 1;
        #endregion

        #region Class Members
        private float startScale;
        private Coroutine rotationCoroutine;
        private Coroutine expansionCoroutine;
        private Coroutine collapseCoroutine;
        #endregion

        private void Start() {
            LevelRing ring = GetComponentInParent<LevelRing>();
            this.startScale = transform.localScale.x;
            this.rotationCoroutine = StartCoroutine(Rotate(rotAngle));

            ring.SelectedEvent += delegate(bool flag) {
                //rotate
                float angle = flag ? selectedRotAngle : rotAngle;
                StopCoroutine(rotationCoroutine);
                rotationCoroutine = StartCoroutine(Rotate(angle));

                //expand
                if (flag) {
                    if (collapseCoroutine != null) StopCoroutine(collapseCoroutine);
                    expansionCoroutine = StartCoroutine(Expand());
                }
                else {
                    if (expansionCoroutine != null) StopCoroutine(expansionCoroutine);
                    collapseCoroutine = StartCoroutine(Collapse());
                }
            };
        }

        /// <summary>
        /// Rotate the ring circle.
        /// </summary>
        /// <param name="speed">The speed of rotation</param>
        private IEnumerator Rotate(float angle) {
            float timer = 0;

            while (true) {
                timer += Time.deltaTime;
                transform.Rotate(0, 0, angle);
                yield return null;
            }
        }

        /// <summary>
        /// Expand and collapse the ring circle.
        /// </summary>
        private IEnumerator Expand() {
            float timer = 0;

            while (true) {
                float sineWave = Mathf.Sin(timer * expansionSpeed - 1) + 1;
                timer += Time.deltaTime;
                float targetScale = startScale + expansionScale * sineWave;
                transform.localScale = Vector3.one * targetScale;
                yield return null;
            }
        }

        /// <summary>
        /// Collapse the ring to its original size.
        /// </summary>
        private IEnumerator Collapse() {
            Vector3 srcScale = transform.localScale;
            Vector3 targetScale = Vector3.one * startScale;
            float timer = 0;

            while (timer <= collapseTime) {
                timer += Time.deltaTime;
                float step = timer / collapseTime;
                transform.localScale = Vector3.Lerp(srcScale, targetScale, step);
                yield return null;
            }
        }
    }
}