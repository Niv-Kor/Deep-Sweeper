
using System;
using UnityEngine;

namespace DeepSweeper.CameraSet
{
    [Serializable]
    public struct CameraSetConfig
    {
        [Tooltip("The camera's role and unique ID in the scene.")]
        [SerializeField] public CameraRole Role;

        [Tooltip("A camera rig around which the camera rotates.")]
        [SerializeField] public CameraRig Rig;

        [Tooltip("The camera itself.")]
        [SerializeField] public DynamicCamera Camera;

        [Tooltip("The camera's post processing manager.")]
        [SerializeField] public PostProcessingManager PostProcessing;

        [Tooltip("True to always keep this camera on, no matter the switches.")]
        [SerializeField] public bool AlwaysOn;
    }
}