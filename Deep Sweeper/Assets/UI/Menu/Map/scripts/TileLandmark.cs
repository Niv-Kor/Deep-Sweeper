using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.Menu.Map
{
    public class TileLandmark : TileAttribute
    {
        #region Exposed Editor Parameters
        [Tooltip("The icon to use as an available landmark.")]
        [SerializeField] private Texture availableIcon;

        [Tooltip("The icon to use as an unavailable landmark.")]
        [SerializeField] private Texture unavailableIcon;
        #endregion

        #region Class Members
        private RawImage image;
        #endregion

        #region Properties
        protected override TileAttributeState DefaultState {
            get => TileAttributeState.Unavailable;
        }
        #endregion

        private void Awake() {
            this.image = GetComponent<RawImage>();
        }

        /// <param name="flag">True to display the icon or false to hide it</param>
        private void DisplayIcon(bool flag) {
            if (flag) image.color = Color.white;
            else {
                image.texture = null;
                image.color = new Color(0x0, 0x0, 0x0, 0x0);
            }
        }

        /// <inheritdoc/>
        protected override void SetState(TileAttributeState state) {
            if (state == TileAttributeState.Unavailable) DisplayIcon(false);
            else {
                DisplayIcon(true);
                switch (state) {
                    case TileAttributeState.On: image.texture = availableIcon; break;
                    case TileAttributeState.Off: image.texture = unavailableIcon; break;
                }
            }
        }
    }
}