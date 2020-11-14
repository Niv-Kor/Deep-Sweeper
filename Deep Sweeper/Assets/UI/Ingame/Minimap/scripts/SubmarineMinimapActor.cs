using UnityEngine;

public class SubmarineMinimapActor : MinimapActor
{
    private Camera FPCam;

    protected override void Start() {
        base.Start();
        this.FPCam = CameraManager.Instance.FPCam;
    }

    protected override float GetYawAngle() {
        return FPCam.transform.rotation.eulerAngles.y;
    }
}