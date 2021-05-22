using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeepSweeper.CameraSet
{
    public class CameraManager : Singleton<CameraManager>
    {
        #region Exposed Editor Parameters
        [Tooltip("A list of the scene's camera sets.")]
        [SerializeField] private List<CameraSetConfig> setsConfig;

        [Tooltip("The default camera of the scene (usually the player's FP camera).")]
        [SerializeField] private CameraRole defaultCamera;
        #endregion

        #region Constants
        private static readonly string UNTAGGED_TAG = "Untagged";
        private static readonly string CAMERA_TAG = "MainCamera";
        #endregion

        #region Properties
        public CameraSetConfig CurrentActive { get; private set; }
        #endregion

        private void Start() {
            Get(defaultCamera, out CameraSetConfig defConfig);
            this.CurrentActive = defConfig;
        }

        /// <summary>
        /// Register a new dynamic camera in camera sets cache.
        /// </summary>
        /// <param name="cam">The camera to register</param>
        /// <returns>The newly created camera configuration.</returns>
        private CameraSetConfig registerCamera(DynamicCamera cam) {
            CameraSetConfig config = new CameraSetConfig();
            config.Role = CameraRole.Undefined;
            config.Camera = cam;
            config.Rig = null;
            config.PostProcessing = null;
            config.AlwaysOn = false;

            setsConfig.Add(config);
            return config;
        }

        /// <summary>
        /// Get a specific camera set by its role.
        /// If more than one set exists, only the first one is returned.
        /// </summary>
        /// <param name="role">The role of the requested camera</param>
        /// <param name="cameraSet">The returned camera set (or default if it doesn't exist)</param>
        /// <returns>True if a camera with the specified role exists.</returns>
        public bool Get(CameraRole role, out CameraSetConfig cameraSet) {
            var list = (from cam in setsConfig
                        where cam.Role == role
                        select cam).ToList();

            bool exists = list.Count > 0;
            cameraSet = exists ? list[0] : default;
            return exists;
        }

        /// <summary>
        /// Get the DynamicCamera component of a camera.
        /// </summary>
        /// <param name="role">The role of the requested camera</param>
        /// <returns>The camera's DynamicCamera component.</returns>
        public DynamicCamera GetCamera(CameraRole role) {
            bool exists = Get(role, out CameraSetConfig config);
            return exists ? config.Camera : null;
        }

        /// <summary>
        /// Get the rig of a camera.
        /// </summary>
        /// <param name="role">The role of the requested camera</param>
        /// <returns>The camera's rig.</returns>
        public CameraRig GetRig(CameraRole role) {
            bool exists = Get(role, out CameraSetConfig config);
            return exists ? config.Rig : null;
        }

        /// <summary>
        /// Get the post processing manager of a camera.
        /// </summary>
        /// <param name="role">The role of the requested camera</param>
        /// <returns>The camera's post processing manager.</returns>
        public PostProcessingManager GetPostProcessManager(CameraRole role) {
            bool exists = Get(role, out CameraSetConfig config);
            return exists ? config.PostProcessing : null;
        }

        /// <summary>
        /// Switch the main camera.
        /// </summary>
        /// <param name="role">The role of the camera into which to switch</param>
        public void Switch(CameraRole role) {
            bool listed = Get(role, out CameraSetConfig reqConfig);
            bool camMissing = reqConfig.Camera is null;
            if (!listed || camMissing) return;

            foreach (CameraSetConfig config in setsConfig) {
                bool isNewCam = config.Role == role;
                config.Camera.tag = isNewCam ? CAMERA_TAG : UNTAGGED_TAG;
                config.Camera.Enable(config.AlwaysOn || isNewCam);
            }

            CurrentActive = reqConfig;
        }

        /// <summary>
        /// Switch the main camera.
        /// </summary>
        /// <param name="cam">The camera into which to switch</param>
        public void Switch(DynamicCamera cam) {
            if (cam is null) return;

            //check if camera is already known
            var list = (from set in setsConfig
                        where set.Camera == cam
                        select set).ToList();

            bool known = list.Count != 0;
            CameraSetConfig config = known ? list[0] : registerCamera(cam);

            //switch to that camera
            config.Role = CameraRole.DynamicallySelected;
            Switch(CameraRole.DynamicallySelected);
            config.Role = CameraRole.Undefined;
        }
    }
}