namespace FieldMeta
{
    public class FieldRewardMeta : FieldMetaValue
    {
        protected override string GetFieldValue(DifficultyLevel difficulty) {
            return GetCurrentConfig(difficulty).PhaseReward.ToString();
        }
    }
}