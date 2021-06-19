using com.ootii.Cameras;
using DeepSweeper.Player.Input;
using UnityEngine;

namespace DeepSweeper.CameraSet
{
    public class CameraRig : MonoBehaviour, ILockedInput
    {
        #region Class Members
        private InputLocker inputLocker;
        private CameraController controller;
        #endregion

        private void Awake() {
            this.controller = GetComponent<CameraController>();
            this.inputLocker = new InputLocker(this);
        }

        /// <inheritdoc/>
        public void OnEnableInput() {
            controller.enabled = true;
            controller.ActiveMotor.Initialize();
        }

        /// <inheritdoc/>
        public void OnDisableInput() {
            controller.enabled = false;
        }

        /// <inheritdoc/>
        public void Enable(bool flag, bool force = false) {
            inputLocker.Enable(flag, force);
        }
    }
}