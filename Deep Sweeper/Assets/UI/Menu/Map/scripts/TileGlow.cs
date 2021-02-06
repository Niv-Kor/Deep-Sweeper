using UnityEngine.UI;

namespace LevelsMap
{
    public class TileGlow : TileAttribute
    {
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

        /// <inheritdoc/>
        protected override void SetState(TileAttributeState state) {
            image.enabled = state == TileAttributeState.On;
        }
    }
}