using System;

public class Phase
{
    #region Class Members
    public Phase PreviousPhase { get; private set; }
    public Phase FollowPhase { get; set; }
    public MineField Field { get; private set; }
    public Gate EntranceGate { get; private set; }
    public Gate ExitGate { get; private set; }
    #endregion

    #region Properties
    public string MapName { get; private set; }
    public int Index { get; private set; }
    public PhaseConfig Config { get; private set; }
    #endregion

    /// <param name="index">The index of the phase (0 based)</param>
    /// <param name="field">The phase's mine field</param>
    /// <param name="prevPhase">
    /// The phase that comes before this one
    /// (can be null if it's the first phase)
    /// </param>
    /// <param name="prevPhase">The previous phase link</param>
    public Phase(int index, MineField field, Phase prevPhase, PhaseConfig config) {
        this.Index = index;
        this.Field = field;
        this.Config = config;
        this.MapName = config.MapName;
        this.ExitGate = config.Gate;
        this.EntranceGate = prevPhase?.ExitGate;
        this.PreviousPhase = prevPhase;
        ActivateField(false);

        if (config.Gate != null) config.Gate.Phase = this;
    }

    /// <param name="index">The index of the phase (0 based)</param>
    /// <param name="field">The phase's mine field</param>
    /// <param name="leadGate">The gate that leads to this phase</param>
    /// <param name="prevPhase">The previous phase link</param>
    public Phase(int index, MineField field, Gate leadGate, PhaseConfig config) : this(index, field, (Phase) null, config) {
        this.EntranceGate = leadGate;
    }

    /// <summary>
    /// Begin the phase's timer and game mechanics.
    /// The phase must first be initialized.
    /// </summary>
    public void Begin() {
        ActivateField(true);
        GameFlow.Instance.ReportPhaseUpdated();
    }

    /// <summary>
    /// Activate or deactivate the phase's field.
    /// </summary>
    /// <param name="flag">True to activate or false to deactivate</param>
    /// <param name="destoryField">True to permanently destory the mine field object</param>
    public void ActivateField(bool flag, bool destoryField = false) {
        if (!flag && destoryField) Field.DestroyField();
        else {
            Field.gameObject.SetActive(flag);
            if (flag) Field.Begin();
        }
    }

    /// <summary>
    /// Finish the phase.
    /// </summary>
    public void Conclude() {
        if (ExitGate != null) {
            ExitGate.RequestOpen(true);
            ActivateField(false, true);
        }
        else {
            ///TODO win
        }
    }
}