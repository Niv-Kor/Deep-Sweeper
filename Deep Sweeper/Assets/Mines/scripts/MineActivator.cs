using UnityEngine;
using UnityEngine.Rendering;

public class MineActivator : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("The mine head avatar object.")]
    [SerializeField] private GameObject mineHead;

    [Tooltip("The ChainRoot components on one of the chain joints.")]
    [SerializeField] private ChainRoot chainRoot;

    [Tooltip("The joints whose kinematics will toggle according to their activation state.")]
    [SerializeField] private Rigidbody[] joints;

    [Header("Settings")]
    [Tooltip("The distance from the submarine from which the chain will start moving.")]
    [SerializeField] private float enableAtDistance;

    private GameObject submarine;
    private MeshRenderer avatarRenderer;

    public bool ChainEnabled {
        get { return chainRoot.enabled; }
        set {
            chainRoot.enabled = value;
            Activate(value);
        }
    }

    private void Start() {
        this.submarine = GameObject.FindGameObjectWithTag("Player");
        this.avatarRenderer = mineHead.GetComponent<MeshRenderer>();
        this.ChainEnabled = false;
    }

    private void Update() {
        float distance = Vector3.Distance(submarine.transform.position, transform.position);

        if (!ChainEnabled && distance <= enableAtDistance) {
            ChainEnabled = true;
        }
        else if (ChainEnabled && distance > enableAtDistance) {
            ChainEnabled = false;
        }
    }

    /// <summary>
    /// Activate the chain's rigibody and gravity properties.
    /// </summary>
    /// <param name="flag"></param>
    private void Activate(bool flag) {
        foreach (Rigidbody joint in joints) joint.isKinematic = !flag;
        avatarRenderer.shadowCastingMode = flag ? ShadowCastingMode.On : ShadowCastingMode.Off;
        avatarRenderer.receiveShadows = flag;
    }
}