using System;
using System.Collections.Generic;
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

    #region Exposed Editor Parameters
    [Tooltip("A list of modifiable counters.")]
    [SerializeField] private List<CounterSettings> counters;
    #endregion

    #region Class Members
    private FlagsManager flagsMngr;
    private List<Sweeper> sweepers;
    #endregion

    protected override void Start() {
        this.flagsMngr = FlagsManager.Instance;
        this.sweepers = new List<Sweeper>();
        base.Start();
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
        SetCounter(index, num.ToString());
    }

    /// <summary>
    /// Bind or unbind FlagsManager events.
    /// </summary>
    /// <param name="flag">True to bind the events or false to unbind</param>
    private void BindEvents(bool flag) {
        FlagsManager flagsMngr = FlagsManager.Instance;

        void UpdateFlagsCount(bool update) {
            if (update) SetCounter(0, flagsMngr.AvailableFlags);
        }

        if (flag) {
            flagsMngr.FlagTakenEvent += UpdateFlagsCount;
            flagsMngr.FlagReturnedEvent += UpdateFlagsCount;
        }
        else {
            flagsMngr.FlagTakenEvent -= UpdateFlagsCount;
            flagsMngr.FlagReturnedEvent -= UpdateFlagsCount;
        }
    }

    /// <summary>
    /// Activate when a mine in the current phase's field is being sweeped.
    /// This function updates the right counter and decreases its value by 1.
    /// </summary>
    private void OnMineSweeped() {
        CounterSettings counter = counters[1];
        int currentValue = int.Parse(counter.textComponent.text);
        SetCounter(1, currentValue - 1);
    }

    /// <summary>
    /// Activate when this spatial is activated.
    /// This method updates the flags counters.
    /// </summary>
    private void OnDisplay() {
        MineField field = GameFlow.Instance.CurrentPhase.Field;
        SetCounter(0, flagsMngr.AvailableFlags);

        //find unsweeped mines amount and bind new sweeper events
        var grids = field.Grids;
        int unsweeped = grids.Count;

        foreach (MineGrid grid in grids) {
            Sweeper sweeper = grid.Sweeper;
            if (sweeper.IsDismissed) unsweeped--;
            sweeper.MineDisposalStartEvent += OnMineSweeped;
            sweepers.Add(sweeper);
        }

        SetCounter(1, unsweeped);
    }

    /// <summary>
    /// Display or hide the spatial.
    /// </summary>
    /// <param name="flag">True to display or false to hide</param>
    public void Display(bool flag) {
        Enabled = flag;
        BindEvents(flag);

        if (flag) flagsMngr.FlagsAmountUpdateEvent += OnDisplay;
        else {
            //unbind sweeper events
            foreach (Sweeper sweeper in sweepers)
                sweeper.MineDisposalStartEvent -= OnMineSweeped;

            flagsMngr.FlagsAmountUpdateEvent -= OnDisplay;
            sweepers.Clear();
        }
    }
}