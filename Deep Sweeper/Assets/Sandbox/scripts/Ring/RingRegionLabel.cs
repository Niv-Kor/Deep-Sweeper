using TMPro;
using UnityEngine;

namespace DeepSweeper.Menu.UI.Campaign.Sandbox.Ring
{
    public class RingRegionLabel : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The marker component of this ring.")]
        [SerializeField] private RingMarker marker;

        [Header("Settings")]
        [Tooltip("True to display this label if the ring marker is enabled.")]
        [SerializeField] private bool displayWithMarker;
        #endregion

        #region Class Members
        private TextMeshProUGUI text;
        #endregion

        #region Properties
        public string Text {
            get => text.text;
            set { text.text = value; }
        }
        #endregion

        private void Awake() {
            this.text = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start() {
            OnMarkerDisplayEvent(marker.Displayed);
            marker.MarkerDisplayedEvent += OnMarkerDisplayEvent;
        }

        /// <summary>
        /// Activate when the marker's state is changed.
        /// This function displays or hides the label accordingly.
        /// </summary>
        /// <param name="displayed">True if the marker is displayed or false if it's hidden</param>
        private void OnMarkerDisplayEvent(bool displayed) {
            text.enabled = displayWithMarker == displayed;
        }
    }
}