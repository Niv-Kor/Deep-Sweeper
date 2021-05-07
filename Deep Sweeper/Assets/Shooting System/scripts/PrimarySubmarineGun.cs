using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrimarySubmarineGun : SubmarineGun
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
    private SubmarineOrientation submarine;
    #endregion

    #region Properties
    protected override GunType Type => GunType.Primary;
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

    protected override void Start() {
        base.Start();
        this.submarine = Submarine.Instance.Oriantation;

        //create barrel
        this.submarineRB = Submarine.Instance.GetComponent<Rigidbody>();
        this.recoilParticles = GetComponentsInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Move the submarine backwards with a recoil shock.
    /// </summary>
    private void Recoil() {
        Vector3 backwards = submarine.Forward * -1;
        submarineRB.AddForce((backwards - Vector3.down) * recoil);
    }

    /// <summary>
    /// Launch a missile from the center of the sight.
    /// </summary>
    /// <param name="fwdDir">The direction at which the bullet is fired</param>
    /// <param name="recoil">True to apply submarine recoil</param>
    /// <param name="targetGrid">The grid that's in the center of the sight at the time of launch</param>
    /// <param name="ignoreBarrelContent">
    /// True to ignore the fact that the barrel
    /// might already consist of a bullet.
    /// If set to false, a bullet will not be fired
    /// when the barrel is not empty.
    /// </param>
    /// <returns>True if a bullet has been fired successfully.</returns>
    public bool Fire(Vector3 fwdDir, bool recoil, MineGrid targetGrid, bool ignoreBarrelContent = false) {
        if (!ignoreBarrelContent && BarrelContent > 0) return false;

        //create bullet
        GameObject bulletInstance = Instantiate(bulletPrefab);
        bulletInstance.transform.SetParent(barrel.transform);
        bulletInstance.transform.localPosition = Vector3.zero;
        bulletInstance.transform.rotation = Quaternion.LookRotation(fwdDir, Vector3.up);

        //temporarily set the mine's layer as a terget mine
        if (targetGrid != null) {
            Bullet bulletCmp = bulletInstance.GetComponent<Bullet>();
            targetGrid.TempTarget = true;

            //ensure the mine has been detonated and revert its layer
            bulletCmp.BulletHitEvent += delegate {
                targetGrid.DetonationSystem.TriggerHit(bulletCmp, true);
                targetGrid.TempTarget = false;
            };
        }

        if (recoil) {
            foreach (ParticleSystem particle in recoilParticles) particle.Play();
            Recoil();
        }

        return true;
    }

    /// <inheritdoc/>
    protected override void FireAtIndicator(SightRay.TargetInfo target) {
        //only fire the bullets if the indicator is fulfilled
        if (target.Indicator.IsIndicationFulfilled) {
            IEnumerable<MineGrid> section = from neighbour in target.Grid.Section
                                            where neighbour != null && !neighbour.DetonationSystem.IsDetonated && !neighbour.SelectionSystem.IsFlagged
                                            select neighbour;

            //fire a bullet at each of the neighbours
            if (section.Count() > 0) {
                foreach (MineGrid neighbour in section) {
                    Vector3 neighbourPos = neighbour.Avatar.transform.position;
                    Vector3 neighbourDir = Vector3.Normalize(neighbourPos - transform.position);
                    Fire(neighbourDir, false, neighbour, true);
                }
            }
            else FireAtNull();
        }
        else FireAtNull();
    }

    /// <inheritdoc/>
    protected override void FireAtMine(SightRay.TargetInfo target) {
        Fire(transform.forward, true, target.Grid);
    }

    /// <inheritdoc/>
    protected override void FireAtNull() {
        Fire(transform.forward, true, null);
    }
}
