using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static LevelsMap.Landmark;

namespace LevelsMap
{
    public class MapTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The image that represents the tile's texture.")]
        [SerializeField] private RawImage textureImage;

        [Header("View")]
        [Tooltip("The sprite to use as the tile's texture.")]
        [SerializeField] private Texture tileSprite;

        [Header("Level")]
        [SerializeField] private int levelIndex = -1;
        #endregion

        #region Class Members
        private Landmark landmark;
        private TileGlow glow;
        #endregion

        private void Start() {
            this.landmark = GetComponentInChildren<Landmark>();
            this.glow = GetComponentInChildren<TileGlow>();

            //set landmark availability
            if (landmark != null) {
                LandmarkIconState landmarkState = LandmarkIconState.None;

                if (levelIndex >= 0) {
                    bool isOpen = GameAdvancement.Instance.IsLevelOpen(levelIndex);
                    landmarkState = isOpen ? LandmarkIconState.Available : LandmarkIconState.Unavailable;
                }

                landmark.Availability = landmarkState;
            }
        }

        private void OnValidate() {
            textureImage.texture = tileSprite;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            glow.Enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            glow.Enabled = false;
        }
    }
}