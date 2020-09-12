using UnityEngine;
using UnityEngine.Events;

public class SubmarineGun : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("The player submarine's rigidbody.")]
    [SerializeField] private Rigidbody submarine;

    [Header("Timing")]
    [Tooltip("The time it takes to load a new bullet.")]
    [SerializeField] private float loadingTime;

    [Header("Physics")]
    [Tooltip("The force in which the fire recoil takes place.")]
    [SerializeField] private float recoil;

    [Tooltip("The speed of the bullet.")]
    [SerializeField] private float speed;

    private ParticleSystem[] particles;
    private float loadingTimer;
    private bool bulletInBarrel;

    public event UnityAction BulletLoadTrigger;

    private void Start() {
        this.particles = GetComponentsInChildren<ParticleSystem>();
        this.loadingTimer = 0;
        this.bulletInBarrel = true;
        BulletLoadTrigger += delegate () { bulletInBarrel = true; };
    }

    private void Update() {
        if (!bulletInBarrel) {
            if (loadingTimer < loadingTime) loadingTimer += Time.deltaTime;
            else {
                loadingTimer = 0;
                BulletLoadTrigger?.Invoke();
            }
        }
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
        if (bulletInBarrel) {
            bulletInBarrel = false;
            foreach (ParticleSystem particle in particles) particle.Play();
            Recoil();
        }
    }
}