using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The mine's avatar object.")]
    [SerializeField] private GameObject avatar;

    [Header("Configurations")]
    [Tooltip("A list of the possible selection configurations.")]
    [SerializeField] private List<SelectionConfig> selections;

    [Header("Timing")]
    [Tooltip("The time it takes to lerp between one material to another.")]
    [SerializeField] private float applyTime;
    #endregion

    #region Class Members
    private Material materialComponent;
    private Renderer render;
    private SelectionMode m_mode;
    #endregion

    #region Events
    public event UnityAction<SelectionMode, SelectionMode, Material> ModeApplicationStartEvent;
    public event UnityAction<SelectionMode, SelectionMode, Material> ModeApplicationHalfwayEvent;
    #endregion

    #region Public Properties
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
                StartCoroutine(Lerp(m_mode, value, nextMat));
                m_mode = value;
            }
        }
    }
    #endregion

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
    /// <param name="oldMode">Previously applied selection mode</param>
    /// <param name="newMode">Selection mode to apply</param>
    /// <param name="target">The material into which to transition</param>
    private IEnumerator Lerp(SelectionMode oldMode, SelectionMode newMode, Material target) {
        ModeApplicationStartEvent?.Invoke(oldMode, newMode, target);

        float lerpedTime = 0;
        bool invokedEvent = false;

        while (lerpedTime <= applyTime) {
            lerpedTime += Time.deltaTime / applyTime;
            materialComponent.Lerp(materialComponent, target, lerpedTime);

            //invoke an event once after lerping halfway
            if (!invokedEvent && lerpedTime > applyTime / 2) {
                ModeApplicationHalfwayEvent?.Invoke(oldMode, newMode, target);
                invokedEvent = true;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Check if a mode is considered a flagged mode.
    /// </summary>
    /// <param name="mode">The selection mode to check</param>
    /// <returns>True if the specified selection mode is a flagged mode.</returns>
    public static bool IsFlagMode(SelectionMode mode) {
        switch (mode) {
            case SelectionMode.Flagged:
            case SelectionMode.FlaggedNeighbourIndication:
                return true;

            default: return false;
        }
    }
}