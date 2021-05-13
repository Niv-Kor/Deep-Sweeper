using DeepSweeper.Level.Mine;
using TMPro;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Minimap
{
    public class IndicationMinimapIcon : MinimapIcon
    {
        protected override void Start() {
            base.Start();

            TextMeshPro textMesh = GetComponent<TextMeshPro>();
            IndicationNumber originIndicator = transform.parent.GetComponentInChildren<IndicationNumber>();
            RectTransform rectTransform = GetComponent<RectTransform>();
            RectTransform originRectTransform = originIndicator.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            textMesh.text = originIndicator.Value.ToString();
            originIndicator.ValueChange += delegate (string val) { textMesh.text = val; };
        }
    }
}