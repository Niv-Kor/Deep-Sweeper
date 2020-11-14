using UnityEngine;
using UnityEngine.UI;

namespace FieldMeta
{
    public class MapTitleMeta : MonoBehaviour
    {
        private void Start() {
            Text textCmp = GetComponentInChildren<Text>();
            textCmp.text = GameFlow.Instance.CurrentPhase.MapName;
        }
    }
}