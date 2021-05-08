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
        public DynamicCamera CurrentActive { get; protected set; }
        protected abstract DynamicCamera DefaultCamera { get; }
        #endregion

        private void Start() {
            this.CurrentActive = DefaultCamera;
        }

        /// <summary>
        /// Switch the main camera.
        /// </summary>
        /// <param name="cam">The camera into which to switch</param>
        public void Switch(DynamicCamera cam) {
            DynamicCamera[] allCams = FindObjectsOfType<DynamicCamera>();
            if (!allCams.ToList().Contains(cam)) return;

            foreach (DynamicCamera iterCam in allCams) {
                var audio = iterCam.GetComponent<AudioListener>();
                bool isNewCam = iterCam == cam;
                iterCam.tag = isNewCam ? CAMERA_TAG : UNTAGGED_TAG;
                iterCam.Enable(iterCam.AlwaysOn || isNewCam);
                if (audio != null) audio.enabled = isNewCam;
            }

            CurrentActive = cam;
        }
    }
}