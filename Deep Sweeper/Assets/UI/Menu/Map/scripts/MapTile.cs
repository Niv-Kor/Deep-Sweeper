using DeepSweeper.Flow;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeepSweeper.Menu.Map
{
    public class MapTile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The image that represents the tile's texture.")]
        [SerializeField] private RawImage textureImage;

        [Header("View")]
        [Tooltip("The sprite to use as the tile's texture.")]
        [SerializeField] private Texture tileSprite;

        [Header("Level")]
        [Tooltip("The index of the tile's level (-1 if it represents no level).")]
        [SerializeField] private int levelIndex = -1;

        [Tooltip("The region that this tile represents.")]
        [SerializeField] private Region region = Region.None;
        #endregion

        #region Class Members
        private CampaignMap map;
        private TileHighlighter highlighter;
        private TileLandmark landmark;
        private TileFrame frame;
        private TileGlow glow;
        private bool objectiveLevel;
        #endregion

        #region Properties
        public Region Region => region;
        #endregion

        private void Awake() {
            this.map = GetComponentInParent<CampaignMap>();
        }

        private void Start() {
            this.highlighter = GetComponentInChildren<TileHighlighter>();
            this.landmark = GetComponentInChildren<TileLandmark>();
            this.frame = GetComponentInChildren<TileFrame>();
            this.glow = GetComponentInChildren<TileGlow>();
            this.objectiveLevel = GameAdvancement.Instance.IsObjectiveLevel(levelIndex);

            //set landmark state
            if (landmark != null) {
                TileAttributeState landmarkState = TileAttributeState.Unavailable;

                if (levelIndex >= 0) {
                    glow.State = TileAttributeState.Off;
                    bool isOpen = GameAdvancement.Instance.IsLevelOpen(levelIndex);
                    landmarkState = isOpen ? TileAttributeState.On : TileAttributeState.Off;

                    if (objectiveLevel) {
                        highlighter.State = TileAttributeState.On;
                        frame.State = TileAttributeState.On;
                    }
                }

                landmark.State = landmarkState;
            }
        }

        private void OnValidate() {
            textureImage.texture = tileSprite;
        }


        /// <inheritdoc/>
        public void OnPointerEnter(PointerEventData ev) {
            if (glow.State != TileAttributeState.Unavailable) {
                if (objectiveLevel) frame.State = TileAttributeState.Off;
                glow.State = TileAttributeState.On;
            }
        }

        /// <inheritdoc/>
        public void OnPointerExit(PointerEventData ev) {
            if (glow.State != TileAttributeState.Unavailable) {
                if (objectiveLevel) frame.State = TileAttributeState.On;
                glow.State = TileAttributeState.Off;
            }
        }

        /// <inheritdoc/>
        public void OnPointerClick(PointerEventData eventData) {
            map.ReportTileClicked(this);
        }
    }
}