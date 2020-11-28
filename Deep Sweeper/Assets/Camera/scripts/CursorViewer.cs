using UnityEngine;
using UnityEngine.Events;

public class CursorViewer : Singleton<CursorViewer>
{
    #region Exposed Editor Parameters
    [Tooltip("The key that triggers the display of the cursor.")]
    [SerializeField] private KeyCode displayKey = KeyCode.Escape;

    [Tooltip("View the cursor as the game begins.")]
    [SerializeField] private bool displayOnStart = true;
    #endregion

    #region Class Members
    private bool m_display;
    #endregion

    #region Events
    public UnityAction<bool> StatusChangeEvent;
    #endregion

    #region Properties
    public bool Display {
        get { return m_display; }
        set {
            if (!Lock) {
                m_display = value;
                Cursor.visible = value;
                Cursor.lockState = !value ? CursorLockMode.Locked : CursorLockMode.None;
                StatusChangeEvent?.Invoke(value);
            }
        }
    }

    public bool Lock { get; set; }
    #endregion

    private void Start() {
        this.Lock = false;
        this.Display = displayOnStart;
    }

    private void Update() {
        if (Lock) return;

        bool keyPress = Input.GetKeyDown(displayKey);
        bool leftClick = Input.GetMouseButton(0);

        //toggle cursor
        if (keyPress && !Display) Display = true;
        else if (leftClick && Display) Display = false;
    }
}