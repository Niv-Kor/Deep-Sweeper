using UnityEngine;

public class Indicator : MonoBehaviour
{
    public delegate void IndicatorReveal(bool instant);
    public event IndicatorReveal IndicatorRevealEvent;

    private IndicatorText indicatorText;

    public int MinedNeighbours {
        get { return indicatorText.Value; }
        set { indicatorText.Value = value; }
    }

    private void Awake() {
        this.indicatorText = GetComponentInChildren<IndicatorText>();
    }

    /// <summary>
    /// Display the indicator.
    /// </summary>
    /// <param name="flag">True to display or false to hide</param>
    /// <param name="instant">True to enable it instantly without special effects</param>
    public void Display(bool flag, bool instant) {
        indicatorText.Enabled = flag;
        if (flag) IndicatorRevealEvent?.Invoke(instant);
    }

    /// <returns>True if the indicator is enabled and displayed.</returns>
    public bool IsDisplayed() { return indicatorText.Enabled; }
}