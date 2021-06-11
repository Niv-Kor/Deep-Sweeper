using DeepSweeper.Characters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class SectorManager : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [SerializeField] private RawImage spriteCmp;
        [SerializeField] private SpriteOrientation preferredOrientation;
        [SerializeField] private List<SpriteConfiguration> spritesConfig; ///REMOVE
        [SerializeField] private List<SegmentInstructions> instructions;
        [SerializeField] private List<Image> sectoredImages;
        #endregion

        #region Class Members
        private SectorHighlighter highlighter;
        private SelectionArrow arrow;
        private bool m_selected;
        #endregion

        #region Properties
        public RadialToolkit.Segment Segment { get; private set;}
        public Persona Character { get; set; }
        public bool Selected {
            get => m_selected;
            set {
                if (m_selected != value) {
                    highlighter.Highlight(value);
                    if (value) arrow.Launch();
                    m_selected = value;
                }
            }
        }
        #endregion

        private void Awake() {
            this.highlighter = GetComponent<SectorHighlighter>();
            this.arrow = GetComponentInChildren<SelectionArrow>();
        }

        private bool IsOrientationCompatible(SpriteOrientation orientation) {
            return orientation == preferredOrientation || orientation == SpriteOrientation.Straight;
        }

        private void SetCharacter(SpriteConfiguration config) {
            spriteCmp.texture = config.Sprite;
            spriteCmp.transform.localPosition = config.Offset;

            //horizontal flip the sprite
            spriteCmp.transform.localRotation = Quaternion.identity;

            if (!IsOrientationCompatible(config.Orientation))
                spriteCmp.transform.Rotate(0, 180, 0);
        }

        private bool TryGetInstructions(RadialToolkit.Segment segment, out SegmentInstructions instruction) {
            if (instructions.FindIndex(x => x.Segment == segment) != -1) {
                instruction = instructions.Find(x => x.Segment == segment);
                return true;
            }
            else {
                instruction = default;
                return false;
            }
        }

        public void Build(RadialToolkit.Segment segment) {
            bool available = TryGetInstructions(segment, out SegmentInstructions instructions);
            if (!available) return;

            RadialToolkit.RadialDivision division = RadialToolkit.Originate(segment);
            int divisionValue = division.AsAmount();
            arrow.Set(segment);
            Segment = segment;

            //customize each image confined by the sector
            foreach (Image image in sectoredImages) {
                image.fillOrigin = (int) instructions.FillOrigin;
                image.fillAmount = 1f / divisionValue;
                image.fillClockwise = instructions.Clockwise;
                Vector3 eulerRot = Vector3.forward * instructions.Roll;
                image.rectTransform.localRotation = Quaternion.Euler(eulerRot);
            }
        }
    }
}