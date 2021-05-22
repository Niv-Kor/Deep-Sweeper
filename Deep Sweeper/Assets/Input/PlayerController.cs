using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : Singleton<PlayerController>
{
    private class SequentialClickDetector
    {
        #region Class Members
        private int counter;
        private int maxClickDelay;
        #endregion

        #region Events
        public event UnityAction TargetSequenceEvent;
        #endregion

        #region Properties
        public int CounterGoal { get; private set; }
        #endregion

        /// <param name="targetSequence">The target amount of clicks that will invoke an event</param>
        /// <param name="maxClickDelay">The maximum time allowed between clicks (in seconds)</param>
        public SequentialClickDetector(int targetSequence, float maxClickDelay) {
            this.CounterGoal = targetSequence;
            this.maxClickDelay = (int)(maxClickDelay * 1000);
            this.counter = 0;
        }

        /// <summary>
        /// Reset the counter back to 0.
        /// </summary>
        public void ResetCounter() { counter = 0; }

        /// <summary>
        /// Increase the counter by 1 and invoke the target event if needed.
        /// </summary>
        public void IncreaseCounter() {
            if (++counter == CounterGoal) {
                TargetSequenceEvent?.Invoke();
                counter = 0;
            }
            else Task.Delay(maxClickDelay).ContinueWith(x => ResetCounter());
        }
    }

    #region Exposed Editor Parameters
    [Tooltip("The maximum time allowed between clicks that invoke multiple click events.")]
    [SerializeField] private float timeBetweenSequenceClicks = .5f;
    #endregion

    #region Class Members
    private PlayerControls controls;
    private SequentialClickDetector[] dashDetectors;
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
    public event UnityAction<Vector2> DashEvent;

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

        this.dashDetectors = new SequentialClickDetector[4];
        for (int i = 0; i < dashDetectors.Length; i++)
            dashDetectors[i] = new SequentialClickDetector(2, timeBetweenSequenceClicks);

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

        dashDetectors[0].TargetSequenceEvent += delegate { DashEvent?.Invoke(Vector2.up); };
        dashDetectors[1].TargetSequenceEvent += delegate { DashEvent?.Invoke(Vector2.right); };
        dashDetectors[2].TargetSequenceEvent += delegate { DashEvent?.Invoke(Vector2.down); };
        dashDetectors[3].TargetSequenceEvent += delegate { DashEvent?.Invoke(Vector2.left); };

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
        DetectHorizontalDash();

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

    /// <summary>
    /// Detect a double click on one of the horizontal movement keys,
    /// that indicates a dash towards that direction.
    /// </summary>
    private void DetectHorizontalDash() {
        if (Horizontal.y > 0) dashDetectors[0].IncreaseCounter();
        if (Horizontal.x > 0) dashDetectors[1].IncreaseCounter();
        if (Horizontal.y < 0) dashDetectors[2].IncreaseCounter();
        if (Horizontal.x < 0) dashDetectors[3].IncreaseCounter();
    }
}