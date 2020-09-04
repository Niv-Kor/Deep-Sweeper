public class Phase
{
    public Phase PreviousPhase { get; private set; }
    public Phase FollowPhase { get; set; }
    public MineField Field { get; private set; }
    public Gate EnteranceGate { get; private set; }
    public Gate ExitGate { get; private set; }

    /// <param name="field">The phase's mine field</param>
    /// <param name="gate">A gate from this phase to the following one</param>
    /// <param name="prevPhase">The previous phase link</param>
    public Phase(MineField field, Gate gate, Phase prevPhase) {
        this.Field = field;
        this.ExitGate = gate;
        this.EnteranceGate = prevPhase?.ExitGate;
        this.PreviousPhase = prevPhase;

        if (gate != null) gate.Phase = this;
    }
}