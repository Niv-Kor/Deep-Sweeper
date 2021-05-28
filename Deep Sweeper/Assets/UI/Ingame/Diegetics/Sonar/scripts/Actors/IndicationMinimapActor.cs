using DeepSweeper.CameraSet;
using DeepSweeper.Level.Mine;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Diegetics.Sonar
{
    public class IndicationMinimapActor : MinimapActor
    {
        #region Class Members
        private DynamicCamera FPCam;
        #endregion

        protected override void Awake() {
            base.Awake();

            DetonationSystem detonationSys = GetComponentInParent<DetonationSystem>();
            CameraManager.Instance.Get(CameraRole.Main, out CameraSetConfig camConfig);
            this.FPCam = camConfig.Camera;

            //apply minimap layer when mine dismisses
            detonationSys.DetonationEvent += delegate { ApplyMinimapLayer(true); };
        }

        /// <inheritdoc/>
        protected override float GetYawAngle() {
            Quaternion rot = Quaternion.LookRotation(FPCam.transform.forward, Vector3.up);
            return rot.eulerAngles.y;
        }
    }
}