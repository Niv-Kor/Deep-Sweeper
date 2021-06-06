using com.ootii.Cameras;
using UnityEngine;

namespace DeepSweeper.CameraSet
{
    public class CameraRig : MonoBehaviour
    {
        #region Class Members
        private CameraController controller;
        #endregion

        #region Properties
        public bool Locked { get; private set; }
        #endregion

        private void Awake() {
            this.controller = GetComponent<CameraController>();
        }

        /// <summary>
        /// Pause the rig's cursor movement.
        /// </summary>
        /// <param name="lockState">
        /// True to lock the rig's pause state
        /// until it's explicitly released
        /// </param>
        public void Pause(bool lockState = false) {
            if (Locked && !lockState) return;

            controller.enabled = false;
            Locked = lockState;
        }

        /// <summary>
        /// Resume the rig's cursor movement.
        /// </summary>
        /// <param name="releaseState">True to release the rig's state</param>
        public void Resume(bool releaseState = false) {
            if (Locked && !releaseState) return;

            controller.enabled = true;
            controller.ActiveMotor.Initialize();
            Locked = !releaseState;
        }
    }
}