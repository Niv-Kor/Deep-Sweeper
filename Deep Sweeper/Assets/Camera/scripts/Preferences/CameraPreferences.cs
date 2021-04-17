using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPreferences : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("True to always keep this camera on,\n"
           + "no matter any camera switches that take place during the game")]
    [SerializeField] private bool alwaysOn;
    #endregion

    #region Properties
    public bool AlwaysOn { get => alwaysOn; }
    #endregion

    /// <summary>
    /// Activate when the camera component is activated.
    /// </summary>
    public virtual void OnActivation() {}

    /// <summary>
    /// Activate when the camera component is deactivated.
    /// </summary>
    public virtual void OnDeactivation() {}
}