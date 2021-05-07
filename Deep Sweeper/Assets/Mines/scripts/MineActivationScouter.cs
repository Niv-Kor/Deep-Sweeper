public class MineActivationScouter : ActivationScouter
{
    private MineGrid grid;
    private SubmarineOrientation submarine;

    protected override void Start() {
        base.Start();
        this.grid = GetComponentInParent<MineGrid>();
        this.submarine = FindObjectOfType<SubmarineOrientation>();
    }

    /// <inheritdoc/>
    protected override bool InDeciderRange() {
        Phase phase = submarine.CurrentPhase;
        bool inRangeGrid = phase != null && phase.Field.ContainsGrid(grid);
        return inRangeGrid && base.InDeciderRange();
    }
}