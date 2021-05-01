using DeepSweeper.Camera;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubmarineGun : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("A prefab of a bullet.")]
    [SerializeField] private GameObject bulletPrefab;

    [Tooltip("The barrel object that contains all released bullets.")]
    [SerializeField] private GameObject barrel;

    [Header("Physics")]
    [Tooltip("The force in which the fire recoil takes place.")]
    [SerializeField] private float recoil;

    [Tooltip("The speed of the bullet.")]
    [SerializeField] private float speed;
    #endregion

    #region Class Members
    private ParticleSystem[] recoilParticles;
    private Rigidbody submarineRB;
    #endregion

    #region Properties
    public int BarrelContent {
        get {
            Bullet[] bullets = barrel.GetComponentsInChildren<Bullet>();
            IEnumerable<Bullet> active = from bullet in bullets
                                         where bullet.IsActive
                                         select bullet;

            return active.Count();
        }
    }
    #endregion

    private void Start() {
        //create barrel
        this.submarineRB = Submarine.Instance.GetComponent<Rigidbody>();
        this.recoilParticles = GetComponentsInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Move the submarine backwards with a recoil shock.
    /// </summary>
    private void Recoil() {
        Transform FPCam = IngameCameraManager.Instance.FPCam.transform;
        Vector3 backwards = FPCam.forward * -1;
        Vector3 downwards = FPCam.up * -1;
        submarineRB.AddForce((backwards - downwards) * recoil);
    }

    /// <summary>
    /// Launch a missile from the center of the sight.
    /// </summary>
    /// <param name="fwdDir">The direction at which the bullet is fired</param>
    /// <param name="upDir">The upwards direction relative to the bullet's direction</param>
    /// <param name="recoil">True to apply submarine recoil</param>
    /// <param name="targetMine">The mine that's in the center of the sight at the time of launch</param>
    /// <param name="ignoreBarrelContent">
    /// True to ignore the fact that the barrel
    /// might already consist of a bullet.
    /// If set to false, a bullet will not be fired
    /// when the barrel is not empty.
    /// </param>
    /// <returns>True if a bullet has been fired successfully.</returns>
    public bool Fire(Vector3 fwdDir, Vector3 upDir, bool recoil, MineGrid targetMine, bool ignoreBarrelContent = false) {
        if (ignoreBarrelContent || BarrelContent == 0) {
            //create bullet
            GameObject bulletInstance = Instantiate(bulletPrefab);
            bulletInstance.transform.SetParent(barrel.transform);
            bulletInstance.transform.localPosition = Vector3.zero;
            bulletInstance.transform.rotation = Quaternion.LookRotation(fwdDir, upDir);

            //temporarily set the mine's layer as a terget mine
            if (targetMine != null) {
                Bullet bulletCmp = bulletInstance.GetComponent<Bullet>();
                targetMine.TempTarget = true;

                //ensure the mine has been detonated and revert its layer
                bulletCmp.BulletHitEvent += delegate {
                    targetMine.TriggerHit(BulletHitType.SingleHit, true);
                    targetMine.TempTarget = false;
                };
            }

            if (recoil) {
                foreach (ParticleSystem particle in recoilParticles) particle.Play();
                Recoil();
            }

            return true;
        }
        else return false;
    }
}