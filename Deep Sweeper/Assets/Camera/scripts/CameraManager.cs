using System.Linq;
using UnityEngine;

namespace DeepSweeper.Camera
{
    public abstract class CameraManager<T> : Singleton<T> where T : MonoBehaviour
    {
        #region Constants
        private static readonly string UNTAGGED_TAG = "Untagged";
        private static readonly string CAMERA_TAG = "MainCamera";
        #endregion

        #region Properties
        public UnityEngine.Camera CurrentActive { get; protected set; }
        #endregion

        private void Start() {
            this.CurrentActive = GetDefaultCamera();
        }

        /// <returns>The main camera in the scene.</returns>
        protected abstract UnityEngine.Camera GetDefaultCamera();

        /// <summary>
        /// Switch the main camera.
        /// </summary>
        /// <param name="cam">The camera into which to switch</param>
        public void Switch(UnityEngine.Camera cam) {
            UnityEngine.Camera[] allCams = FindObjectsOfType<UnityEngine.Camera>();
            if (!allCams.ToList().Contains(cam)) return;

            foreach (UnityEngine.Camera iterCam in allCams) {
                AudioListener audio = iterCam.GetComponent<AudioListener>();
                CameraPreferences camPref = iterCam.GetComponent<CameraPreferences>();
                bool isNewCam = iterCam == cam;
                bool isOldCam = iterCam == CurrentActive;

                iterCam.tag = isNewCam ? CAMERA_TAG : UNTAGGED_TAG;
                iterCam.enabled = (camPref != null && camPref.AlwaysOn) || isNewCam;
                if (audio != null) audio.enabled = isNewCam;

                //activate an optional callback method
                if (isNewCam) camPref?.OnActivation();
                else if (isOldCam) camPref?.OnDeactivation();
            }

            CurrentActive = cam;
        }
    }
}