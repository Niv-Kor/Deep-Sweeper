using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSelector : MonoBehaviour
{
    [Serializable]
    private struct SelectionConfig
    {
        [Tooltip("The related selection mode.")]
        [SerializeField] public SelectionMode Mode;

        [Tooltip("The selection material.")]
        [SerializeField] public Material Material;

        [Tooltip("A sprite to display over the mine on selection.")]
        [SerializeField] public Sprite Sprite;
    }

    [Header("Prefabs")]
    [Tooltip("The mine's avatar object.")]
    [SerializeField] private GameObject avatar;

    [Header("Configurations")]
    [Tooltip("A list of the possible selection configurations.")]
    [SerializeField] private List<SelectionConfig> selections;

    [Header("Timing")]
    [Tooltip("The time it takes to lerp between one material to another.")]
    [SerializeField] private float applyTime;

    public delegate void ModeApplication(SelectionMode mode, Material material);
    public event ModeApplication ModeApplicationEvent;

    private Material materialComponent;
    private Renderer render;
    private SelectionMode m_mode;

    public MineMark Mark { get; private set; }
    public SelectionMode Mode {
        get { return m_mode; }
        set {
            SelectionConfig config = GetConfiguration(value);
            Material nextMat = config.Material;
            Sprite sprite = config.Sprite;
            Mark.Display(sprite != null, applyTime, sprite);

            if (nextMat != null) {
                StopAllCoroutines();
                StartCoroutine(Lerp(value, nextMat));
                m_mode = value;
            }
        }
    }

    private void Awake() {
        this.render = avatar.GetComponent<MeshRenderer>();
        this.Mark = GetComponentInChildren<MineMark>();
        this.materialComponent = render.material;
        this.m_mode = SelectionMode.Default;
    }

    /// <summary>
    /// Get a selection mode's corresponding material.
    /// </summary>
    /// <param name="mode">The model that represents the wished material</param>
    /// <returns>The corresponding material.</returns>
    private SelectionConfig GetConfiguration(SelectionMode mode) {
        return selections.Find(x => x.Mode == mode);
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