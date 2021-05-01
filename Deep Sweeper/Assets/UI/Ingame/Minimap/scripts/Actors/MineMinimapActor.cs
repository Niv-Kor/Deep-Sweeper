using UnityEngine;

public class MineMinimapActor : MinimapActor
{
    #region Exposed Editor Parameters
    [Tooltip("The color of a flagged mine's icon.")]
    [SerializeField] private Color flaggedColor;
    #endregion

    protected override void Awake() {
        base.Awake();

        MineGrid grid = GetComponent<MineGrid>();
        MineSelector selector = grid.Selector;
        Sweeper sweeper = grid.Sweeper;
        if (sweeper.IsDismissed) Sprite = null;

        //bind events
        sweeper.MineDisposalStartEvent += delegate { Sprite = null; };
        selector.ModeApplicationEvent += OnMineSelection;
    }

    /// <summary>
    /// Activate when the selection mode of the mine is changed.
    /// This method changes the color of the minimap icon accordingly.
    /// </summary>
    /// <see cref="MineSelector.ModeApplicationEvent"/>
    private void OnMineSelection(SelectionMode oldMode, SelectionMode newMode) {
        bool oldFlagged = MineSelector.IsFlagMode(oldMode);
        bool newFlagged = MineSelector.IsFlagMode(newMode);

        if (!oldFlagged && newFlagged) spriteRenderer.color = flaggedColor;
        else if (oldFlagged && !newFlagged) spriteRenderer.color = defaultColor;
    }
}