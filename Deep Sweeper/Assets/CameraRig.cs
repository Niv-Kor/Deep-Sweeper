using com.ootii.Cameras;
using UnityEngine;

namespace DeepSweeper.Camera
{
    public class CameraRig : MonoBehaviour
    {
        #region Class Members
        private CameraController controller;
        #endregion

        private void Awake() {
            this.controller = GetComponent<CameraController>();
        }

        /// <summary>
        /// Pause the rig's cursor movement.
        /// </summary>
        public void Pause() {
            controller.enabled = false;
        }

        /// <summary>
        /// Resume the rig's cursor movement.
        /// </summary>
        public void Resume() {
            controller.enabled = true;
            controller.ActiveMotor.Initialize();
        }
    }
}