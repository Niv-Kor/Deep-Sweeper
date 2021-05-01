using System;
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

        [Tooltip("The color to which the selected mine should change.")]
        [SerializeField] public Color Color;

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

    #region Constants
    private static readonly string FLAG_CHECK_SFX = "flag_check";
    #endregion

    #region Class Members
    private Jukebox jukebox;
    private MineSensorsManager sensorsMngr;
    private SelectionMode m_mode;
    #endregion

    #region Events
    /// <param type=typeof(SelectionMode)>The previous mode</param>
    /// <param type=typeof(SelectionMode)>The new mode</param>
    public event UnityAction<SelectionMode, SelectionMode> ModeApplicationEvent;
    #endregion
    
    #region Public Properties
    public MineMark Mark { get; private set; }
    public SelectionMode Mode {
        get { return m_mode; }
        set {
            if (m_mode == value) return;

            SelectionConfig config = GetConfiguration(value);
            Sprite sprite = config.Sprite;
            Mark.Display(sprite != null, applyTime, sprite);

            bool fromFlagged = IsFlagMode(m_mode);
            bool toFlagged = IsFlagMode(value);
            bool flagChange = false;
            bool permit = true;

            //check flags permission
            if (!fromFlagged && toFlagged) {
                permit = FlagsManager.Instance.TakeFlag();
                flagChange = true;
            }
            else if (fromFlagged && !toFlagged) {
                permit = FlagsManager.Instance.ReturnFlag();
                flagChange = true;
            }

            //apply mode
            if (permit) {
                if (flagChange) jukebox?.Play(FLAG_CHECK_SFX);
                sensorsMngr.Colorize(config.Color);
                ModeApplicationEvent?.Invoke(m_mode, value);
                m_mode = value;
            }
        }
    }
    #endregion

    private void Awake() {
        this.jukebox = GetComponent<Jukebox>();
        this.Mark = GetComponentInChildren<MineMark>();
        this.sensorsMngr = GetComponentInChildren<MineSensorsManager>();
        this.m_mode = SelectionMode.None;
    }

    private void Start() {
        this.Mode = SelectionMode.Default;
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