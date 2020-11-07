using TMPro;
using UnityEngine;

public class PhaseCounter : MonoBehaviour
{
    #region Constants
    private static readonly int[] DECIAML_VALUES = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
    private static readonly string[] ROMAN_VALUES = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
    #endregion

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
        text.text = RomanifyNumber(number + 1);
    }

    /// <summary>
    /// Convert a decimal number to its roman numerals representation.
    /// </summary>
    /// <param name="number">A decimal number to convert</param>
    /// <returns>A string representation of the specified number using roman numerals.</returns>
    private string RomanifyNumber(int number) {
        string romanStr = "";
        int i = DECIAML_VALUES.Length - 1;

        while (number > 0) {
            int div = number / DECIAML_VALUES[i];
            number %= DECIAML_VALUES[i];

            while (div-- > 0) romanStr += ROMAN_VALUES[i];
            i--;
        }

        return romanStr;
    }
}