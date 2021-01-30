using UnityEngine;
using UnityEngine.UI;

namespace LevelsMap
{
    public class Landmark : MonoBehaviour
    {
        public enum LandmarkIconState
        {
            None,
            Available,
            Unavailable
        }

        #region Exposed Editor Parameters
        [Tooltip("The icon to use as an available landmark.")]
        [SerializeField] private Texture availableIcon;

        [Tooltip("The icon to use as an unavailable landmark.")]
        [SerializeField] private Texture unavailableIcon;
        #endregion

        #region Class Members
        private RawImage image;
        private LandmarkIconState m_available;
        #endregion

        #region Properties
        public LandmarkIconState Availability {
            get { return m_available; }
            set {
                if (m_available != value) {
                    m_available = value;
                    SetIcon(value);
                }
            }
        }
        #endregion

        private void Awake() {
            this.image = GetComponent<RawImage>();
            this.Availability = LandmarkIconState.None;
        }

        private void Start() {
            SetIcon(Availability);
        }

        /// <param name="flag">True to display the icon or false to hide it</param>
        private void DisplayIcon(bool flag) {
            if (flag) image.color = Color.white;
            else {
                image.texture = null;
                image.color = new Color(0x0, 0x0, 0x0, 0x0);
            }
        }

        /// <summary>
        /// Change the landmark's icon relative to its state.
        /// </summary>
        /// <param name="state">The availability state of the landmark icon</param>
        private void SetIcon(LandmarkIconState state) {
            if (state == LandmarkIconState.None) DisplayIcon(false);
            else {
                DisplayIcon(true);
                switch (state) {
                    case LandmarkIconState.Available: image.texture = availableIcon; break;
                    case LandmarkIconState.Unavailable: image.texture = unavailableIcon; break;
                }
            }
        }
    }
}