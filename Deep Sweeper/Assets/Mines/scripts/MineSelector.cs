using System.Collections;
using UnityEngine;

public class MineSelector : MonoBehaviour
{
    [Header("Materials")]
    [Tooltip("The defaultive mine material.")]
    [SerializeField] private Material defaultMaterial;

    [Tooltip("The material to apply on a flagged mine.")]
    [SerializeField] private Material flagMaterial;

    [Header("Timing")]
    [Tooltip("The time it takes to lerp between one material to another.")]
    [SerializeField] private float applyTime;

    private Material currentMaterial;
    private Material materialComponent;
    private SelectionModes currentMode;
    private float lerpedTime = 0;

    public SelectionModes Mode {
        get { return currentMode; }
        set { Apply(value); }
    }

    private void Awake() {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        this.materialComponent = renderer.material;
        this.currentMode = SelectionModes.DEFAULT;
        this.currentMaterial = GetMaterial(currentMode);
        this.lerpedTime = 0;
    }

    /// <summary>
    /// Get a selection mode's corresponding material.
    /// </summary>
    /// <param name="mode">The model that represents the wished material</param>
    /// <returns>The corresponding material.</returns>
    private Material GetMaterial(SelectionModes mode) {
        switch (mode) {
            case SelectionModes.DEFAULT: return defaultMaterial;
            case SelectionModes.FLAG: return flagMaterial;
            default: return null;
        }
    }

    /// <summary>
    /// Transit between two of the mine's materials.
    /// </summary>
    /// <param name="to">The material into which to transition</param>
    private IEnumerator Lerp(Material target) {
        while (lerpedTime <= applyTime) {
            lerpedTime += Time.deltaTime / applyTime;
            materialComponent.Lerp(materialComponent, target, lerpedTime);
            yield return null;
        }
    }

    /// <summary>
    /// Apply a selection to the mine.
    /// </summary>
    /// <param name="mode">Selection mode to apply</param>
    private void Apply(SelectionModes mode) {
        Material nextMat = GetMaterial(mode);
        lerpedTime = 0;

        if (nextMat != null) {
            StopAllCoroutines();
            StartCoroutine(Lerp(nextMat));
            currentMaterial = nextMat;
            currentMode = mode;
        }
    }
}