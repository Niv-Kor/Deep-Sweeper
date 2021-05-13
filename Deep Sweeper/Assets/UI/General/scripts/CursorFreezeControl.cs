using DeepSweeper.CameraSet;
using DeepSweeper.Player;
using GamedevUtil.Player;
using UnityEngine;

namespace DeepSweeper.UI
{
    public class CursorFreezeControl : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Tooltip("The conrtoller that allows the player to move.")]
        [SerializeField] private PlayerController3D playerController;

        [Tooltip("The camera controller that the cursor's display status affects.")]
        [SerializeField] private CameraRig cameraController;
        #endregion

        private void Awake() {
            //auto find the mandatory movement input components
            playerController ??= Submarine.Instance.Controller;
            cameraController ??= IngameCameraManager.Instance.Rig;
            CursorViewer.Instance.StatusChangeEvent += OnCursorDisplayStatusChange;
        }

        /// <summary>
        /// Activate when the cursor's display status is changed.
        /// This function enables or disables the player's movement input components
        /// while the cursor is shown.
        /// </summary>
        /// <param name="display">True if the cursor is now being displayed after the change</param>
        private void OnCursorDisplayStatusChange(bool display) {
            if (cameraController != null) {
                if (display) cameraController.Pause();
                else cameraController.Resume();
            }
            if (playerController != null) playerController.IsMovable = !display;
        }
    }
}