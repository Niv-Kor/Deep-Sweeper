public class FlagsInfo : CounterInfo
{
    /// <inheritdoc/>
    protected override void AssignCounters() {
        FlagsManager flagsMngr = FlagsManager.Instance;
        flagsMngr.FlagTakenEvent += delegate { LeftCounter = flagsMngr.AvailableFlags; };
        flagsMngr.FlagReturnedEvent += delegate { LeftCounter = flagsMngr.AvailableFlags; };
        flagsMngr.FlagsAmountUpdateEvent += delegate {
            RightCounter = flagsMngr.MaxFlags;
            LeftCounter = flagsMngr.AvailableFlags;
        };
    }
}