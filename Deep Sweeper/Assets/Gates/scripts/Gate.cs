using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Gate : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The scale at which the emblem's x axis widens when the gate opens.\n"
           + "A scale of 1 means that the x axis scales at the same rate as the y axis.")]
    [SerializeField] private float xScaleMultiplier = 0;

    [Header("Timing")]
    [Tooltip("The time it takes the upper edge to disappear when the gate opens.")]
    [SerializeField] private float upperEdgeShutTime;

    [Tooltip("The time it takes the force field to disappear when the gate opens.")]
    [SerializeField] private float forceFielsShutTime;

    [Tooltip("The time it takes the emblem to disappear when the gate opens.")]
    [SerializeField] private float emblemShutTime;

    [Tooltip("The time it takes to enable the gate camera before opening it.")]
    [SerializeField] private float pauseBeforeCam;

    [Tooltip("The time it takes to open the gate after the camera has been switched.")]
    [SerializeField] private float pauseBeforeOpen;

    [Tooltip("The time it takes to disable the gate camera after it has been opened.")]
    [SerializeField] private float pauseAfterOpen;

    private static readonly Color TRANSPARENT = new Color(0xff, 0xff, 0xff, 0x0);

    private Gradient forceFieldGradient;
    private LineRenderer upperEdge;
    private Camera cam;
    private ParticleSystem forceField;
    private Transform emblem;
    private SubmarineMovementController submarine;
    private UnityAction onFullyBlankHandler1;
    private UnityAction onFullyBlankHandler2;
    private UnityAction onFullyTransparentHandler1;
    private UnityAction onFullyTransparentHandler2;

    public event UnityAction GateOpenEvent;

    public Phase Phase { get; set; }
    public bool IsOpen { get; private set; }

    private void Start() {
        this.submarine = FindObjectOfType<SubmarineMovementController>();
    }

    /// <summary>
    /// Initialize the component.
    /// </summary>
    /// <param name="forceField">The force field's particle system</param>
    /// <param name="emblem">The emblems transform</param>
    public void Initiate(ParticleSystem forceField, Transform emblem) {
        this.forceField = forceField;
        this.emblem = emblem;
        this.cam = GetComponentInChildren<Camera>();
        this.upperEdge = GetComponentInChildren<LineRenderer>();
        this.forceFieldGradient = new Gradient();
        GradientColorKey[] shades = { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) };
        GradientAlphaKey[] alpha = { new GradientAlphaKey(1, 0f), new GradientAlphaKey(0, 1f) };
        forceFieldGradient.SetKeys(shades, alpha);

        //first time the screen goes blank
        this.onFullyBlankHandler1 = new UnityAction(delegate {
            CameraManager.Instance.Switch(cam);
            BlankScreen.Instance.FullyBlankEvent -= onFullyBlankHandler1;
        });

        //seconds time the screen goes blank
        this.onFullyBlankHandler2 = new UnityAction(delegate {
            CameraManager.Instance.Switch(CameraManager.Instance.FPCam);
            BlankScreen.Instance.FullyBlankEvent -= onFullyBlankHandler2;
        });

        this.onFullyTransparentHandler1 = new UnityAction(delegate {
            StartCoroutine(OnFullyTransparentScreen());
            BlankScreen.Instance.FullyTransparentEvent -= onFullyTransparentHandler1;
        });

        this.onFullyTransparentHandler2 = new UnityAction(delegate {
            submarine.MovementAllowd = true;
            BlankScreen.Instance.FullyTransparentEvent -= onFullyTransparentHandler2;
        });
    }

    /// <summary>
    /// Open the gate.
    /// </summary>
    public void Open() {
        StartCoroutine(AnimateGateOpening());
    }

    /// <summary>
    /// Slowly animate the opening of the gate,
    /// while switching between the cameras.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AnimateGateOpening() {
        yield return new WaitForSeconds(pauseBeforeCam);
        submarine.MovementAllowd = false;
        BlankScreen.Instance.FullyBlankEvent += onFullyBlankHandler1;
        BlankScreen.Instance.FullyTransparentEvent += onFullyTransparentHandler1;
        BlankScreen.Instance.Apply(1, 1);
    }

    /// <summary>
    /// Activate when the blank screen is fully gone.
    /// </summary>
    private IEnumerator OnFullyTransparentScreen() {
        yield return new WaitForSeconds(pauseBeforeOpen);
        StartCoroutine(ShutForceField());
        StartCoroutine(ShutEmblem());
        StartCoroutine(ShutUpperEdge());
        float openTime = Mathf.Max(forceFielsShutTime, emblemShutTime, upperEdgeShutTime);
        GateOpenEvent?.Invoke();
        yield return new WaitForSeconds(openTime + pauseAfterOpen);

        BlankScreen.Instance.FullyBlankEvent += onFullyBlankHandler2;
        BlankScreen.Instance.FullyTransparentEvent += onFullyTransparentHandler2;
        BlankScreen.Instance.Apply(1, 1);
    }

    /// <summary>
    /// Dismiss the force field.
    /// </summary>
    private IEnumerator ShutForceField() {
        var mainModule = forceField.main;
        var colorModule = forceField.colorOverLifetime;
        colorModule.color = forceFieldGradient;
        mainModule.startLifetime = forceFielsShutTime;
        forceField.Clear();
        forceField.Emit(1);
        mainModule.startColor = TRANSPARENT;

        yield return new WaitForSeconds(3);
        forceField.gameObject.SetActive(false);
    }

    /// <summary>
    /// Dismiss the emblem.
    /// </summary>
    private IEnumerator ShutEmblem() {
        Vector3 scale = emblem.localScale;
        float timer = 0;

        while (timer <= emblemShutTime) {
            timer += Time.deltaTime;
            float newY = Mathf.Lerp(scale.y, 0, timer / emblemShutTime);
            float deltaY = Mathf.Abs(emblem.localScale.y - newY);
            float newX = emblem.localScale.x + deltaY * xScaleMultiplier;
            emblem.localScale = new Vector3(newX, newY, scale.z);
            yield return null;
        }

        emblem.gameObject.SetActive(false);
    }

    /// <summary>
    /// Dismiss the upper edge.
    /// </summary>
    private IEnumerator ShutUpperEdge() {
        Gradient gardient = new Gradient();
        GradientColorKey[] shades = { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) };
        float timer = 0;

        while (timer <= upperEdgeShutTime) {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / upperEdgeShutTime);
            GradientAlphaKey[] alphaKeys = { new GradientAlphaKey(alpha, 0f), new GradientAlphaKey(alpha, 1f) };
            gardient.SetKeys(shades, alphaKeys);
            upperEdge.colorGradient = gardient;
            yield return null;
        }
    }
}