using Constants;
using DeepSweeper.Player.ShootingSystem;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    #region Class Members
    private MineGrid grid;
    #endregion

    private void Start() {
        this.grid = GetComponentInParent<MineGrid>();
    }

    private void OnParticleCollision(GameObject obj) {
        LayerMask layer = Layers.BULLET;
        if (Layers.ContainedInMask(obj.layer, layer)) {
            Bullet bullet = obj.GetComponentInParent<Bullet>();
            grid.DetonationSystem.TriggerHit(bullet, true);
        }
    }
}