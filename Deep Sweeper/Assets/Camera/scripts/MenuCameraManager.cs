using UnityEngine;

namespace DeepSweeper.Camera
{
    public class MenuCameraManager : CameraManager<MenuCameraManager>
    {
        #region Exposed Editor Parameters
        [Header("Background")]
        [Tooltip("Background camera component.")]
        [SerializeField] private DynamicCamera backgroundCam;

        [Tooltip("The background camera's post processing manager component.")]
        [SerializeField] private PostProcessingManager backgroundPostProcess;

        [Header("UI")]
        [Tooltip("UI camera component.")]
        [SerializeField] private DynamicCamera uiCam;

        [Header("Sandbox")]
        [Tooltip("Sandbox camera component.")]
        [SerializeField] private DynamicCamera sandboxCam;
        #endregion

        #region Properties
        public DynamicCamera BackgroundCam => backgroundCam;
        public DynamicCamera UICam => uiCam;
        public DynamicCamera SandboxCam => sandboxCam;
        public PostProcessingManager BackgroundPostProcess => backgroundPostProcess;
        protected override DynamicCamera DefaultCamera => BackgroundCam;
        #endregion

        /// <inheritdoc/>
    }
}