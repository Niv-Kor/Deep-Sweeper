namespace FieldMeta
{
    public class FieldMinesMeta : FieldMetaValue
    {
        protected override string GetFieldValue(DifficultyLevel difficulty) {
            float percent = GetCurrentConfig(difficulty).MinesPercent;
            int total = field.MatrixSize.x * field.MatrixSize.y;
            int amount = (int) (percent * total / 100);
            return amount.ToString();
        }
    }
}