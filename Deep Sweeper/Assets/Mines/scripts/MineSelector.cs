using System.Collections;
using UnityEngine;

public class MineSelector : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("The mine's avatar object.")]
    [SerializeField] private GameObject avatar;

    [Header("Materials")]
    [Tooltip("The defaultive mine material.")]
    [SerializeField] private Material defaultMaterial;

    [Tooltip("The material to apply on a flagged mine.")]
    [SerializeField] private Material flagMaterial;

    [Header("Timing")]
    [Tooltip("The time it takes to lerp between one material to another.")]
    [SerializeField] private float applyTime;

    private Material materialComponent;
    private Renderer render;
    private SelectionMode m_mode;

    public delegate void ModeApplication(SelectionMode mode, Material material);
    public event ModeApplication ModeApplicationEvent;

    public SelectionMode Mode {
        get { return m_mode; }
        set {
            Material nextMat = GetMaterial(value);

            if (nextMat != null) {
                StopAllCoroutines();
                StartCoroutine(Lerp(value, nextMat));
                m_mode = value;
            }
        }
    }

    private void Awake() {
        this.render = avatar.GetComponent<MeshRenderer>();
        this.materialComponent = render.material;
        this.m_mode = SelectionMode.DEFAULT;
    }

    /// <summary>
    /// Get a selection mode's corresponding material.
    /// </summary>
    /// <param name="mode">The model that represents the wished material</param>
    /// <returns>The corresponding material.</returns>
    private Material GetMaterial(SelectionMode mode) {
        switch (mode) {
            case SelectionMode.DEFAULT: return defaultMaterial;
            case SelectionMode.FLAG: return flagMaterial;
            default: return null;
        }
    }

    /// <summary>
    /// Transit between two of the mine's materials.
    /// </summary>
    /// <param name="mode">Selection mode to apply</param>
    /// <param name="to">The material into which to transition</param>
    private IEnumerator Lerp(SelectionMode mode, Material target) {
        float lerpedTime = 0;
        bool invokedEvent = false;

        while (lerpedTime <= applyTime) {
            lerpedTime += Time.deltaTime / applyTime;
            materialComponent.Lerp(materialComponent, target, lerpedTime);

            //invoke an event once after lerping halfway
            if (!invokedEvent && lerpedTime > applyTime / 2) {
                ModeApplicationEvent?.Invoke(mode, target);
                invokedEvent = true;
            }

            yield return null;
        }
    }
}