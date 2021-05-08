using com.ootii.Cameras;
using UnityEngine;

namespace DeepSweeper.Camera
{
    public class IngameCameraManager : CameraManager<IngameCameraManager>
    {
        #region Exposed Editor Parameters
        [Header("First Person")]
        [Tooltip("First person camera component.")]
        [SerializeField] private DynamicCamera fpCam;

        [Tooltip("Camera rig object.")]
        [SerializeField] private CameraRig rig;

        [Tooltip("The first person camera's post processing manager component.")]
        [SerializeField] private PostProcessingManager fpPostProcess;

        [Header("Minimap")]
        [Tooltip("Minimap camera component.")]
        [SerializeField] private DynamicCamera minimapCam;

        [Tooltip("The minimap camera's post processing manager component.")]
        [SerializeField] private PostProcessingManager minimapPostProcess;
        #endregion

        #region Properties
        public DynamicCamera FPCam => fpCam;
        public DynamicCamera MinimapCam => minimapCam;
        public CameraRig Rig => rig;
        public PostProcessingManager FPPostProcess => fpPostProcess;
        public PostProcessingManager MinimapPostProcess => minimapPostProcess;
        protected override DynamicCamera DefaultCamera => FPCam;
        #endregion
    }
}