using System.Collections.Generic;

public class MinesInfo : CounterInfo
{
    #region Class Members
    private List<Sweeper> sweepers;
    #endregion

    protected override void Awake() {
        this.sweepers = new List<Sweeper>();
        base.Awake();
    }

    /// <inheritdoc/>
    protected override void AssignCounters() {
        GameFlow.Instance.PhaseUpdatedEvent += delegate {
            //clear sweeper triggers
            foreach (Sweeper sweeper in sweepers)
                sweeper.MineDisposalStartEvent -= OnMineSweep;

            //init counters
            MineField field = GameFlow.Instance.CurrentPhase.Field;
            RightCounter = field.Grids.Count;
            LeftCounter = field.Grids.Count;
            sweepers.Clear();

            foreach (MineGrid grid in field.Grids) {
                Sweeper sweeper = grid.Sweeper;

                //count sweeped mines
                if (sweeper.IsDismissed) LeftCounter--;

                //assign sweeper trigger
                sweepers.Add(sweeper);
                sweeper.MineDisposalStartEvent += OnMineSweep;
            }
        };
    }

    /// <summary>
    /// Activate when a mine is being sweeped.
    /// </summary>
    private void OnMineSweep() { LeftCounter--; }
}