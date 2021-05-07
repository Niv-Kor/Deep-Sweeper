using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class DynamicCamera : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Tooltip("True to always keep this camera on,\n"
               + "no matter any camera switches that take place during the game")]
        [SerializeField] private bool alwaysOn;
        #endregion

        #region Properties
        public bool AlwaysOn => alwaysOn;
        public bool IsDisplaying => Camera.enabled;
        public UnityEngine.Camera Camera { get; private set; }
        #endregion

        protected virtual void Awake() {
            this.Camera = GetComponent<UnityEngine.Camera>();
        }

        /// <summary>
        /// Enable or disable the camera.
        /// </summary>
        /// <param name="flag">True to enable or false to disable</param>
        public void Enable(bool flag) {
            if (Camera.enabled == flag) return;

            void Callback() { Camera.enabled = flag; }

            //preliminary callbacks
            if (flag) BeforeActivation();
            else BeforeDeactivation();

            //subsequent callbacks
            if (flag) {
                Callback();
                OnActivation();
            }
            else OnDeactivation(Callback);
        }

        /// <summary>
        /// Activate before the camera component is activated.
        /// </summary>
        protected virtual void BeforeActivation() {}

        /// <summary>
        /// Activate when the camera component is activated.
        /// </summary>
        protected virtual void OnActivation() {}

        /// <summary>
        /// Activate before the camera component is activated.
        /// </summary>
        protected virtual void BeforeDeactivation() {}

        /// <summary>
        /// Activate when the camera component is deactivated.
        /// </summary>
        /// <param name="callback">A callback function to activate as soon as the process is done</param>
        protected virtual void OnDeactivation(UnityAction callback = null) {}
    }
}