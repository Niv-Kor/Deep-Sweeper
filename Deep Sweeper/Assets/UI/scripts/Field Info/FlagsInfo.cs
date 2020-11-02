using System.Collections.Generic;

public class FlagsInfo : CounterInfo
{
    #region Class Members
    private List<Sweeper> sweepers;
    #endregion

    protected override void Awake() {
        this.sweepers = new List<Sweeper>();
        base.Awake();
    }

    /// <inheritdoc/>
    protected override void BindEvents() {
        FlagsManager flagsMngr = FlagsManager.Instance;

        flagsMngr.FlagTakenEvent += delegate(bool success) {
            if (success) SetCounter(0, flagsMngr.AvailableFlags);
        };

        flagsMngr.FlagReturnedEvent += delegate(bool success) {
            if (success) SetCounter(0, flagsMngr.AvailableFlags);
        };

        flagsMngr.FlagsAmountUpdateEvent += delegate {
            MineField field = GameFlow.Instance.CurrentPhase.Field;
            SetCounter(0, flagsMngr.AvailableFlags);

            //unbind sweeper events
            foreach (Sweeper sweeper in sweepers)
                sweeper.MineDisposalStartEvent -= OnMineSweeped;

            //find unsweeped mines amount and bind new sweeper events
            sweepers.Clear();
            var grids = field.Grids;
            int unsweeped = grids.Count;
            foreach (MineGrid grid in grids) {
                Sweeper sweeper = grid.Sweeper;
                if (sweeper.IsDismissed) unsweeped--;
                sweeper.MineDisposalStartEvent += OnMineSweeped;
                sweepers.Add(sweeper);
            }

            SetCounter(1, unsweeped);
        };
    }

    /// <summary>
    /// Activate when a mine in the current phase's field is being sweeped.
    /// This function updates the right counter and decreases its value by 1.
    /// </summary>
    private void OnMineSweeped() {
        CounterSettings counter = GetCounter(1);
        int currentValue = int.Parse(counter.textComponent.text);
        SetCounter(1, currentValue - 1);
    }
}