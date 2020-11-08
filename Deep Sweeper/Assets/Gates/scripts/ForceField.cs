using Constants;
using UnityEngine;
using UnityEngine.Events;

public class ForceField : MonoBehaviour
{
    #region Constants
    private static readonly Color TRANSPARENT = new Color(0xff, 0xff, 0xff, 0x0);
    #endregion

    #region Class Members
    private Gradient gradient;
    #endregion

    #region Events
    public event UnityAction CrossEvent;
    #endregion

    #region Properties
    public Collider Collider { get; private set; }
    public ParticleSystem ParticleSystem { get; private set; }
    #endregion

    private void Awake() {
        this.ParticleSystem = GetComponent<ParticleSystem>();
        this.Collider = GetComponent<Collider>();
        this.gradient = new Gradient();
        GradientColorKey[] shades = { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) };
        GradientAlphaKey[] alpha = { new GradientAlphaKey(1, 0f), new GradientAlphaKey(0, 1f) };
        gradient.SetKeys(shades, alpha);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == Layers.GetLayerValue(Layers.PLAYER)) CrossEvent?.Invoke();
    }

    /// <summary>
    /// Dismiss the force field's physical particles.
    /// </summary>
    /// <param name="time">The time it takes the particles to die off.</param>
    public void ShutFieldEmission(float time = 0) {
        var mainModule = ParticleSystem.main;
        var colorModule = ParticleSystem.colorOverLifetime;
        colorModule.color = gradient;
        mainModule.startLifetime = time;
        ParticleSystem.Clear();
        ParticleSystem.Emit(1);
        mainModule.startColor = TRANSPARENT;
    }
}