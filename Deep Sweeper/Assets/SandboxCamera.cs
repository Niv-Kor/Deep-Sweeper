using System.Collections;
using UnityEngine;

namespace DeepSweeper.Camera
{
    public class SandboxCamera : DynamicCamera
    {
        #region Exposed Editor Parameters
        [Header("Animation")]
        [Tooltip("The time it takes the camera's view to fully grow.")]
        [SerializeField] private float camGrowTime;

        [Tooltip("The time it takes the camera's view to fully shrink.")]
        [SerializeField] private float camShrinkTime;
        #endregion

        #region Constant
        private static readonly int SMALLEST_VIEW = 179;
        #endregion

        #region Class Members
        private float originFieldOfView;
        #endregion

        protected override void Awake() {
            base.Awake();
            this.originFieldOfView = Camera.fieldOfView;
        }

        /// <summary>
        /// Slowly grow or shrink the camera's view to the full.
        /// </summary>
        /// <param name="grow">True to grow the camera's view or false to shrink it</param>
        /// <param name="time">The time it takes to complete the process</param>
        private IEnumerator GrowView(bool grow, float time) {
            float timer = 0;
            float from = Camera.fieldOfView;
            float to = grow ? originFieldOfView : SMALLEST_VIEW;

            while (timer <= time) {
                timer += Time.deltaTime;
                Camera.fieldOfView = Mathf.Lerp(from, to, timer / time);
                yield return null;
            }
        }

        /// <inheritdoc/>
        protected override void BeforeActivation() {
            Camera.fieldOfView = SMALLEST_VIEW;
        }

        /// <inheritdoc/>
        protected override void OnActivation() {
            StopAllCoroutines();
            StartCoroutine(GrowView(true, camGrowTime));
        }

        /// <inheritdoc/>
        protected override void OnDeactivation() {
            StopAllCoroutines();
            StartCoroutine(GrowView(false, camShrinkTime));
        }
    }
}