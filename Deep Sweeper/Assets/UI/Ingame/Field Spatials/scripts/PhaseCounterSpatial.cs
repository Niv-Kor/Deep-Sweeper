using TMPro;
using UnityEngine;

public class PhaseCounterSpatial : PhaseSpatial<PhaseCounterSpatial>
{
    #region Exposed Editor Parameters
    [Tooltip("The text component that consists of the phase's number.")]
    [SerializeField] private TextMeshProUGUI modifiableText;
    #endregion

    protected override void Start() {
        base.Start();
        GameFlow.Instance.PhaseUpdatedEvent += SetPhaseNumber;
    }

    /// <summary>
    /// Change the phase number.
    /// </summary>
    /// <param name="number">The new phase number</param>
    private void SetPhaseNumber(int number) {
        modifiableText.text = NumericUtils.Romanify(number + 1);
    }

    /// <summary>
    /// Display or hide the phase title.
    /// </summary>
    /// <param name="flag">True to display or false to hide</param>
    public void Display(bool flag) { Enabled = flag; }
}