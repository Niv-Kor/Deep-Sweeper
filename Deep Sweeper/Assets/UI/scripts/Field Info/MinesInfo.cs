using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinesInfo : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Parameters")]
    [Tooltip("Selected mines counter.")]
    [SerializeField] private TextMeshProUGUI selectedMinesText;

    [Tooltip("Overall field mines counter.")]
    [SerializeField] private TextMeshProUGUI fieldMinesAmountText;
    #endregion

    #region Class Members
    private int fieldMinesAmount;
    private int selectedAmount;
    private List<MineSelector> currentSelectors;
    #endregion

    private void Awake() {
        this.currentSelectors = new List<MineSelector>();
        GameFlow.Instance.PhaseUpdatedEvent += CollectFieldInfo;
    }

    /// <summary>
    /// Collect and update the UI info according to the current phase.
    /// </summary>
    private void CollectFieldInfo() {
        //clear previously assigned event listeners from mine selectors
        foreach (MineSelector selector in currentSelectors)
            selector.ModeApplicationStartEvent -= OnModeApplicationStart;

        currentSelectors.Clear();

        //reset mine amounts
        MineField field = GameFlow.Instance.CurrentPhase.Field;
        List<MineGrid> mines = field.Grids;
        fieldMinesAmount = field.MinesAmount;
        selectedAmount = 0;

        foreach (MineGrid mine in mines) {
            MineSelector selector = mine.Selector;
            currentSelectors.Add(selector);
            selector.ModeApplicationStartEvent += OnModeApplicationStart;
        }

        UpdateUIComponents();
    }

    /// <summary>
    /// Activate on MineSelector.ModeApplicationEvent.
    /// This function updates the amount of selected mines in the current field.
    /// </summary>
    /// <param name="mode">The previous mine selection mode</param>
    /// <param name="mode">The applied mine selection mode</param>
    private void OnModeApplicationStart(SelectionMode oldMode, SelectionMode newMode, Material _) {
        if (oldMode == newMode) return;
        bool previouslyFlagged = MineSelector.IsFlagMode(oldMode);

        switch (newMode) {
            //new flagged mine
            case SelectionMode.Flagged:
            case SelectionMode.FlaggedNeighbourIndication:
                if (!previouslyFlagged) selectedAmount++;
                break;

            //mine deflagged
            case SelectionMode.Default:
            case SelectionMode.NeighbourIndication:
                if (previouslyFlagged) selectedAmount--;
                break;
        }

        UpdateUIComponents();
    }

    /// <summary>
    /// Update the counters' text components.
    /// </summary>
    private void UpdateUIComponents() {
        selectedMinesText.text = selectedAmount.ToString();
        fieldMinesAmountText.text = fieldMinesAmount.ToString();
    }
}