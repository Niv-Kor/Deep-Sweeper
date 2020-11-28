using com.ootii.Cameras;
using UnityEngine;

public class CursorControlCoordinator : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The conrtoller that allows the player to move.")]
    [SerializeField] private PlayerController3D playerController;

    [Tooltip("The camera controller that the cursor's display status affects.")]
    [SerializeField] private CameraController cameraController;
    #endregion

    private void Awake() {
        //auto find the mandatory movement input components
        if (playerController == null) playerController = FindObjectOfType<PlayerController3D>();
        if (cameraController == null) cameraController = FindObjectOfType<CameraController>();
        CursorViewer.Instance.StatusChangeEvent += OnCursorDisplayStatusChange;
    }

    /// <summary>
    /// Activate when the cursor's display status is changed.
    /// This function enables or disables the player's movement input components
    /// while the cursor is shown.
    /// </summary>
    /// <param name="display">True if the cursor is now being displayed after the change</param>
    private void OnCursorDisplayStatusChange(bool display) {
        if (cameraController != null) cameraController.enabled = !display;
        if (playerController != null) playerController.IsMovable = !display;
    }
}