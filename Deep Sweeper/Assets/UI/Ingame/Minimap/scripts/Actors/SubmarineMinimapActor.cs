using DeepSweeper.Camera;
using UnityEngine;

public class SubmarineMinimapActor : MinimapActor
{
    #region Class Members
    private Camera FPCam;
    #endregion

    protected override void Awake() {
        base.Awake();
        this.FPCam = IngameCameraManager.Instance.FPCam;
    }

    /// <inheritdoc/>
    protected override float GetYawAngle() {
        return FPCam.transform.rotation.eulerAngles.y;
    }
}