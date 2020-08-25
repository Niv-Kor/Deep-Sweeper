using UnityEngine;

public class Sweeper : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("The mine's shell parent object.")]
    [SerializeField] private GameObject avatar;

    [Tooltip("The mine's shell parent object.")]
    [SerializeField] private GameObject avatarShell;

    [Tooltip("The chain root that holds the mine to the ground.")]
    [SerializeField] private ChainRoot chain;

    [Tooltip("All explosion particles to play on explosion event.")]
    [SerializeField] private ParticleSystem[] particles;

    private Rigidbody rigidBody;
    private MeshRenderer render;
    private SphereCollider col;

    public bool IsDismissed { get; set; }

    public delegate void MineDesposal();
    public event MineDesposal MineDesposalEvent;

    private void Awake() {
        this.rigidBody = avatarShell.GetComponent<Rigidbody>();
        this.render = avatar.GetComponent<MeshRenderer>();
        this.col = avatar.GetComponent<SphereCollider>();
        this.IsDismissed = false;
    }

    /// <summary>
    /// Vanish the mine.
    /// </summary>
    /// <param name="explosion">True to explode the mine using particle effects</param>
    /// <param name="breakChain">True to break the chain and release the mine</param>
    private void Dismiss(bool explosion, bool breakChain) {
        if (IsDismissed) return;

        MineDesposalEvent?.Invoke();
        render.enabled = breakChain;
        col.enabled = false;
        IsDismissed = true;

        if (explosion)
            foreach (ParticleSystem part in particles) part.Play();

        if (breakChain) {
            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.useGravity = true;
            chain.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Explode the mine.
    /// </summary>
    public void Explode() { Dismiss(true, false); }

    /// <summary>
    /// Quietly vanish the mine without explision.
    /// </summary>
    public void Vanish() { Dismiss(false, false); }

    /// <summary>
    /// Break the mine's chain and release it to the surface.
    /// </summary>
    public void BreakChain() { Dismiss(false, true); }
}