using UnityEngine;

namespace DeepSweeper.Camera
{
    public class MenuCameraManager : CameraManager<MenuCameraManager>
    {
        #region Exposed Editor Parameters
        [Header("Background")]
        [Tooltip("Background camera component.")]
        [SerializeField] private UnityEngine.Camera backgroundCam;

        [Tooltip("The background camera's post processing manager component.")]
        [SerializeField] private PostProcessingManager backgroundPostProcess;

        [Header("UI")]
        [Tooltip("UI camera component.")]
        [SerializeField] private UnityEngine.Camera uiCam;

        [Tooltip("The UI camera's post processing manager component.")]
        [SerializeField] private PostProcessingManager uiPostProcess;

        [Header("Sandbox")]
        [Tooltip("Sandbox camera component.")]
        [SerializeField] private UnityEngine.Camera sandboxCam;

        [Tooltip("The sandbox camera's post processing manager component.")]
        [SerializeField] private PostProcessingManager sandboxPostProcess;
        #endregion

        #region Properties
        public UnityEngine.Camera BackgroundCam { get => backgroundCam; }
        public UnityEngine.Camera UICam { get => uiCam; }
        public UnityEngine.Camera SandboxCam { get => uiCam; }
        public PostProcessingManager BackgroundPostProcess { get => backgroundPostProcess; }
        public PostProcessingManager UIPostProcess { get => uiPostProcess; }
        public PostProcessingManager SandboxPostProcess { get => sandboxPostProcess; }
        #endregion

        /// <inheritdoc/>
        protected override UnityEngine.Camera GetDefaultCamera() => BackgroundCam;
    }
}