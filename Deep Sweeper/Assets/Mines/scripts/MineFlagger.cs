using UnityEngine;

public class MineFlagger : MonoBehaviour
{
    [Tooltip("The mesh renderer component of the flag itself.")]
    [SerializeField] private GameObject banner;

    [Tooltip("The time it takes to place the flag on the mine.")]
    [SerializeField] private float placementTime;

    [Tooltip("The time it takes to pull the flag off the mine.")]
    [SerializeField] private float pullingTime;

    [Tooltip("The initial height from which the flag is placed, " +
             "or rather the final height to which the flag is pulled.")]
    [SerializeField] private float fromHeight;

    private static readonly Vector3 EPSILON_SCALE = new Vector3(.01f, .01f, .01f);

    private MeshRenderer render, bannerRender;
    private FlagAnimator bannerAnimator;
    private float toHeight, startHeight;
    private float lerpedTime;
    private Vector3 defScale, startScale;
    private bool placing, pulling;

    public bool IsFlagged { get; private set; }

    private void Start() {
        this.render = GetComponent<MeshRenderer>();
        this.bannerRender = banner.GetComponent<MeshRenderer>();
        this.bannerAnimator = banner.GetComponent<FlagAnimator>();
        this.defScale = transform.localScale;
        this.fromHeight += transform.localPosition.y;
        this.toHeight = transform.localPosition.y;
        this.lerpedTime = 0;
        this.placing = false;
        this.pulling = false;
        this.IsFlagged = false;

        //initially shrink to minimum
        transform.localScale = EPSILON_SCALE;
        transform.localPosition += new Vector3(0, fromHeight - transform.localPosition.y, 0);
        Display(false);
    }

    private void Update() {
        if (!placing && !pulling) return;
        float timer = placing ? placementTime : pullingTime;

        if (lerpedTime < timer) {
            lerpedTime += Time.deltaTime;
            Vector3 destScale = placing ? defScale : EPSILON_SCALE;
            float destHeight = placing ? toHeight : fromHeight;
            Vector3 originPos = transform.localPosition;
            Vector3 destPos = transform.localPosition;
            originPos.y = startHeight;
            destPos.y = destHeight;

            transform.localScale = Vector3.Lerp(startScale, destScale, lerpedTime / timer);
            transform.localPosition = Vector3.Lerp(originPos, destPos, lerpedTime / timer);
        }
        else {
            //vanish
            if (pulling) {
                IsFlagged = false;
                Display(false);
            }

            lerpedTime = 0;
            placing = false;
            pulling = false;
        }
    }

    /// <summary>
    /// Show or hide the flag.
    /// </summary>
    /// <param name="flag">True to display the flag or false to stop rendering it</param>
    private void Display(bool flag) {
        render.enabled = flag;
        bannerAnimator.enabled = flag;
        bannerRender.enabled = flag;
    }

    /// <summary>
    /// Save the current transformation state of the flag.
    /// </summary>
    private void CacheStartingConditions() {
        startScale = transform.localScale;
        startHeight = transform.localPosition.y;
    }

    /// <summary>
    /// Place the flag on the mine.
    /// </summary>
    public void Place() {
        Display(true);
        IsFlagged = true;
        pulling = false;
        placing = true;
        lerpedTime = 0;
        CacheStartingConditions();
    }

    /// <summary>
    /// Pull the flag from the mine.
    /// </summary>
    public void Pull() {
        placing = false;
        pulling = true;
        lerpedTime = 0;
        CacheStartingConditions();
    }
}