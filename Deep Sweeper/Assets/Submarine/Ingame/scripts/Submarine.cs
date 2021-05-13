namespace DeepSweeper.Player
{
    public class Submarine : Singleton<Submarine>
    {
        #region Properties
        public SubmarineOrientation Orientation { get; private set; }
        public SubmarineMovementController Controller { get; private set; }
        #endregion

        private void Awake() {
            this.Orientation = GetComponent<SubmarineOrientation>();
            this.Controller = GetComponent<SubmarineMovementController>();
        }
    }
}