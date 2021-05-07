using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PromtWindow : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Buttons")]
    [Tooltip("The button that triggers the window confirmation.")]
    [SerializeField] private Button confirmButton;

    [Tooltip("The button that triggers the window cancelation.")]
    [SerializeField] private Button cancelButton;
    #endregion

    #region Events
    public event UnityAction WindowConfirmedEvent;
    public event UnityAction WindowCanceledEvent;
    #endregion

    protected virtual void Awake() {
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirm);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancel);
    }

    /// <summary>
    /// Close this promt window.
    /// </summary>
    public virtual void Close() {
        CursorViewer.Instance.Lock = false;
        CursorViewer.Instance.IsDisplayed = false;
        Destroy(gameObject);
    }

    /// <summary>
    /// Activate when any of the window's confirm buttons is pressed.
    /// </summary>
    protected void OnConfirm() {
        WindowConfirmedEvent += Close;
        WindowConfirmedEvent?.Invoke();
    }

    /// <summary>
    /// Activate when any of the window's cancel buttons is pressed.
    /// </summary>
    protected void OnCancel() {
        WindowCanceledEvent += Close;
        WindowCanceledEvent?.Invoke();
    }
}