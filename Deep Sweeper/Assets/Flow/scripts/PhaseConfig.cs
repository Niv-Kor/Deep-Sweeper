using System;
using UnityEngine;

[Serializable]
public struct PhaseConfig
{
    [Tooltip("The phase's boundaries.")]
    [SerializeField] public Confine Confine;

    [Tooltip("Percentage of the mines within the mine fields.")]
    [SerializeField] public int MinesPercent;

    [Tooltip("The exit gate from this phase to the next one.")]
    [SerializeField] public Gate Gate;

    [Tooltip("The amount of time the player gets to finish the phase.")]
    [SerializeField] public int TimerSeconds;
}