using DeepSweeper.CameraSet;
using UnityEngine;

public class SubmarineOrientation : MonoBehaviour
{
    #region Class Members
    private Transform rig;
    #endregion

    #region Properties
    public Vector3 Position => transform.position;
    public Vector3 Forward => rig.forward;
    public Vector3 Right => rig.right;
    public Vector3 Up => rig.up;
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
        this.rig = IngameCameraManager.Instance.Rig.transform;
    }
}