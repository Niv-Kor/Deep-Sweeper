using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineGun : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("The bullet to use when shooting")]
    [SerializeField] private GameObject missilesPrefab;

    [Tooltip("First person camera sight.")]
    [SerializeField] private GameObject sight;

    [Header("Physics")]
    [SerializeField] private float recoil;

    /*private static readonly string BULLET_PARENT_NAME = "Bullets";

    private List<GameObject> bullets;
    private GameObject bulletParent;*/
    private SubmarineMovementController submarine;
    private ParticleSystem[] particles;

    private void Start() {
        this.submarine = FindObjectOfType<SubmarineMovementController>();
        this.particles = GetComponentsInChildren<ParticleSystem>();
    }

    private void Recoil() {
        submarine.Shock(recoil);
    }

    public void Fire() {
        /*//instantiate
        GameObject instance = Instantiate(missilesPrefab);
        Vector3 forward = FPCam.transform.forward;
        //instance.transform.SetParent(bulletParent.transform);
        instance.transform.position = transform.position;
        instance.transform.rotation = Quaternion.LookRotation(forward);
        Missile missileComponent = instance.GetComponent<Missile>();
        missileComponent.Fire(target);
        missiles.Add(missileComponent);*/

        //fire
        /*Rigidbody instanceRB = instance.GetComponent<Rigidbody>();
        instanceRB.AddRelativeForce(forward * force);*/

        foreach (ParticleSystem particle in particles) particle.Play();
        Recoil();
    }
}