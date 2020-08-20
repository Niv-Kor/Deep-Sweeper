using System;
using UnityEngine;

[Serializable]
public struct Confine
{
    [Tooltip("The confines' offset point.")]
    [SerializeField] public Vector3 Offset;

    [Tooltip("The confine's 3D boundaries.")]
    [SerializeField] public Vector3 Size;
}