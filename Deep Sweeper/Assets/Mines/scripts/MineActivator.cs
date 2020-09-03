﻿using UnityEngine;

public class MineActivator : ObjectActivator
{
    [Header("Prefabs")]
    [Tooltip("A list of object to enable when the mine is activated.")]
    [SerializeField] private GameObject[] activateOnEnable;

    [Tooltip("A list of object to disable when the mine is activated.")]
    [SerializeField] private GameObject[] activateOnDisable;

    [Header("Configurations")]
    [Tooltip("Distance from the play within which the mine is activated.")]
    [SerializeField] private float activationRange;

    private MeshClone[] clones;
    private Sweeper sweeper;

    private void Awake() {
        this.sweeper = GetComponent<Sweeper>();
        this.clones = GetComponentsInChildren<MeshClone>();
        var scouters = GetComponentsInChildren<ActivationScouter>();
        var player = Submarine.Instance.transform;

        foreach (ActivationScouter scouter in scouters) {
            scouter.Activator = this;
            scouter.Decider = player;
            scouter.ReportRange = activationRange;
        }

        //deactivate clones when the mine disposes
        sweeper.MineDisposalStartEvent += delegate {
            foreach (MeshClone clone in clones) clone.DisplayMesh(false);
        };
    }

    /// <inheritdoc/>
    protected override void Enable(bool flag) {
        if (sweeper.IsDismissed) return;

        foreach (GameObject obj in activateOnEnable) obj.SetActive(flag);
        foreach (GameObject obj in activateOnDisable) obj.SetActive(!flag);
        foreach (MeshClone clone in clones) clone.DisplayMesh(!flag);
    }
}