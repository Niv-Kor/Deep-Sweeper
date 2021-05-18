using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.CameraSet
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
            this.originFieldOfView = CameraComponent.fieldOfView;
        }

        /// <summary>
        /// Slowly grow or shrink the camera's view to the full.
        /// </summary>
        /// <param name="grow">True to grow the camera's view or false to shrink it</param>
        /// <param name="time">The time it takes to complete the process</param>
        /// <param name="callback">A callback function to activate as soon as the process is done</param>
        private IEnumerator GrowView(bool grow, float time, UnityAction callback = null) {
            float timer = 0;
            float from = CameraComponent.fieldOfView;
            float to = grow ? originFieldOfView : SMALLEST_VIEW;

            while (timer <= time) {
                timer += Time.deltaTime;
                CameraComponent.fieldOfView = Mathf.Lerp(from, to, timer / time);
                yield return null;
            }

            callback?.Invoke();
        }

        /// <inheritdoc/>
        protected override void BeforeActivation() {
            CameraComponent.fieldOfView = SMALLEST_VIEW;
        }

        /// <inheritdoc/>
        protected override void OnActivation() {
            StopAllCoroutines();
            StartCoroutine(GrowView(true, camGrowTime));
        }

        /// <inheritdoc/>
        protected override void OnDeactivation(UnityAction callback = null) {
            StopAllCoroutines();
            StartCoroutine(GrowView(false, camShrinkTime, callback));
        }
    }
}