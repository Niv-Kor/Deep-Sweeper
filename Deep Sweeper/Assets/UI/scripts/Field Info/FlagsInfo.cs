public class FlagsInfo : CounterInfo
{
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
            SetCounter(1, field.Grids.Count);
        };
    }
}