using Constants;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    [Tooltip("The type of hit type that occurs when a bullet hits this object.")]
    [SerializeField] private BulletHitType hitType;

    private MineGrid grid;

    private void Start() {
        this.grid = GetComponentInParent<MineGrid>();
    }

    private void OnParticleCollision(GameObject obj) {
        LayerMask layer = Layers.BULLET;
        if (Layers.ContainedInMask(obj.layer, layer)) grid.TriggerHit(hitType, true);
    }
}