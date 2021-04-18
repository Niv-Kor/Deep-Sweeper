namespace DeepSweeper.Camera
{
    public class GateCamera : DynamicCamera
    {
        /// <inheritdoc/>
        protected override void OnActivation() {
            DiegeticsManager.Instance.Activate(false, 0);
        }

        /// <inheritdoc/>
        protected override void OnDeactivation() {
            DiegeticsManager.Instance.Activate(true, 0);
        }
    }
}