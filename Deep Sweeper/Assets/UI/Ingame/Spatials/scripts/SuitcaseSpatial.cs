using System.Collections;
using TMPro;
using UnityEngine;

public class SuitcaseSpatial// : Spatial<SuitcaseSpatial>
{
    /*#region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The textual payment component.")]
    [SerializeField] private TextMeshProUGUI textCmp;

    [Header("Settings")]
    [Tooltip("Amount of digits to display.")]
    [SerializeField] private int displayedDigits = 5;
    #endregion

    #region Constants
    private static readonly float MAX_SLOW_INCREMENT_PERCENT = 2f;
    #endregion

    #region Class Members
    private long m_total;
    private bool initialized;
    #endregion

    #region Properties
    public long MaxValue {
        get {
            long roundVal = (long) Mathf.Pow(10, displayedDigits);
            return roundVal - 1;
        }
    }

    private long CurrentTotal {
        get {
            if (textCmp != null) {
                string cleanText = textCmp.text.Replace(",", "");
                return long.Parse(cleanText);
            }
            else return 0;
        }
    }

    public long Total {
        get { return m_total; }
        private set { m_total = (long) Mathf.Min(value, MaxValue); }
    }
    #endregion

    private void Awake() {
        this.initialized = false;
    }

    protected override void Start() {
        Collect(0); //init counter
    }

    /// <summary>
    /// Collect money into the suitcase.
    /// </summary>
    /// <param name="value">The base value of the tip</param>
    public void Collect(long amount) {
        bool overflow = Total + amount > MaxValue;
        amount = overflow ? MaxValue - Total : amount;
        Total += amount;
        StartCoroutine(Add(amount));
    }

    /// <summary>
    /// Add a sum of money to the coins counter.
    /// </summary>
    /// <param name="tempVal">The total amount of coins to add</param>
    private IEnumerator Add(long val) {
        long remainValue = val;

        while (remainValue > 0 || !initialized) {
            //calculate increment ratio
            int valDigits = NumericUtils.CountDigits(remainValue);
            long decimalScale = (int) Mathf.Pow(10, valDigits - 1);
            bool slowDown = remainValue <= decimalScale * MAX_SLOW_INCREMENT_PERCENT;

            //calculate next portion
            long additiveStep;

            if (!initialized) additiveStep = 0;
            else {
                additiveStep = decimalScale;
                additiveStep /= slowDown ? 10 : 1;
                if (additiveStep == 0) additiveStep = 1;
                remainValue = (long) Mathf.Max(remainValue - additiveStep, 0);
            }

            //edit textual value
            long nextTotal = CurrentTotal + additiveStep;
            int nextTotalDigits = NumericUtils.CountDigits(nextTotal);
            string formatTotal = nextTotal.ToString("N0");
            for (int i = 0; i < displayedDigits - nextTotalDigits; i++)
                formatTotal = "0" + formatTotal;

            //update text
            SetText(textCmp, formatTotal, false);
            initialized = true;
            yield return null;
        }
    }*/
}