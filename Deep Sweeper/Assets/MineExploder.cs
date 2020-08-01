using UnityEngine;

public class MineExploder : MonoBehaviour
{
    [Tooltip("All explosion particles to play on explosion event.")]
    [SerializeField] private ParticleSystem[] particles;

    private MeshRenderer render;
    private bool exploded;

    private void Start() {
        this.render = GetComponent<MeshRenderer>();
        this.exploded = false;
    }

    /// <summary>
    /// Explode the mine.
    /// </summary>
    public void Explode() {
        if (exploded) return;

        render.enabled = false;
        exploded = true;
        foreach (ParticleSystem part in particles) part.Play();
    }
}