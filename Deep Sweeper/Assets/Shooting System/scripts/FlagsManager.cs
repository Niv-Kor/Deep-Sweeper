using UnityEngine.Events;

public class FlagsManager : Singleton<FlagsManager>
{
    #region Events
    public event UnityAction<bool> FlagTakenEvent;
    public event UnityAction<bool> FlagReturnedEvent;
    public event UnityAction FlagsAmountUpdateEvent;
    #endregion

    #region Public Properties
    public int MaxFlags { get; private set; }
    public int AvailableFlags { get; private set; }
    #endregion

    private void Awake() {
        GameFlow.Instance.PhaseUpdatedEvent += CollectFieldInfo;
    }

    /// <summary>
    /// Collect and update the UI info according to the current phase.
    /// </summary>
    private void CollectFieldInfo() {
        MineField field = GameFlow.Instance.CurrentPhase.Field;
        MaxFlags = field.MinesAmount;
        AvailableFlags = field.MinesAmount;
        FlagsAmountUpdateEvent?.Invoke();
    }

    /// <summary>
    /// Take a flag in order to use it on a mine.
    /// </summary>
    /// <returns>True if there are enough available flags.</returns>
    public bool TakeFlag() {
        bool success;

        if (AvailableFlags > 0) {
            AvailableFlags--;
            success = true;
        }
        else success = false;

        FlagTakenEvent?.Invoke(success);
        return success;
    }

    /// <summary>
    /// Return a flag to the pile.
    /// </summary>
    /// <returns>True if the pile was not full before returning the flag.</returns>
    public bool ReturnFlag() {
        bool success;

        if (AvailableFlags < MaxFlags) {
            AvailableFlags++;
            success = true;
        }
        else success = false;

        FlagReturnedEvent?.Invoke(success);
        return success;
    }
}