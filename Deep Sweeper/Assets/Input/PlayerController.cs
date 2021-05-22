using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Singleton<PlayerController>
{
    #region Class Members
    private PlayerControls controls;
    private bool movingHorizontally;
    private bool movingVertically;
    #endregion

    #region Events
    public event UnityAction PrimaryOperationStartEvent;
    public event UnityAction PrimaryOperationStopEvent;
    public event UnityAction SecondaryOperationStartEvent;
    public event UnityAction SecondaryOperationStopEvent;
    public event UnityAction CursorDisplayEvent;
    public event UnityAction CursorDisplayHide;
    public event UnityAction HorizontalMovementStopEvent;
    public event UnityAction VerticalMovementStopEvent;

    /// <param type=typeof(int)>Commander's index</param>
    public event UnityAction<int> CommanderSelectionEvent;

    /// <param type=typeof(float)>
    /// A positive (0:1] value when ascending
    /// or a negative [-1:0) when descending.
    /// </param>
    public event UnityAction<float> VerticalMovementEvent;

    /// <param type=typeof(Vector2)>
    /// X slot represents a Z axis movement (backwards [-1:0) and forwards (0:1])
    /// and Y slot represents an X axis movement (left [-1:0) and right (0:1])
    /// </param>
    public event UnityAction<Vector2> HorizontalMovementEvent;
    #endregion

    #region Properties
    public Vector2 Horizontal => controls.Player.Horizontal.ReadValue<Vector2>();
    public Vector2 Vertical => controls.Player.Vertical.ReadValue<Vector2>();
    public Vector2 MouseDelta => controls.Player.Look.ReadValue<Vector2>();
    #endregion

    protected override void Awake() {
        base.Awake();
        this.controls = new PlayerControls();
        controls.Enable();
        BindEvents();
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

    /// <summary>
    /// Bind keys' press, hold or stop events.
    /// </summary>
    private void BindEvents() {
        //mobility
        controls.Player.Horizontal.performed += delegate {
            if (!movingHorizontally) StartCoroutine(InvokeHorizontalMovement());
        };

        controls.Player.Vertical.performed += delegate {
            if (!movingVertically) StartCoroutine(InvokeVerticalMovement());
        };

        //shooting system
        controls.Player.PrimaryOperation.started += delegate { PrimaryOperationStartEvent?.Invoke(); };
        controls.Player.PrimaryOperation.canceled += delegate { PrimaryOperationStopEvent?.Invoke(); };
        controls.Player.SecondaryOperation.started += delegate { SecondaryOperationStartEvent?.Invoke(); };
        controls.Player.SecondaryOperation.canceled += delegate { SecondaryOperationStopEvent?.Invoke(); };

        //cursor operations
        controls.UI.CursorDisplay.started += delegate { CursorDisplayEvent?.Invoke(); };
        controls.UI.CursorHide.started += delegate { CursorDisplayHide?.Invoke(); };

        //commander system
        controls.UI.CommanderSelection1.started += delegate { CommanderSelectionEvent?.Invoke(0); };
        controls.UI.CommanderSelection2.started += delegate { CommanderSelectionEvent?.Invoke(1); };
        controls.UI.CommanderSelection3.started += delegate { CommanderSelectionEvent?.Invoke(2); };
    }

    /// <summary>
    /// Contineously invoke horizontal movement events
    /// as long as the player holds the movement keys.
    /// </summary>
    private IEnumerator InvokeHorizontalMovement() {
        movingHorizontally = true;

        while (Horizontal.magnitude > 0) {
            HorizontalMovementEvent?.Invoke(Horizontal);
            yield return null;
        }

        HorizontalMovementStopEvent?.Invoke();
        movingHorizontally = false;
    }

    /// <summary>
    /// Contineously invoke verical movement events
    /// as long as the player holds the movement keys.
    /// </summary>
    private IEnumerator InvokeVerticalMovement() {
        movingVertically = true;

        while (Mathf.Abs(Vertical.y) > 0) {
            VerticalMovementEvent?.Invoke(Vertical.y);
            yield return null;
        }

        VerticalMovementStopEvent?.Invoke();
        movingVertically = false;
    }
}