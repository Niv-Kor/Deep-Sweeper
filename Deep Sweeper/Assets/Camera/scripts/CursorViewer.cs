using UnityEngine;
using UnityEngine.Events;

public class CursorViewer : Singleton<CursorViewer>
{
    #region Exposed Editor Parameters
    [Tooltip("View the cursor as the game begins.")]
    [SerializeField] private bool displayOnStart = true;
    #endregion

    #region Class Members
    private bool m_isDisplayed;
    #endregion

    #region Events
    /// <param type=typeof(bool)>True of the cursor is displayed</param>
    public UnityAction<bool> StatusChangeEvent;
    #endregion

    #region Properties
    public bool Lock { get; set; }
    public bool IsDisplayed {
        get { return m_isDisplayed; }
        set {
            if (!Lock) {
                m_isDisplayed = value;
                Cursor.visible = value;
                Cursor.lockState = !value ? CursorLockMode.Locked : CursorLockMode.None;
                StatusChangeEvent?.Invoke(value);
            }
        }
    }
    #endregion

    private void Start() {
        this.Lock = false;
        this.IsDisplayed = displayOnStart;

        PlayerController.Instance.CursorDisplayEvent += OnCursorDisplayClick;
        PlayerController.Instance.CursorDisplayHide += OnCursorHideClick;
    }

    /// <summary>
    /// Activate when the cursor display key is pressed.
    /// </summary>
    private void OnCursorDisplayClick() {
        if (!Lock && !IsDisplayed) IsDisplayed = true;
    }

    /// <summary>
    /// Activate when the cursor hide key is pressed.
    /// </summary>
    private void OnCursorHideClick() {
        if (!Lock && IsDisplayed) IsDisplayed = false;
    }
}