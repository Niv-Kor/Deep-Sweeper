using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Singleton<PlayerController>
{

    #region Class Members
    private PlayerControls controls;
    #endregion

    #region Events
    public event UnityAction PrimaryOperationEvent;
    public event UnityAction SecondaryOperationEvent;
    public event UnityAction CursorDisplayEvent;
    public event UnityAction CursorDisplayHide;
    public event UnityAction<int> CommanderSelectionEvent;
    #endregion

    #region Properties
    public Vector2 Horizontal => controls.Player.Horizontal.ReadValue<Vector2>();
    public Vector2 Vertical => controls.Player.Vertical.ReadValue<Vector2>();
    public Vector2 MouseDelta => controls.Player.Look.ReadValue<Vector2>();
    public float Turbo => controls.Player.Turbo.ReadValue<float>();
    #endregion

    private void Awake() {
        this.controls = new PlayerControls();
        controls.Enable();

        //bind event
        controls.Player.PrimaryOperation.started += delegate { PrimaryOperationEvent?.Invoke(); };
        controls.Player.SecondaryOperation.started += delegate { SecondaryOperationEvent?.Invoke(); };
        controls.UI.CursorDisplay.started += delegate { CursorDisplayEvent?.Invoke(); };
        controls.UI.CursorHide.started += delegate { CursorDisplayHide?.Invoke(); };
        controls.UI.CommanderSelection1.started += delegate { CommanderSelectionEvent?.Invoke(0); };
        controls.UI.CommanderSelection2.started += delegate { CommanderSelectionEvent?.Invoke(1); };
        controls.UI.CommanderSelection3.started += delegate { CommanderSelectionEvent?.Invoke(2); };
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }
}