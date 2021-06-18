using System;
using UnityEngine;

namespace DeepSweeper.CameraSet.PostProcessing
{
    [Serializable]
    public struct EffectInstructions
    {
        [Tooltip("The post processing filter.")]
        [SerializeField] public Filter Filter;

        [Tooltip("The exact name of the filter's effect.")]
        [SerializeField] public string Effect;

        [Tooltip("The target value of the filter.")]
        [SerializeField] public float Value;

        [Tooltip("The time it takes the value to change [s].")]
        [SerializeField] public float LerpTime;
    }
}