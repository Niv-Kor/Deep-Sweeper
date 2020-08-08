using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    [Tooltip("The layer of the submarine bullets.")]
    [SerializeField] private LayerMask bulletsLayer;

    private MineGrid grid;

    private void Start() {
        this.grid = GetComponentInParent<MineGrid>();
    }

    private void OnParticleCollision(GameObject obj) {
        if (Constants.Layers.ContainedInMask(obj.layer, bulletsLayer)) grid.TriggerHit();
    }
}