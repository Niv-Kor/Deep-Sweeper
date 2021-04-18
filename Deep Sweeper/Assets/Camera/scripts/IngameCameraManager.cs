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
        [SerializeField] private Transform rig;

        [Tooltip("The first person camera's post processing manager component.")]
        [SerializeField] private PostProcessingManager fpPostProcess;

        [Header("Minimap")]
        [Tooltip("Minimap camera component.")]
        [SerializeField] private DynamicCamera minimapCam;

        [Tooltip("The minimap camera's post processing manager component.")]
        [SerializeField] private PostProcessingManager minimapPostProcess;
        #endregion

        #region Properties
        public DynamicCamera FPCam { get => fpCam; }
        public DynamicCamera MinimapCam { get => minimapCam; }
        public Transform Rig { get => rig; }
        public PostProcessingManager FPPostProcess { get => fpPostProcess; }
        public PostProcessingManager MinimapPostProcess { get => minimapPostProcess; }
        #endregion

        /// <inheritdoc/>
        protected override DynamicCamera GetDefaultCamera() => FPCam;
    }
}