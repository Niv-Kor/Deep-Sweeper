using UnityEngine;
using UnityEngine.UI;

namespace FieldMeta
{
    public class RegionTitleMeta : MonoBehaviour
    {
        private void Start() {
            Text textCmp = GetComponentInChildren<Text>();
            textCmp.text = GameFlow.Instance.RegionName;
        }
    }
}