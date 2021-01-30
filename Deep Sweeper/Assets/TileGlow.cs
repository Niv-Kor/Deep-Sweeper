using UnityEngine;
using UnityEngine.UI;

namespace LevelsMap
{
    public class TileGlow : MonoBehaviour
    {
        #region Class Members
        private RawImage image;
        #endregion

        #region Properties
        public bool Enabled {
            get { return image.enabled; }
            set { image.enabled = value; }
        }
        #endregion

        private void Awake() {
            this.image = GetComponent<RawImage>();
            this.Enabled = false;
        }
    }
}