using UnityEngine;

public class MarineParticlesSpawner : MarineSpawner
{
    protected override void ApplyEmission(GameObject instance, int emission) {
        var emissionCurve = new ParticleSystem.MinMaxCurve(emission);
        ParticleSystem particles = instance.GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule module = particles.emission;
        module.rateOverTime = emissionCurve;
    }
}