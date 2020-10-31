using TMPro;
using UnityEngine;

public abstract class CounterInfo : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("Left hand side counter.")]
    [SerializeField] protected TextMeshProUGUI leftCounterText;

    [Tooltip("Overall right hand side counter.")]
    [SerializeField] protected TextMeshProUGUI rightCounterText;
    #endregion

    #region Class Members
    protected int m_leftCounter;
    protected int m_rightCounter;
    #endregion

    #region Properties
    protected int LeftCounter {
        get { return m_leftCounter; }
        set {
            m_leftCounter = value;
            UpdateUIComponents();
        }
    }

    protected int RightCounter {
        get { return m_rightCounter; }
        set {
            m_rightCounter = value;
            UpdateUIComponents();
        }
    }
    #endregion

    protected virtual void Awake() {
        AssignCounters();
    }

    /// <summary>
    /// Assign values or triggers to the left and right counters.
    /// </summary>
    protected abstract void AssignCounters();

    /// <summary>
    /// Update the counters' text components.
    /// </summary>
    protected virtual void UpdateUIComponents() {
        leftCounterText.text = LeftCounter.ToString();
        rightCounterText.text = RightCounter.ToString();
    }
}