using UnityEngine;

namespace Menu.Map
{
    public class MapScaler : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Tooltip("An origin scale value that corresponds with the origin resolution.")]
        [SerializeField] private float originScale;

        [Tooltip("The screen resolution for which the map's scale is the origin scale.")]
        [SerializeField] private Vector2 originResolution;
        #endregion

        private void Awake() {
            RectTransform rect = GetComponent<RectTransform>();
            float originMagnitude = originResolution.magnitude;
            Vector2 resVec = new Vector2(Screen.width, Screen.height);
            float resMagnitude = resVec.magnitude;
            float scale = resMagnitude / originMagnitude;
            rect.localScale = Vector3.one * originScale * scale;
        }
    }
}