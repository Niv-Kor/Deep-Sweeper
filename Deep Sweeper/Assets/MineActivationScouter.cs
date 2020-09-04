public class MineActivationScouter : ActivationScouter
{
    private MineGrid grid;
    private SubmarineOriantation submarine;

    protected override void Start() {
        base.Start();
        this.grid = GetComponentInParent<MineGrid>();
        this.submarine = FindObjectOfType<SubmarineOriantation>();
    }

    /// <inheritdoc/>
    protected override bool InDeciderRange() {
        Phase phase = submarine.CurrentPhase;
        bool inRangeGrid = phase != null && phase.Field.ContainsGrid(grid);
        return inRangeGrid && base.InDeciderRange();
    }
}