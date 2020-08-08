using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    private BulletHit hitParent;

    private void Start() {
        this.hitParent = GetComponentInParent<BulletHit>();
    }

    private void OnParticleSystemStopped() {
        hitParent.NotifyParticlesDeath();
    }
}
