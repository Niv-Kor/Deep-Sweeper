using UnityEngine;

public class SubmarineMinimapActor : MinimapActor
{
    private Camera FPCam;

    protected override void Awake() {
        base.Awake();
        this.FPCam = CameraManager.Instance.FPCam;
    }

    protected override float GetYawAngle() {
        return FPCam.transform.rotation.eulerAngles.y;
    }
}