using Pixelome;
using System.Collections;
using TMPro;
using UnityEngine;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class RadialText : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Speed")]
        [Tooltip("The angle at which the text spins each frame while "
               + "not hovering the local segment.")]
        [SerializeField] private float fastSpinAngle;

        [Tooltip("The angle at which the text spins each frame while "
               + "hovering the local segment.")]
        [SerializeField] private float slowSpinAngle;

        [Tooltip("The time it takes the text to adapt to a change in its speed.")]
        [SerializeField] private float speedChangeTime;
        #endregion

        #region Constants
        private static readonly float MIN_FACE_INSIDE_ANGLE = 110;
        private static readonly float MAX_FACE_INSIDE_ANGLE = 250;
        #endregion

        #region Class Members
        private RectTransform rect;
        private TextMeshProUGUI textCmp;
        private RadialToolkit.Segment currentSegment;
        private RadialToolkit.RadialDivision division;
        private CircularTextWarp textWarpCmp;
        private float currentRadialSpeed;
        #endregion

        private void Start() {
            this.rect = GetComponent<RectTransform>();
            this.textCmp = GetComponent<TextMeshProUGUI>();
            this.textWarpCmp = GetComponent<CircularTextWarp>();
            this.currentRadialSpeed = fastSpinAngle;

            CommanderSpatial spatial = GetComponentInParent<CommanderSpatial>();
            SectorialDivisor divisor = spatial.GetComponentInChildren<SectorialDivisor>();
            divisor.SectorSelectedEvent += OnSectorSelected;
            spatial.ActivatedEvent += OnSpatialActivated;
        }

        /// <summary>
        /// Activate when a sectors gets temporarily selected.
        /// This method changes the text according to the selected character.
        /// </summary>
        /// <param name="sector">The selected sector</param>
        /// <seealso cref="SectorialDivisor.SectorSelectedEvent"/>
        private void OnSectorSelected(SectorManager sector) {
            textCmp.text = ExtractDataString(sector);

            currentSegment = sector.Segment;
            division = currentSegment.Originate();
        }

        /// <summary>
        /// Construct a string of information regarding a sector.
        /// </summary>
        /// <param name="sector">The sector from which to extract the information</param>
        /// <returns>A string of relevant sector data.</returns>
        private string ExtractDataString(SectorManager sector) {
            return $"{sector.Character}";
        }

        /// <summary>
        /// Activate when the spatial changes its activation state.
        /// </summary>
        /// <param name="flag">True if the spatial activates or false if it deactivates</param>
        private void OnSpatialActivated(bool flag) {
            StopAllCoroutines();
            if (flag) StartCoroutine(Spin());
        }

        /// <summary>
        /// Spin the radial text around the circle.
        /// </summary>
        private IEnumerator Spin() {
            bool inSegmentGlobal = false;

            while (true) {
                float z = rect.localRotation.eulerAngles.z % 360;
                bool inSegment = division.ToSegment(z) == currentSegment;
                textWarpCmp.FacingInside = z >= MIN_FACE_INSIDE_ANGLE && z <= MAX_FACE_INSIDE_ANGLE;

                if (inSegmentGlobal != inSegment) {
                    float angle = inSegment ? slowSpinAngle : fastSpinAngle;
                    StartCoroutine(ChangeRadialSpeed(angle));
                    inSegmentGlobal = inSegment;
                }

                rect.Rotate(0, 0, currentRadialSpeed);

                yield return null;
            }
        }

        /// <summary>
        /// Slowly change the speed of the text's spin.
        /// </summary>
        /// <param name="targetVal">The new value</param>
        private IEnumerator ChangeRadialSpeed(float targetVal) {
            float startVal = currentRadialSpeed;
            float timer = 0;

            while (timer <= speedChangeTime) {
                timer += Time.deltaTime;
                float step = timer / speedChangeTime;
                currentRadialSpeed = Mathf.Lerp(startVal, targetVal, step);

                yield return null;
            }
        }
    }
}