using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.CameraSet
{
    [RequireComponent(typeof(Camera))]
    public class DynamicCamera : MonoBehaviour
    {
        #region Class Members
        private AudioListener audioCmp;
        #endregion

        #region Properties
        public bool IsDisplaying => CameraComponent.enabled;
        public Camera CameraComponent { get; private set; }
        #endregion

        protected virtual void Awake() {
            this.CameraComponent = GetComponent<Camera>();
            this.audioCmp = GetComponent<AudioListener>();
        }

        /// <summary>
        /// Enable or disable the camera.
        /// </summary>
        /// <param name="flag">True to enable or false to disable</param>
        public void Enable(bool flag) {
            if (CameraComponent.enabled == flag) return;

            void Callback() {
                CameraComponent.enabled = flag;
                if (audioCmp != null) audioCmp.enabled = flag;
            }

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