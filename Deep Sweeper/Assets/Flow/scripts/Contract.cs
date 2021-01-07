public class Contract : Singleton<Contract>
{
    #region Constants
    private static readonly long MIN_PAYMENT = 5500;
    #endregion

    #region Properties
    public long BasePayment { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    #endregion

    private void Awake() {
        this.BasePayment = MIN_PAYMENT;
        this.Difficulty = DifficultyLevel.Easy;
    }
}