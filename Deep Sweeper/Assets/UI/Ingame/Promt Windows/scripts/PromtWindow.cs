using UnityEngine;
using UnityEngine.Events;

public class PromtWindow : MonoBehaviour
{
    #region Events
    public event UnityAction windowConfirmedEvent;
    public event UnityAction windowCanceledEvent;
    #endregion

    /// <summary>
    /// Close this promt window.
    /// </summary>
    public virtual void Close() {
        CursorViewer.Instance.Lock = false;
        CursorViewer.Instance.Display = false;
        Destroy(gameObject);
    }

    /// <summary>
    /// Activate when any of the window's confirm buttons is pressed.
    /// </summary>
    protected void OnConfirm() {
        windowConfirmedEvent?.Invoke();
    }

    /// <summary>
    /// Activate when any of the window's cancel buttons is pressed.
    /// </summary>
    protected void OnCancel() {
        windowCanceledEvent?.Invoke();
    }
}