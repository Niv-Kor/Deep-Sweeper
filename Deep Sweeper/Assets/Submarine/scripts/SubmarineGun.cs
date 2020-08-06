using UnityEngine;

public class SubmarineGun : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("The player submarine's rigidbody.")]
    [SerializeField] private Rigidbody submarine;

    [Header("Physics")]
    [Tooltip("The force in which the fire recoil takes place.")]
    [SerializeField] private float recoil;

    private ParticleSystem[] particles;

    private void Start() {
        this.particles = GetComponentsInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Move the submarine backwards with a recoil shock.
    /// </summary>
    private void Recoil() {
        Transform FPCam = CameraManager.Instance.FPCam.transform;
        Vector3 backwards = FPCam.forward * -1;
        Vector3 downwards = FPCam.up * -1;
        submarine.AddForce((backwards - downwards) * recoil);
    }

    /// <summary>
    /// Fire a missile from the center of the sight.
    /// </summary>
    public void Fire() {
        foreach (ParticleSystem particle in particles) particle.Play();
        Recoil();
    }
}