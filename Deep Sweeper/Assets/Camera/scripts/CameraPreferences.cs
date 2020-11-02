﻿using UnityEngine;

public class CameraPreferences : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("True to always keep this camera on,\n"
           + "no matter any camera switches that take place during the game")]
    [SerializeField] private bool alwaysOn;
    #endregion

    #region Properties
    public bool AlwaysOn { get { return alwaysOn; } }
    #endregion
}