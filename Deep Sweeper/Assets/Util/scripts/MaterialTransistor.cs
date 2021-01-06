using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialTransistor : MonoBehaviour
{
    [Tooltip("A list of the materials within which the mesh can transist.")]
    [SerializeField] private Material[] materials;

    [Tooltip("The time it takes the mesh to transist once.")]
    [SerializeField] private float timer;

    private Renderer render;
    private int index;

    private void Start() {
        this.index = 0;
    }

    /// <summary>
    /// Transist to the next material in the list.
    /// </summary>
    public void Transist() {
        if (materials.Length < 2) return;
        int nextIndex;

        if (index + 1 < materials.Length) nextIndex = index + 1;
        else nextIndex = 0;

        Transist(nextIndex);
    }

    /// <summary>
    /// Transist to the specified indexed material.
    /// </summary>
    /// <param name="matIndex">The index of the material in the list</param>
    public void Transist(int matIndex) {
        int len = materials.Length;

        if (len < 2 || index >= len || matIndex >= len) return;
        else StopAllCoroutines();

        Material fromMat = materials[index];
        Material toMat = materials[matIndex];
        index = matIndex;

        StartCoroutine(Lerp(fromMat, toMat));
    }

    /// <summary>
    /// Lerp between two materials.
    /// </summary>
    /// <param name="from">The material from which to transist</param>
    /// <param name="to">The material into which to transist</param>
    private IEnumerator Lerp(Material from, Material to) {
        if (materials.Length == 0) yield break;
        float lerpedTime = 0;

        while (lerpedTime < timer) {
            lerpedTime += Time.deltaTime;
            render.material.Lerp(from, to, lerpedTime);
            yield return null;
        }
    }
}