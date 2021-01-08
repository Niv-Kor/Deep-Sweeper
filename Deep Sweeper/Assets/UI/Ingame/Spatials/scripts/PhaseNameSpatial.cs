﻿using TMPro;
using UnityEngine;

public class PhaseNameSpatial : Spatial<PhaseNameSpatial>
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
        GameFlow.Instance.PhaseUpdatedEvent += SetPhase;
    }

    /// <summary>
    /// Change the phase number.
    /// </summary>
    /// <param name="number">The new phase number</param>
    private void SetPhase(PhaseConfig phaseConfig, PhaseDifficultyConfig _, int number) {
        phaseNumberCmp.text = NumericUtils.Romanify(number + 1);
        mapNameCmp.text = phaseConfig.MapName;
    }

    /// <summary>
    /// Display or hide the phase title.
    /// </summary>
    /// <param name="flag">True to display or false to hide</param>
    public void Display(bool flag) { Enabled = flag; }
}