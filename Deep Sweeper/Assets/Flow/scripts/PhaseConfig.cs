using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PhaseConfig
{
    [Tooltip("The unique name of this map within the region.")]
    [SerializeField] public string MapName;

    [Tooltip("The phase's boundaries.")]
    [SerializeField] public Confine Confine;

    [Tooltip("The exit gate from this phase to the next one.")]
    [SerializeField] public Gate Gate;

    [Tooltip("A list of configurations for each difficulty level.")]
    [SerializeField] public List<PhaseDifficultyConfig> Levels;
}