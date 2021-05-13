using UnityEngine;

public class MineSensor : MonoBehaviour
{
    #region Class Members
    private ParticleSystem[] particles;
    private Renderer render;
    #endregion

    private void Awake() {
        this.particles = GetComponentsInChildren<ParticleSystem>();
        this.render = GetComponent<Renderer>();
    }

    /// <summary>
    /// Break this sensor.
    /// </summary>
    public void Break() {
        foreach (ParticleSystem particle in particles) particle.Play();
        render.enabled = false;
    }
}