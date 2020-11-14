using UnityEngine;

namespace FieldMeta
{
    public class FieldGridsMeta : FieldMetaValue
    {
        protected override string GetFieldValue(DifficultyLevel difficulty) {
            Vector2Int size = field.MatrixSize;
            int rows = size.x;
            int cols = size.y;
            return rows + " x " + cols;
        }
    }
}