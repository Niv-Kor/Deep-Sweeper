using UnityEngine;

public class IndicationMinimapActor : MinimapActor
{
    private void Awake() {
        //apply minimap layer when mine dismisses
        Sweeper sweeper = GetComponentInParent<Sweeper>();
        sweeper.MineDisposalEndEvent += delegate { ApplyMinimapLayer(true); };
    }

    protected override float GetYawAngle() {
        Camera FPCam = CameraManager.Instance.FPCam;
        Quaternion rot = Quaternion.LookRotation(FPCam.transform.forward, Vector3.up);
        return rot.eulerAngles.y;
    }
}