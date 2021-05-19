using DeepSweeper.Characters;
using System;
using UnityEngine;

namespace DeepSweeper.Player
{
    [Serializable]
    public struct MobilityConfig : IAbility
    {
        [Tooltip("The speed of the submarine when moving on the plane.")]
        [SerializeField] public float HorizontalSpeed;

        [Tooltip("The speed of the submarine when moving upwards or downwards.")]
        [SerializeField] public float VerticalSpeed;

        [Tooltip("The speed multiplier of the turbo key.")]
        [SerializeField] public float TurboMultiplier;

        [Tooltip("The speed multiplier of the dash mechanics.")]
        [SerializeField] public float DashMultiplier;

        [Tooltip("The Z angle of the submarine when moving sideways.")]
        [SerializeField] public float RollAngle;

        [Tooltip("The X angle of the submarine when moving forwards.")]
        [SerializeField] public float PitchAngle;
    }
}