using UnityEngine;

public class BulletHit : MonoBehaviour
{
    private int particlesSystemsAmount;

    private void Start() {
        ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
        this.particlesSystemsAmount = systems.Length;
    }

    /// <summary>
    /// Notify when a child particle system has ended.
    /// When all particle systems end, this object destorys itself.
    /// </summary>
    public void NotifyParticlesDeath() {
        if (--particlesSystemsAmount == 0) Destroy(gameObject);
    }
}