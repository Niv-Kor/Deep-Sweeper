public class Submarine : Singleton<Submarine> {
    #region Class Members
    private SubmarineOrientation m_orientation;
    #endregion

    #region Properties
    public SubmarineOrientation Oriantation => m_orientation;
    #endregion

    private void Awake() {
        this.m_orientation = GetComponent<SubmarineOrientation>();
    }
}