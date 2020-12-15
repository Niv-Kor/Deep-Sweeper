public class GateCamPreferences : CameraPreferences
{
    /// <inheritdoc/>
    public override void OnActivation() {
        DiegeticsManager.Instance.Activate(false, 0);
    }

    /// <inheritdoc/>
    public override void OnDeactivation() {
        DiegeticsManager.Instance.Activate(true, 0);
    }
}