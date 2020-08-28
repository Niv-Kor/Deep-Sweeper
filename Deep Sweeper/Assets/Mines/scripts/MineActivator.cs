using UnityEngine;

public class MineActivator : ObjectActivator
{
    [Header("Prefabs")]
    [Tooltip("The chain and indicator's parent object.")]
    [SerializeField] private GameObject[] affectedObjects;

    [Tooltip("Distance from the play within which the mine is activated.")]
    [SerializeField] private float activationRange;

    private MeshClone[] clones;
    private Sweeper sweeper;

    private void Awake() {
        this.sweeper = GetComponent<Sweeper>();
        this.clones = GetComponentsInChildren<MeshClone>();
        var scouters = GetComponentsInChildren<ActivationScouter>();
        var player = FindObjectOfType<SubmarineDepthControl>().transform;

        foreach (ActivationScouter scouter in scouters) {
            scouter.Activator = this;
            scouter.Decider = player;
            scouter.ReportRange = activationRange;
        }

        //deactivate clones when the mine disposes
        sweeper.MineDesposalEvent += delegate {
            foreach (MeshClone clone in clones) clone.DisplayMesh(false);
        };
    }

    protected override void Enable(bool flag) {
        if (sweeper.IsDismissed) return;

        foreach (GameObject obj in affectedObjects) obj.SetActive(flag);
        foreach (MeshClone clone in clones) clone.DisplayMesh(!flag);
    }
}