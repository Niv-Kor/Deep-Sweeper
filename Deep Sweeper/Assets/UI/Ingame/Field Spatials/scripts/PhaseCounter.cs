using TMPro;
using UnityEngine;

public class PhaseCounter : MonoBehaviour
{
    #region Class Members
    private TextMeshProUGUI text;
    #endregion

    private void Start() {
        this.text = GetComponent<TextMeshProUGUI>();
        GameFlow.Instance.PhaseUpdatedEvent += SetPhaseNumber;
    }

    /// <summary>
    /// Change the phase number.
    /// </summary>
    /// <param name="number">The new phase number</param>
    private void SetPhaseNumber(int number) {
        text.text = NumericUtils.Romanify(number + 1);
    }
}