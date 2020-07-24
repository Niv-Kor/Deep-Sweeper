using UnityEngine;

public class CursorViewer : MonoBehaviour
{
    [Tooltip("View the cursor regularly during gameplay.")]
    [SerializeField] public bool showCursor = true;

    void Update() {
        bool pressEsc = Input.GetKeyDown(KeyCode.Escape);
        bool clickMouse = Input.GetMouseButton(0);

        //toggle cursor
        if (pressEsc || (!showCursor && clickMouse)) showCursor = !showCursor;

        Cursor.lockState = showCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !showCursor;
    }
}