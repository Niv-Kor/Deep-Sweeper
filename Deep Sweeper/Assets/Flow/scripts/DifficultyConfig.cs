using System;
using UnityEngine;

[Serializable]
public struct DifficultyConfig
{
    [Tooltip("The difficulty level of this configuration.")]
    [SerializeField] public DifficultyLevel Difficulty;

    [Tooltip("Percentage of the mines within the mine field.")]
    [SerializeField] public float MinesPercent;

    [Tooltip("Amount of money guranteed upon completing the phase.")]
    [SerializeField] public long PhaseReward;

    [Tooltip("The maximum time available to complete the phase.")]
    [SerializeField] public int Clock;
}