using DeepSweeper.Camera;
using UnityEngine;

public class IndicationMinimapActor : MinimapActor
{
    protected override void Awake() {
        base.Awake();

        //apply minimap layer when mine dismisses
        DetonationSystem detonationSys = GetComponentInParent<DetonationSystem>();
        detonationSys.DetonationEvent += delegate { ApplyMinimapLayer(true); };
    }

    protected override float GetYawAngle() {
        DynamicCamera FPCam = IngameCameraManager.Instance.FPCam;
        Quaternion rot = Quaternion.LookRotation(FPCam.transform.forward, Vector3.up);
        return rot.eulerAngles.y;
    }
}