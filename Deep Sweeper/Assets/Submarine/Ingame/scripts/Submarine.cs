public class Submarine : Singleton<Submarine>
{
    #region Properties
    public SubmarineOrientation Oriantation { get; private set; }
    public SubmarineMovementController Controller { get; private set; }
    #endregion

    private void Awake() {
        this.Oriantation = GetComponent<SubmarineOrientation>();
        this.Controller = GetComponent<SubmarineMovementController>();
    }
}