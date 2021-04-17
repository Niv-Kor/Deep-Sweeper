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

    #region Constants
    private static readonly string DIG_SFX = "dig";
    private static readonly string EXPLOSION_SFX = "explosion";
    #endregion

    #region Class Members
    private Jukebox jukebox;
    private MeshRenderer[] renders;
    private SphereCollider col;
    private ParticleSystem[] particles;
    private bool isMined;
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
        this.renders = avatar.GetComponentsInChildren<MeshRenderer>();
        this.col = avatar.GetComponentInChildren<SphereCollider>();
        this.jukebox = GetComponent<Jukebox>();
        this.IsDismissed = false;
    }

    private void Start() {
        this.isMined = GetComponentInParent<MineGrid>().IsMined;

        //assign an event trigger to the main camera
        MineDisposalStartEvent += ShakeCamera;
    }

    /// <summary>
    /// Shake the camera as an explosion effect.
    /// </summary>
    /// <param name="maxForce">
    /// True if the camera should shake at max force,
    /// or false if it should be relative to the mine's distance
    /// </param>
    private void ShakeCamera() {
        CameraShaker camShaker = CameraManager.Instance.FPCam.GetComponent<CameraShaker>();
        SightRay ray = SightRay.Instance;

        if (camShaker != null) {
            float shakeStrength = 5; //max force
            
            //relative to distance from mine
            if (!isMined) {
                Transform player = Submarine.Instance.transform;
                float dist = Vector3.Distance(transform.position, player.position);
                float clampedDist = Mathf.Clamp(dist, 0, ray.MaxDistance);
                shakeStrength = 1 - RangeMath.NumberOfRange(clampedDist, 0, ray.MaxDistance);
            }

            camShaker.Shake(shakeStrength);
        }
    }

    /// <summary>
    /// Vanish the mine.
    /// </summary>
    /// <param name="explosion">True to explode the mine using particle effects</param>
    /// <param name="realMine">True if the grid is mined</param>
    private IEnumerator Dismiss(bool explosion) {
        if (IsDismissed) yield break;

        MineDisposalStartEvent?.Invoke();
        foreach (var render in renders) render.enabled = false;
        col.enabled = false;
        IsDismissed = true;
        float vanishTime = 0;

        if (explosion) {
            if (isMined) jukebox.Play(EXPLOSION_SFX);
            else jukebox.Play(DIG_SFX);

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
    /// <param name="realMine">True if the grid is mined</param>
    public void Explode() { StartCoroutine(Dismiss(true)); }

    /// <summary>
    /// Quietly vanish the mine without explision.
    /// </summary>
    public void Vanish() { StartCoroutine(Dismiss(false)); }
}