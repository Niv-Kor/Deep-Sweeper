namespace FieldMeta
{
    public class FieldMinesMeta : FieldMetaValue
    {
        protected override string GetFieldValue(DifficultyLevel difficulty) {
            return field.MinesAmount.ToString();
        }
    }
}