using com.ootii.Cameras;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Gate : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Emblem")]
    [Tooltip("The scale at which the emblem's x axis widens when the gate opens.\n"
           + "A scale of 1 means that the x axis scales at the same rate as the y axis.")]
    [SerializeField] private float xScaleMultiplier = 0;

    [Header("Dolly")]
    [Tooltip("The speed of the camera's dolly.")]
    [SerializeField] private float dollySpeed = 1;

    [Header("Timing")]
    [Tooltip("The time it takes the upper edge to disappear when the gate opens.")]
    [SerializeField] private float upperEdgeShutTime;

    [Tooltip("The time it takes the force field to disappear when the gate opens.")]
    [SerializeField] private float forceFieldShutTime;

    [Tooltip("The time it takes the emblem to disappear when the gate opens.")]
    [SerializeField] private float emblemShutTime;

    [Tooltip("The time it takes to enable the gate camera before opening it.")]
    [SerializeField] private float pauseBeforeCam;

    [Tooltip("The time it takes to open the gate after the camera has been switched.")]
    [SerializeField] private float pauseBeforeOpen;

    [Tooltip("The time it takes to disable the gate camera after it has been opened.")]
    [SerializeField] private float pauseAfterOpen;
    #endregion

    #region Class Members
    private LineRenderer upperEdge;
    private Camera cam;
    private ForceField forceField;
    private Transform emblem;
    private SubmarineMovementController submarine;
    private CameraController camController;
    private UnityAction onFullyBlankHandler1;
    private UnityAction onFullyBlankHandler2;
    private UnityAction onFullyTransparentHandler1;
    private UnityAction onFullyTransparentHandler2;
    private bool crossedGate;
    private bool initiated;
    #endregion

    #region Events
    public event UnityAction GateOpenEvent;
    public event UnityAction GateCrossEvent;
    private event UnityAction ReadyForOpenEvent;
    #endregion

    #region Properties
    public Phase Phase { get; set; }
    public bool IsOpen { get; private set; }
    #endregion

    private void Start() {
        this.submarine = FindObjectOfType<SubmarineMovementController>();
        this.camController = FindObjectOfType<CameraController>();
        this.crossedGate = false;
        this.initiated = false;
    }

    /// <summary>
    /// Initialize the component.
    /// </summary>
    /// <param name="forceField">The force field's particle system</param>
    /// <param name="emblem">The emblems transform</param>
    public void Initiate(ForceField forceField, Transform emblem) {
        this.forceField = forceField;
        this.emblem = emblem;
        this.cam = GetComponentInChildren<Camera>();
        this.upperEdge = GetComponentInChildren<LineRenderer>();

        //bind gate cross event
        forceField.CrossEvent += OnGateCrossed;

        //first time the screen goes blank
        this.onFullyBlankHandler1 = new UnityAction(delegate {
            camController.enabled = false;
            StartCoroutine(MoveDolly());
            CameraManager.Instance.Switch(cam);
            BlankScreen.Instance.FullyBlankEvent -= onFullyBlankHandler1;
        });

        //seconds time the screen goes blank
        this.onFullyBlankHandler2 = new UnityAction(delegate {
            camController.enabled = true;
            StopAllCoroutines();
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

        initiated = true;
        ReadyForOpenEvent?.Invoke();
    }

    /// <summary>
    /// Activate when the gate is crossed for the first time.
    /// </summary>
    private void OnGateCrossed() {
        GateCrossEvent?.Invoke();
        forceField.CrossEvent -= OnGateCrossed;
    }

    /// <summary>
    /// Request the opening of the gate.
    /// The gate will open as soon as it's ready and initiated.
    /// </summary>
    public void RequestOpen(bool animate) {
        if (initiated) Open(animate);
        else ReadyForOpenEvent += delegate { Open(animate); };
    }

    /// <summary>
    /// Open the gate.
    /// </summary>
    private void Open(bool animate) {
        if (animate) StartCoroutine(AnimateGateOpening());
        else {
            forceField.ShutFieldEmission(forceFieldShutTime);
            forceField.Collider.isTrigger = true;
            emblem.gameObject.SetActive(false);
            upperEdge.gameObject.SetActive(false);
            GateOpenEvent?.Invoke();
        }
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
        float openTime = Mathf.Max(forceFieldShutTime, emblemShutTime, upperEdgeShutTime);
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
        forceField.ShutFieldEmission(forceFieldShutTime);
        yield return new WaitForSeconds(3);
        Destroy(forceField.ParticleSystem);
        forceField.Collider.isTrigger = true;
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

        upperEdge.gameObject.SetActive(false);
    }

    /// <summary>
    /// Move the camera dolly towards the gate.
    /// </summary>
    private IEnumerator MoveDolly() {
        Transform camTransform = cam.transform;
        Transform forceTransform = forceField.transform;
        float camDist = Vector3.Distance(camTransform.position, forceTransform.position);
        float minimalDist = camDist * .5f;

        do {
            Vector3 source = camTransform.position;
            Vector3 dest = forceTransform.position;
            float speed = Time.deltaTime * dollySpeed;
            cam.transform.position = Vector3.Lerp(source, dest, speed);
            camDist = Vector3.Distance(camTransform.position, forceTransform.position);
            yield return null;
        }
        while (camDist > minimalDist);
    }
}