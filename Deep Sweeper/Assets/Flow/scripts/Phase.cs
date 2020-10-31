public class Phase
{
    public Phase PreviousPhase { get; private set; }
    public Phase FollowPhase { get; set; }
    public MineField Field { get; private set; }
    public Gate EnteranceGate { get; private set; }
    public Gate ExitGate { get; private set; }

    /// <param name="index">The index of the phase (0 based)</param>
    /// <param name="field">The phase's mine field</param>
    /// <param name="gate">A gate from this phase to the following one</param>
    /// <param name="prevPhase">The previous phase link</param>
    public Phase(int index, MineField field, Gate gate, Phase prevPhase) {
        this.Field = field;
        this.ExitGate = gate;
        this.EnteranceGate = prevPhase?.ExitGate;
        this.PreviousPhase = prevPhase;

        if (gate != null) gate.Phase = this;

        //disable field if it's not the first phase
        if (index != 0) Field.FieldReadyEvent += delegate { ActivateGrids(false); };
    }

    /// <summary>
    /// Start the phase.
    /// </summary>
    private void Initiate() {
        ActivateGrids(true);
        GameFlow.Instance.ReportPhaseUpdated();
    }

    /// <summary>
    /// Activate or deactivate the mine field's grids.
    /// </summary>
    /// <param name="flag">True to activate or false to deactivate</param>
    /// <param name="destoryField">True to permanently destory the mine field object</param>
    public void ActivateGrids(bool flag, bool destoryField = false) {
        if (!flag && destoryField) Field.DestroyField();
        else Field.gameObject.SetActive(flag);
    }

    /// <summary>
    /// Wait until the field is ready and then initiate the phase.
    /// </summary>
    public void InitiateWhenReady() {
        if (!Field.IsReady) Field.FieldReadyEvent += Initiate;
        else Initiate();
    }

    /// <summary>
    /// Finish the phase.
    /// </summary>
    public void Conclude() {
        if (ExitGate != null) {
            ExitGate.Open();
            ActivateGrids(false, true);
        }
        else {
            ///TODO win
        }
    }
}