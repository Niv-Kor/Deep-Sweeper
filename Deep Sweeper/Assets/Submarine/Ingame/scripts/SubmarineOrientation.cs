using DeepSweeper.CameraSet;
using DeepSweeper.Flow;
using UnityEngine;

namespace DeepSweeper.Player
{
    public class SubmarineOrientation : MonoBehaviour
    {
        #region Class Members
        private Transform rig;
        private Transform cam;
        #endregion

        #region Properties
        public Vector3 Position => cam.position;
        public Vector3 Forward => cam.forward;
        public Vector3 Right => rig.right;
        public Vector3 Up => cam.up;
        public Phase CurrentPhase {
            get {
                foreach (Phase phase in LevelFlow.Instance.Phases) {
                    bool entranceOpen = phase.EntranceGate == null || phase.EntranceGate.IsOpen;
                    bool exitOpen = phase.ExitGate != null && phase.ExitGate.IsOpen;
                    if (entranceOpen && !exitOpen) return phase;
                }

                return null; //formal return statement
            }
        }
        #endregion

        private void Awake() {
            this.cam = CameraManager.Instance.GetCamera(CameraRole.Main).transform;
            this.rig = CameraManager.Instance.GetRig(CameraRole.Main).transform;
        }

        /// <summary>
        /// Set an object's rotation towards the player.
        /// </summary>
        /// <param name="transform">The object of which to set the rotation</param>
        public void LootAtPlayer(Transform transform) {
            Vector3 towardsPlayer = Vector3.Normalize(Position - transform.position);
            transform.rotation = Quaternion.LookRotation(towardsPlayer, Vector3.up);
        }
    }
}