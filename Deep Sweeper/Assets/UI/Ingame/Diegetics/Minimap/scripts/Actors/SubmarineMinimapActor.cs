using DeepSweeper.CameraSet;
using UnityEngine;

public class SubmarineMinimapActor : MinimapActor
{
    #region Class Members
    private DynamicCamera FPCam;
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