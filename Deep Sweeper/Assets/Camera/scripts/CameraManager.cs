using System.Linq;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    #region Exposed Editor Parameters
    [Header("First Person")]
    [Tooltip("First person camera.")]
    [SerializeField] private Camera fpCam;

    [Tooltip("Camera rig object.")]
    [SerializeField] private Transform rig;

    [Tooltip("The first person camera's post processing manager component.")]
    [SerializeField] private PostProcessingManager fpPostProcess;

    [Header("Minimap")]
    [Tooltip("First person camera.")]
    [SerializeField] private Camera minimapCam;

    [Tooltip("The minimap camera's post processing manager component.")]
    [SerializeField] private PostProcessingManager minimapPostProcess;
    #endregion

    #region Constants
    private static readonly string UNTAGGED_TAG = "Untagged";
    private static readonly string CAMERA_TAG = "MainCamera";
    #endregion

    #region Public Properties
    public Camera FPCam { get { return fpCam; } }
    public Camera MinimapCam { get { return minimapCam; } }
    public Transform Rig { get { return rig; } }
    public PostProcessingManager FPPostProcess { get { return fpPostProcess; } }
    public PostProcessingManager MinimapPostProcess { get { return minimapPostProcess; } }
    #endregion

    /// <summary>
    /// Switch the main camera.
    /// </summary>
    /// <param name="cam">The camera into which to switch</param>
    public void Switch(Camera cam) {
        Camera[] allCams = FindObjectsOfType<Camera>();
        if (!allCams.ToList().Contains(cam)) return;

        foreach (Camera iterCam in allCams) {
            AudioListener audio = iterCam.GetComponent<AudioListener>();
            CameraPreferences camPref = iterCam.GetComponent<CameraPreferences>();
            bool isSelected = iterCam == cam;
            iterCam.tag = isSelected ? CAMERA_TAG : UNTAGGED_TAG;
            iterCam.enabled = (camPref != null && camPref.AlwaysOn) || isSelected;
            if (audio != null) audio.enabled = isSelected;
        }
    }
}