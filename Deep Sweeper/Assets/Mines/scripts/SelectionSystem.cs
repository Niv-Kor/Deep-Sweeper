using DeepSweeper.Flow;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Level.Mine
{
    public class SelectionSystem : MineSystem
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
        private SensorsColorizer sensorsColorizer;
        private SelectionMode m_mode;
        #endregion

        #region Events
        /// <param type=typeof(SelectionMode)>The previous mode</param>
        /// <param type=typeof(SelectionMode)>The new mode</param>
        public event UnityAction<SelectionMode, SelectionMode> ModeApplicationEvent;
        #endregion

        #region Public Properties
        public MineMark Mark { get; private set; }
        public bool IsFlagged => IsFlagMode(Mode);
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
                    sensorsColorizer.Colorize(config.Color);
                    ModeApplicationEvent?.Invoke(m_mode, value);
                    m_mode = value;
                }
            }
        }
        #endregion

        private void Awake() {
            this.jukebox = GetComponent<Jukebox>();
            this.Mark = GetComponentInChildren<MineMark>();
            this.sensorsColorizer = GetComponentInChildren<SensorsColorizer>();
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
        /// Highlight all of the mine's neigbours.
        /// </summary>
        /// <param name="flag">True to highlight the neighbours or false to deselect</param>
        public void SelectSection(bool flag) {
            List<MineGrid> section = Grid.Section;

            foreach (MineGrid grid in section) {
                if (grid == null) continue;

                SelectionSystem selector = grid.SelectionSystem;
                MineActivator activator = grid.Activator;

                //activate or deactivate each of the neighbours
                if (flag) activator.ActivateAndLock();
                else activator.Unlock();

                //apply the correct selection mode
                selector.SelectAsNeighbour(flag);
            }
        }

        /// <summary>
        /// Select this mine as a neighbour or as local.
        /// </summary>
        /// <param name="flag">True to select the mine as neighbour or false to select it as local</param>
        public void SelectAsNeighbour(bool flag) {
            if (IsNeighbourMode(Mode) != flag) {
                if (flag) Mode = GetNeighbourVersion(Mode);
                else Mode = GetLocalVersion(Mode);
            }
        }

        /// <summary>
        /// Flag or unflag a mine.
        /// </summary>
        /// <param name="flag">True to flag the mine or false to unflag it</param>
        public void ApplyFlag(bool flag) {
            if (IsFlagged != flag) {
                if (flag) Mode = GetFlaggedVersion(Mode);
                else Mode = GetUnflaggedVersion(Mode);
            }
        }

        /// <summary>
        /// Flag a mine or cancel it if it's already flagged.
        /// </summary>
        public void ToggleFlag() { ApplyFlag(!IsFlagged); }

        /// <summary>
        /// Get the local version of a selection mode (a non-neighbour version).
        /// If the given mode is already the local version of itself, it will be returned.
        /// </summary>
        /// <param name="mode">The selection mode of which to get the local version</param>
        /// <returns>The local version of the given mode.</returns>
        public static SelectionMode GetLocalVersion(SelectionMode mode) {
            if (!IsNeighbourMode(mode)) return mode;
            else {
                switch (mode) {
                    case SelectionMode.NeighbourIndication: return SelectionMode.Default;
                    case SelectionMode.FlaggedNeighbourIndication: return SelectionMode.Flagged;
                    default: return SelectionMode.None;
                }
            }
        }

        /// <summary>
        /// Get the neighbour version of a selection mode.
        /// If the given mode is already the neighbour version of itself, it will be returned.
        /// </summary>
        /// <param name="mode">The selection mode of which to get the neighbour version</param>
        /// <returns>The neighbour version of the given mode.</returns>
        public static SelectionMode GetNeighbourVersion(SelectionMode mode) {
            if (IsNeighbourMode(mode)) return mode;
            else {
                switch (mode) {
                    case SelectionMode.Default: return SelectionMode.NeighbourIndication;
                    case SelectionMode.Flagged: return SelectionMode.FlaggedNeighbourIndication;
                    default: return SelectionMode.None;
                }
            }
        }

        /// <summary>
        /// Get the unflagged version of a selection mode.
        /// If the given mode is already the unflagged version of itself, it will be returned.
        /// </summary>
        /// <param name="mode">The selection mode of which to get the unflagged version</param>
        /// <returns>The unflagged version of the given mode.</returns>
        public static SelectionMode GetUnflaggedVersion(SelectionMode mode) {
            if (!IsFlagMode(mode)) return mode;
            else {
                switch (mode) {
                    case SelectionMode.Flagged: return SelectionMode.Default;
                    case SelectionMode.FlaggedNeighbourIndication: return SelectionMode.NeighbourIndication;
                    default: return SelectionMode.None;
                }
            }
        }

        /// <summary>
        /// Get the flagged version of a selection mode.
        /// If the given mode is already the flagged version of itself, it will be returned.
        /// </summary>
        /// <param name="mode">The selection mode of which to get the flagged version</param>
        /// <returns>The flagged version of the given mode.</returns>
        public static SelectionMode GetFlaggedVersion(SelectionMode mode) {
            if (IsFlagMode(mode)) return mode;
            else {
                switch (mode) {
                    case SelectionMode.Default: return SelectionMode.Flagged;
                    case SelectionMode.NeighbourIndication: return SelectionMode.FlaggedNeighbourIndication;
                    default: return SelectionMode.None;
                }
            }
        }

        /// <summary>
        /// Check if a mode is considered a neighbour mode.
        /// </summary>
        /// <param name="mode">The selection mode to check</param>
        /// <returns>True if the specified selection mode is a neighbour mode.</returns>
        public static bool IsNeighbourMode(SelectionMode mode) {
            switch (mode) {
                case SelectionMode.NeighbourIndication:
                case SelectionMode.FlaggedNeighbourIndication:
                    return true;

                default: return false;
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
}