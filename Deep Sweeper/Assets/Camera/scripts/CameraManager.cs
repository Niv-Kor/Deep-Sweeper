using System.Linq;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [Tooltip("First person camera.")]
    [SerializeField] public Camera FPCam;

    [Tooltip("Camera rig object.")]
    [SerializeField] public Transform Rig;

    private static readonly string UNTAGGED_TAG = "Untagged";
    private static readonly string CAMERA_TAG = "MainCamera";

    /// <summary>
    /// Switch the main camera.
    /// </summary>
    /// <param name="cam">The camera into which to switch</param>
    public void Switch(Camera cam) {
        Camera[] allCams = FindObjectsOfType<Camera>();
        if (!allCams.ToList().Contains(cam)) return;

        foreach (Camera iterCam in allCams) {
            AudioListener audio = iterCam.GetComponent<AudioListener>();
            bool isSelected = iterCam == cam;
            iterCam.tag = isSelected ? CAMERA_TAG : UNTAGGED_TAG;
            iterCam.enabled = isSelected;
            audio.enabled = isSelected;
        }
    }
}