using TMPro;
using UnityEngine;

public class PhaseNameSpatial : PhaseSpatial<PhaseNameSpatial>
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The text component that consists of the phase's number.")]
    [SerializeField] private TextMeshProUGUI phaseNumberCmp;

    [Tooltip("The text component that consists of the map name.")]
    [SerializeField] private TextMeshProUGUI mapNameCmp;
    #endregion

    protected override void Start() {
        base.Start();
        LevelFlow.Instance.PhaseUpdatedEvent += SetPhase;
    }

    /// <summary>
    /// Change the phase number.
    /// </summary>
    /// <param name="number">The new phase number</param>
    private void SetPhase(PhaseConfig phaseConfig, PhaseDifficultyConfig _, int number) {
        phaseNumberCmp.text = NumericUtils.Romanify(number + 1);
        mapNameCmp.text = phaseConfig.MapName;
    }
}