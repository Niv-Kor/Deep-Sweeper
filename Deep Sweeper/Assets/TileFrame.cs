using UnityEngine;
using UnityEngine.UI;

namespace LevelsMap
{
    public class TileFrame : TileAttribute
    {
        #region Exposed Editor Parameters
        [Tooltip("The frame's active color.")]
        [SerializeField] private Color color;
        #endregion

        #region Constants
        private static readonly Color TRANSPARENT = new Color(0x0, 0x0, 0x0, 0x0);
        #endregion

        #region Class Members
        private RawImage image;
        #endregion

        #region Properties
        protected override TileAttributeState DefaultState => TileAttributeState.Off;
        #endregion

        private void Awake() {
            this.image = GetComponent<RawImage>();
        }

        protected override void SetState(TileAttributeState state) {
            image.color = (state == TileAttributeState.On) ? color : TRANSPARENT;
        }
    }
}