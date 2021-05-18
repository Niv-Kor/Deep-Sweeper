using DeepSweeper.CameraSet;

public class SubmarineMinimapActor : MinimapActor
{
    #region Class Members
    private DynamicCamera FPCam;
    #endregion

    protected override void Awake() {
        base.Awake();
        this.FPCam = CameraManager.Instance.GetCamera(CameraRole.Main);
    }

    /// <inheritdoc/>
    protected override float GetYawAngle() {
        return FPCam.transform.rotation.eulerAngles.y;
    }
}