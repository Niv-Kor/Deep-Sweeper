using System;
using UnityEngine;

[Serializable]
public struct PhaseConfig
{
    [Tooltip("The phase's boundaries.")]
    [SerializeField] public Confine Confine;

    [Tooltip("Percentage of the mines within the mine fields.")]
    public int MinesPercent;

    [Tooltip("The exit gate from this phase to the next one.")]
    public Gate Gate;
}