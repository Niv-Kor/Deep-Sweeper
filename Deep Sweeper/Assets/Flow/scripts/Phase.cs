public class Phase
{
    #region Class Members
    private Gate m_entranceGate;
    #endregion

    #region Properties
    public Phase PreviousPhase { get; private set; }
    public Phase FollowPhase { get; set; }
    public MineField Field { get; private set; }
    public Gate ExitGate { get; private set; }
    public string MapName { get; private set; }
    public int Index { get; private set; }
    public PhaseConfig Config { get; private set; }
    public PhaseDifficultyConfig DifficultyConfig {
        get {
            DifficultyLevel difficulty = Contract.Instance.Difficulty;
            return Config.Levels.Find(x => x.Difficulty == difficulty);
        }
    }

    public Gate EntranceGate {
        get { return m_entranceGate; }
        private set {
            if (value != null) {
                m_entranceGate = value;
                m_entranceGate.GateCrossEvent += Begin;
            }
        }
    }
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
    /// <param name="difficulty">The difficulty of the phase</param>
    public void Begin() {
        DifficultyLevel difficulty = Contract.Instance.Difficulty;
        int levelTimer = DifficultyConfig.Clock;
        SpatialsManager.Instance.Activate(difficulty, levelTimer);
        GameFlow.Instance.ReportPhaseUpdated(Config, DifficultyConfig, Index);
        Field.Activate();
    }

    /// <summary>
    /// Finish the phase.
    /// </summary>
    public void Conclude() {
        if (ExitGate != null) {
            SpatialsManager.Instance.Deactivate();
            ExitGate.RequestOpen(true);
        }
        else {
            ///TODO win
        }
    }
}