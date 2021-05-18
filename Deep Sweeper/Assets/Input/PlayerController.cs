using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Singleton<PlayerController>
{
    #region Class Members
    private PlayerControls controls;
    #endregion

    #region Events
    public event UnityAction PrimaryOperationStartEvent;
    public event UnityAction PrimaryOperationStopEvent;
    public event UnityAction SecondaryOperationStartEvent;
    public event UnityAction SecondaryOperationStopEvent;
    public event UnityAction CursorDisplayEvent;
    public event UnityAction CursorDisplayHide;

    /// <param type=typeof(int)>Commander's index</param>
    public event UnityAction<int> CommanderSelectionEvent;
    #endregion

    #region Properties
    public Vector2 Horizontal => controls.Player.Horizontal.ReadValue<Vector2>();
    public Vector2 Vertical => controls.Player.Vertical.ReadValue<Vector2>();
    public Vector2 MouseDelta => controls.Player.Look.ReadValue<Vector2>();
    public float Turbo => controls.Player.Turbo.ReadValue<float>();
    #endregion

    protected override void Awake() {
        base.Awake();
        this.controls = new PlayerControls();
        controls.Enable();

        //bind event
        controls.Player.PrimaryOperation.started += delegate { PrimaryOperationStartEvent?.Invoke(); };
        controls.Player.PrimaryOperation.canceled += delegate { PrimaryOperationStopEvent?.Invoke(); };
        controls.Player.SecondaryOperation.started += delegate { SecondaryOperationStartEvent?.Invoke(); };
        controls.Player.SecondaryOperation.canceled += delegate { SecondaryOperationStopEvent?.Invoke(); };
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