using DeepSweeper.CameraSet;
using DeepSweeper.Player;
using GamedevUtil.Player;
using UnityEngine;

namespace DeepSweeper.UI
{
    public class PlayerInputFreezer : Singleton<PlayerInputFreezer>
    {
        /*#region Exposed Editor Parameters
        [Tooltip("The conrtoller that allows the player to move.")]
        [SerializeField] private PlayerController3D playerController;

        [Tooltip("The camera controller that the cursor's display status affects.")]
        [SerializeField] private CameraRig cameraController;
        #endregion

        #region Class Members
        private bool m_frozen;
        #endregion

        #region Properties
        public bool IsFrozen {
            get => m_frozen;
            set {
                Freeze(value);
                m_frozen = value;
            }
        }
        #endregion

        protected override void Awake() {
            base.Awake();

            //auto find the mandatory movement input components
            playerController ??= Submarine.Instance.Controller;
            cameraController ??= CameraManager.Instance.GetRig(CameraRole.Main);
            CursorViewer.Instance.StatusChangeEvent += Freeze;
        }

        /// <summary>
        /// Enables or disables the player's movement input.
        /// </summary>
        /// <param name="flag">True to freeze the input or false to enable it</param>
        public void Freeze(bool flag = true) {
            if (cameraController != null) {


                if (flag) cameraController.Pause();
                else cameraController.Resume();
            }
            if (playerController != null) playerController.IsMovable = !flag;
        }*/
    }
}