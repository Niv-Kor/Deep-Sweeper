using Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MinimapNavigator : SonarRotator
{
    #region Exposed Editor Parameters
    [Header("Flicker Effect")]
    [Tooltip("The percentage of alpha value loss during the flicker effect.")]
    [SerializeField] [Range(.01f, 1f)] private float alphaLossPercent = 1;

    [Tooltip("The time it takes an arrow to flicker back and forth.")]
    [SerializeField] private float rtFlickerTime;

    [Tooltip("A delay to apply between each of the arrows that have the"
           + "flicker effect applied to them.\nIf there is only one arrow,"
           + "this delay is not applied.")]
    [SerializeField] private float flickerDelay;

    [Header("Tremble Effect")]
    [Tooltip("The angle at which the tremble effect moves from side to side.")]
    [SerializeField] private float trembleEpsilon;

    [Tooltip("A delay applied with each tremble deviation of the arrow.")]
    [SerializeField] private float trembleDelay;
    #endregion

    #region Class Members
    private Transform player;
    private GameFlow flow;
    private RawImage arrow;
    private bool positiveTremble;
    private float minDisplayDistance;
    private float trembleTimer;
    private float extraRotation;
    #endregion

    #region Properties
    public Vector3 Target { get; private set; }
    public Vector3 Direction { get; private set; }
    public float Distance { get; private set; }
    public override bool Enabled {
        get { return arrow != null && arrow.gameObject.activeSelf; }
        protected set {
            if (arrow != null)
                arrow.gameObject.SetActive(value);
        }
    }
    #endregion

    private void Awake() {
        this.arrow = GetComponentInChildren<RawImage>();
    }

    protected override void Start() {
        base.Start();
        this.player = CameraManager.Instance.Rig.transform;
        this.flow = GameFlow.Instance;
        this.positiveTremble = true;
        this.extraRotation = 0;
        this.trembleTimer = 0;

        Camera minimap = CameraManager.Instance.MinimapCam;
        this.minDisplayDistance = minimap.orthographicSize / 4;

        StartCoroutine(FlickerArrow());
    }

    private void Update() {
        if (flow.DuringPhase) {
            Enabled = false;
            return;
        }

        Vector3 pos = player.position;
        Vector3 heightVec = Vector3.up * pos.y;
        Vector3 targetPos = flow.CurrentPhase.EntranceGate.transform.position;
        Vector3 target = Vector3.Scale(targetPos, Vector3.right + Vector3.forward) + heightVec;
        float dist = Vector3.Distance(pos, target);
        Enabled = dist > minDisplayDistance;

        if (Enabled) {
            Vector3 dir = Vector3.Normalize(target - pos);
            bool hit = Physics.Raycast(pos, dir, out RaycastHit info, Mathf.Infinity, Layers.GATE);
            Direction = dir;
            Target = hit ? info.point : target;
            Distance = hit ? info.distance : dist;
        }
    }

    /// <inheritdoc/>
    protected override float CalcAngle() {
        trembleTimer += Time.deltaTime;

        if (trembleTimer >= trembleDelay) {
            float trembleMultiplier = positiveTremble ? 1 : -1;
            extraRotation = trembleEpsilon * trembleMultiplier;
            positiveTremble ^= true;
            trembleTimer = 0;
        }

        float angle = Vector3.Angle(Vector3.forward, Direction);
        Vector3 cross = Vector3.Cross(Vector3.forward, Direction);
        float tanAngle = AngleUtils.TangentiateAngle(angle);
        tanAngle *= (cross.y < 0) ? 1 : -1;

        return base.CalcAngle() + tanAngle + extraRotation;
    }

    /// <summary>
    /// Flicker an arrow's alpha value back and forth once.
    /// </summary>
    private IEnumerator FlickerArrow() {
        Color modifiedColor = arrow.color;
        float originAlpha = modifiedColor.a;
        float halfTime = rtFlickerTime / 2;
        float tempAlpha = (1 - alphaLossPercent) * originAlpha;

        while (true) {
            float timer = 0;

            while (timer <= halfTime) {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(originAlpha, tempAlpha, timer / halfTime);
                modifiedColor.a = alpha;
                arrow.color = modifiedColor;

                yield return null;
            }

            timer = 0;
            while (timer <= halfTime) {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(tempAlpha, originAlpha, timer / halfTime);
                modifiedColor.a = alpha;
                arrow.color = modifiedColor;

                yield return null;
            }
        }
    }
}