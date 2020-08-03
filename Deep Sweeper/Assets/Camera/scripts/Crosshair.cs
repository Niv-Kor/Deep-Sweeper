using UnityEngine;
using UnityEngine.UI;

public class Crosshair : Singleton<Crosshair>
{
    [Header("Prefabs")]
    [Tooltip("Outer frame circle.")]
    [SerializeField] private RectTransform frame;

    [Tooltip("Outer circle.")]
    [SerializeField] private RectTransform outer;

    [Tooltip("Inner crosshair.")]
    [SerializeField] private RectTransform inner;

    [Header("Timing")]
    [Tooltip("The time it takes to fully execute the aim animation.")]
    [SerializeField] private float lockTime;

    [Tooltip("The time it takes to fully execute the release animation.")]
    [SerializeField] private float releaseTime;

    [Header("Animation")]
    [Tooltip("The scale to which the outer frame circle transforms when locking.")]
    [SerializeField] private float frameScale;

    [Tooltip("The scale to which the outer circle transforms when locking.")]
    [SerializeField] private float outerScale;

    [Tooltip("The angle in which the inner crosshair spins when locking.")]
    [SerializeField] private float innerAngle;

    [Tooltip("The color to which the outer circle changes when locking.")]
    [SerializeField] private Color outerColor;

    [Tooltip("The color to which the inner crosshair changes when locking.")]
    [SerializeField] private Color innerColor;

    private Vector3 defFrameScale, startFrameScale;
    private Vector3 defOuterScale, startOuterScale;
    private float defInnerAngle, startInnerAngle;
    private Color defInnerColor, startInnerColor;
    private Color defOuterColor, startOuterColor;
    private RawImage outerImg, innerImg;
    private float lerpedTime;
    private bool locking, releasing;

    private void Start() {
        this.defFrameScale = frame.localScale;
        this.defOuterScale = outer.localScale;
        this.defInnerAngle = inner.rotation.eulerAngles.z;
        this.innerImg = inner.GetComponent<RawImage>();
        this.outerImg = outer.GetComponent<RawImage>();
        this.defOuterColor = outerImg.color;
        this.defInnerColor = innerImg.color;
        this.lerpedTime = 0;
        this.locking = false;
        this.releasing = false;
    }

    private void Update() {
        if (!locking && !releasing) return;
        float timer = locking ? lockTime : releaseTime;

        if (lerpedTime < timer) {
            Vector3 destFrameScale = locking ? defFrameScale * frameScale : defFrameScale;
            Vector3 destOuterScale = locking ? defOuterScale * outerScale : defOuterScale;
            float destInnerAngle = locking ? innerAngle : defInnerAngle;
            Quaternion originInnerAngleVec = Quaternion.Euler(new Vector3(0, 0, startInnerAngle));
            Quaternion destInnerAngleVec = Quaternion.Euler(new Vector3(0, 0, destInnerAngle));
            Color destInnerColor = locking ? innerColor : defInnerColor;
            Color destOuterColor = locking ? outerColor : defOuterColor;

            //animate
            lerpedTime += Time.deltaTime;
            frame.localScale = Vector3.Lerp(startFrameScale, destFrameScale, lerpedTime / timer);
            outer.localScale = Vector3.Lerp(startOuterScale, destOuterScale, lerpedTime / timer);
            inner.rotation = Quaternion.Lerp(originInnerAngleVec, destInnerAngleVec, lerpedTime / timer);
            innerImg.color = Color.Lerp(startInnerColor, destInnerColor, lerpedTime / timer);
            outerImg.color = Color.Lerp(startOuterColor, destOuterColor, lerpedTime / timer);
        }
        else {
            lerpedTime = 0;
            locking = false;
            releasing = false;
        }
    }

    /// <summary>
    /// Save the current transformation state of the frame, outer and inner sight components.
    /// </summary>
    private void CacheStartingConditions() {
        startFrameScale = frame.localScale;
        startOuterScale = outer.localScale;
        startInnerAngle = inner.rotation.eulerAngles.z;
        startInnerColor = innerImg.color;
        startOuterColor = outerImg.color;
    }

    /// <summary>
    /// Lock the sight on the target.
    /// </summary>
    public void Lock() {
        releasing = false;
        locking = true;
        CacheStartingConditions();
        lerpedTime = 0;
    }

    /// <summary>
    /// Release the sight's lock on the target.
    /// </summary>
    public void Release() {
        locking = false;
        releasing = true;
        CacheStartingConditions();
        lerpedTime = 0;
    }
}