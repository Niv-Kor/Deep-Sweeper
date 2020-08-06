using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [Tooltip("First person camera.")]
    [SerializeField] public Camera FPCam;

    [Tooltip("Third person camera.")]
    [SerializeField] public Camera TPCam;

    [Tooltip("Camera rig object.")]
    [SerializeField] public Transform Rig;
}
