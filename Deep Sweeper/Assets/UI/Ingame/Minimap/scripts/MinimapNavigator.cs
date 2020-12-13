using Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MinimapNavigator : SonarRotator
{
    #region Exposed Editor Parameters
    [Header("Flicker Effect")]
    [Tooltip("The arrows that engage with the flicker effect.")]
    [SerializeField] private RawImage[] arrows;
    
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
    private bool positiveTremble;
    private float trembleTimer;
    private float extraRotation;
    #endregion

    #region Properties
    public Vector3 Target { get; private set; }
    public Vector3 Direction { get; private set; }
    public float Distance { get; private set; }
    #endregion

    protected override void Start() {
        base.Start();
        this.player = CameraManager.Instance.Rig.transform;
        this.flow = GameFlow.Instance;
        this.positiveTremble = true;
        this.extraRotation = 0;
        this.trembleTimer = 0;
        StartCoroutine(LaunchFlickerEffect());
    }

    private void Update() {
        if (flow.DuringPhase) return;

        MineField field = flow.CurrentPhase.Field;
        Vector3 pos = player.position;
        Vector3 heightVec = Vector3.up * pos.y;
        Vector3 target = Vector3.Scale(field.Center, Vector3.right + Vector3.forward) + heightVec;
        Vector3 dir = target - pos;
        bool hit = Physics.Raycast(pos, dir, out RaycastHit info, Mathf.Infinity, Layers.MINE_FIELD);
        Target = hit ? info.point : Vector3.zero;
        Direction = hit ? dir : Vector3.zero;
        Distance = hit ? info.distance : Mathf.Infinity;
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
        return base.CalcAngle() + angle + extraRotation;
    }

    /// <summary>
    /// Activate the flicker effect for each of
    /// the arrows with an appropriate delay.
    /// </summary>
    private IEnumerator LaunchFlickerEffect() {
        foreach (RawImage arrow in arrows) {
            StartCoroutine(FlickerArrow(arrow));
            yield return new WaitForSeconds(flickerDelay);
        }
    }

    /// <summary>
    /// Flicker an arrow's alpha value back and forth once.
    /// </summary>
    /// <param name="arrow">The arrow to flicker</param>
    private IEnumerator FlickerArrow(RawImage arrow) {
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