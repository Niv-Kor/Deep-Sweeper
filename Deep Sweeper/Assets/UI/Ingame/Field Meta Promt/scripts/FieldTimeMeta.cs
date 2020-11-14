namespace FieldMeta
{
    public class FieldTimeMeta : FieldMetaValue
    {
        protected override string GetFieldValue(DifficultyLevel difficulty) {
            return GetCurrentConfig(difficulty).Clock.ToString();
        }
    }
}