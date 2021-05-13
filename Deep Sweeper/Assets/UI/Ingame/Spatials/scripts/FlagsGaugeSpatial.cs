using DeepSweeper.Flow;
using DeepSweeper.Level.Mine;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FlagsGaugeSpatial : PhaseSpatial<FlagsGaugeSpatial>
{
    [Serializable]
    protected struct CounterSettings
    {
        [Tooltip("A list of the cunters' text components.")]
        [SerializeField] public TextMeshProUGUI textComponent;

        [Tooltip("The counter's corresponding font size for each amount of characters\n"
               + "(as noted by the list indices).")]
        [SerializeField] public List<int> fontSizes;
    }

    #region Constants
    private static readonly int AVAILABLE_FLAGS_COUNTER = 0;
    private static readonly int AVAILABLE_MINES_COUNTER = 1;
    #endregion

    #region Exposed Editor Parameters
    [Tooltip("A list of modifiable counters.")]
    [SerializeField] private List<CounterSettings> counters;
    #endregion

    #region Class Members
    private FlagsManager flagsMngr;
    private List<DetonationSystem> detonationSystems;
    #endregion

    #region Properties
    public int AvailableFlags => GetCounter(AVAILABLE_FLAGS_COUNTER);
    public int AvailableMines => GetCounter(AVAILABLE_MINES_COUNTER);
    private List<DeepSweeper.Level.Mine.MineGrid> CurrentGrids => LevelFlow.Instance.CurrentPhase.Field.Grids;
    #endregion

    protected override void Start() {
        this.flagsMngr = FlagsManager.Instance;
        this.detonationSystems = new List<DetonationSystem>();

        flagsMngr.FlagTakenEvent += OnFlagsStateChanged;
        flagsMngr.FlagReturnedEvent += OnFlagsStateChanged;
        flagsMngr.FlagsAmountUpdateEvent += delegate { OnFlagsStateChanged(true); };

        base.Start();
    }

    /// <summary>
    /// Activate when the amount of available flags or undetonated mines
    /// in the current field changes.
    /// </summary>
    /// <param name="update">True to update counters</param>
    private void OnFlagsStateChanged(bool update) {
        if (update) {
            FetchAvailableFlags();
            FetchUndetonatedAmount();
        }
    }

    /// <summary>
    /// Set the value of a counter's text component.
    /// </summary>
    /// <param name="index">The index of the counter's text component</param>
    /// <param name="text">The counter's new string value</param>
    private void SetCounter(int index, string text) {
        CounterSettings counter = counters[index];
        TextMeshProUGUI textCmp = counter.textComponent;
        List<int> fontSizes = counter.fontSizes;
        int len = text.Length;
        bool listAvailable = fontSizes.Count > 0;
        bool sizeDefined = fontSizes.Count > len;
        int defaultSize = listAvailable ? fontSizes[fontSizes.Count - 1] : 0;
        int size = sizeDefined ? fontSizes[len] : defaultSize;
        textCmp.fontSize = size;

        base.SetText(textCmp, text, textCmp.text != text);
    }

    /// <see cref="SetCounter(int, string)"/>
    /// <param name="text">The counter's new numeric value</param>
    private void SetCounter(int index, int num) {
        if (GetCounter(index) != num) SetCounter(index, num.ToString());
    }

    /// <param name="index">The index of the counter</param>
    /// <returns>The counter's value</returns>
    private int GetCounter(int index) {
        bool success = int.TryParse(counters[index].textComponent.text, out int amount);
        return success ? amount : 0;
    }

    /// <summary>
    /// Set the amount in the available flags counter.
    /// </summary>
    /// <param name="num">The new amount</param>
    private void SetAvailableFlags(int num) {
        SetCounter(AVAILABLE_FLAGS_COUNTER, num);
    }

    /// <summary>
    /// Set the amount in the available mines counter.
    /// </summary>
    /// <param name="num">The new amount</param>
    private void SetAvailableMines(int num) {
        SetCounter(AVAILABLE_MINES_COUNTER, num);
    }

    /// <summary>
    /// Activate when a mine in the current phase's field is being detonated.
    /// This function updates the right counter and decreases its value by 1.
    /// </summary>
    private void OnMineSweeped() {
        SetAvailableMines(AvailableMines - 1);
    }

    /// <summary>
    /// Activate when this spatial is activated.
    /// This method updates the flags counters.
    /// </summary>
    private void OnDisplay() {
        //bind an event to each of the field's mines
        foreach (DeepSweeper.Level.Mine.MineGrid grid in CurrentGrids) {
            DetonationSystem detonateionSys = grid.DetonationSystem;
            detonateionSys.DetonationEvent += OnMineSweeped;
            detonationSystems.Add(detonateionSys);
        }

        FetchAvailableFlags();
        FetchUndetonatedAmount();
    }

    /// <summary>
    /// Find the amount of available flags in the current field
    /// and update the relevant counter.
    /// </summary>
    private void FetchAvailableFlags() {
        SetAvailableFlags(flagsMngr.AvailableFlags);
    }

    /// <summary>
    /// Find the amount of undetonated mines in the current field
    /// and update the relevant counter.
    /// </summary>
    private void FetchUndetonatedAmount() {
        int unsweeped = (from grid in CurrentGrids
                         where !grid.DetonationSystem.IsDetonated
                         select grid).Count();

        SetAvailableMines(unsweeped);
    }

    /// <summary>
    /// Display or hide the spatial.
    /// </summary>
    /// <param name="flag">True to display or false to hide</param>
    public void Display(bool flag) {
        Enabled = flag;

        if (flag) flagsMngr.FlagsAmountUpdateEvent += OnDisplay;
        else {
            //unbind sweeper events
            foreach (DetonationSystem sweeper in detonationSystems)
                sweeper.DetonationEvent -= OnMineSweeped;

            flagsMngr.FlagsAmountUpdateEvent -= OnDisplay;
            detonationSystems.Clear();
        }
    }
}