namespace DeepSweeper.Player
{
    public class Submarine : Singleton<Submarine>
    {
        #region Properties
        public SubmarineOrientation Orientation { get; private set; }
        public MobilityController Controller { get; private set; }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.Orientation = GetComponent<SubmarineOrientation>();
            this.Controller = GetComponent<MobilityController>();
        }
    }
}