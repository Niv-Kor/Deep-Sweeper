﻿using System.Collections;
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

    private Rigidbody rigidBody;
    private MeshRenderer render;
    private SphereCollider col;
    private ParticleSystem[] particles;

    public bool IsDismissed { get; set; }

    public delegate void MineDesposal();
    public event MineDesposal MineDisposalStartEvent;
    public event MineDesposal MineDisposalEndEvent;

    private void Awake() {
        this.particles = avatar.GetComponentsInChildren<ParticleSystem>();
        this.rigidBody = avatarShell.GetComponent<Rigidbody>();
        this.render = avatar.GetComponent<MeshRenderer>();
        this.col = avatar.GetComponent<SphereCollider>();
        this.IsDismissed = false;
    }

    private void Start() {
        //assign an event trigger to the main camera
        CameraShaker camShaker = CameraManager.Instance.FPCam.GetComponent<CameraShaker>();
        if (camShaker != null) MineDisposalStartEvent += delegate() { camShaker.Shake(); };
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

        if (breakChain) {
            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.useGravity = true;
            chain.gameObject.SetActive(false);
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