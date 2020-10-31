using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Sweeper : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The mine's shell parent object.")]
    [SerializeField] private GameObject avatar;
    #endregion

    #region Class Members
    private MeshRenderer render;
    private SphereCollider col;
    private ParticleSystem[] particles;
    #endregion

    #region Events
    public event UnityAction MineDisposalStartEvent;
    public event UnityAction MineDisposalEndEvent;
    #endregion

    #region Properties
    public bool IsDismissed { get; private set; }
    #endregion

    private void Awake() {
        this.particles = avatar.GetComponentsInChildren<ParticleSystem>();
        this.render = avatar.GetComponent<MeshRenderer>();
        this.col = avatar.GetComponentInChildren<SphereCollider>();
        this.IsDismissed = false;
    }

    private void Start() {
        //assign an event trigger to the main camera
        CameraShaker camShaker = CameraManager.Instance.FPCam.GetComponent<CameraShaker>();
        SightRay ray = SightRay.Instance;

        if (camShaker != null) MineDisposalStartEvent += delegate() {
            float dist = Vector3.Distance(transform.position, camShaker.transform.position);
            float clampedDist = Mathf.Clamp(dist, 0, ray.MaxDistance);
            float shakeStrength = 1 - RangeMath.NumberOfRange(clampedDist, 0, ray.MaxDistance);
            camShaker.Shake(shakeStrength);
        };
    }

    /// <summary>
    /// Vanish the mine.
    /// </summary>
    /// <param name="explosion">True to explode the mine using particle effects</param>
    /// <param name="breakChain">True to break the chain and release the mine</param>
    private IEnumerator Dismiss(bool explosion, bool breakChain) {
        if (IsDismissed) yield break;

        MineDisposalStartEvent?.Invoke();
        render.enabled = breakChain;
        col.enabled = false;
        IsDismissed = true;
        float vanishTime = 0;

        if (explosion) {
            foreach (ParticleSystem part in particles) {
                float animationTime = part.main.startDelay.constantMax + part.main.duration;
                if (animationTime > vanishTime) vanishTime = animationTime;
                part.Play();
            }
        }

        //wait for the animation to finish and then trigger an event
        yield return new WaitForSeconds(vanishTime);
        MineDisposalEndEvent?.Invoke();
    }

    /// <summary>
    /// Explode the mine.
    /// </summary>
    public void Explode() { StartCoroutine(Dismiss(true, false)); }

    /// <summary>
    /// Quietly vanish the mine without explision.
    /// </summary>
    public void Vanish() { StartCoroutine(Dismiss(false, false)); }

    /// <summary>
    /// Break the mine's chain and release it to the surface.
    /// </summary>
    public void BreakChain() { StartCoroutine(Dismiss(false, true)); }
}