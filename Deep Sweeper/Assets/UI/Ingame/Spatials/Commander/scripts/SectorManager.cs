using DeepSweeper.Characters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class SectorManager : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The character's sprite mask.")]
        [SerializeField] private RectTransform spriteMask;

        [Tooltip("A list of all images that should be bounded by the segment's shape.")]
        [SerializeField] private List<Image> sectoredImages;

        [Header("Settings")]
        [Tooltip("A list of instructions to build each predefined segment.")]
        [SerializeField] private List<SegmentInstructions> instructions;
        #endregion

        #region Class Members
        private SectorHighlighter highlighter;
        private SpriteEmbedder spriteEmbedder;
        private SelectionArrow arrow;
        private bool m_selected;
        #endregion

        #region Properties
        public Persona Character { get; set; }
        public RadialToolkit.Segment Segment { get; private set;}
        public CooldownCurtain Cooldown { get; private set; }
        public bool IsAvailable => !IsDead && !Cooldown.Running;
        public bool IsDead { get; private set; }
        public bool Selected {
            get => m_selected;
            set {
                if (m_selected == value || !IsAvailable) return;

                highlighter.Highlight(value);
                if (value) arrow.Launch();
                m_selected = value;
            }
        }
        #endregion

        private void Awake() {
            this.highlighter = GetComponent<SectorHighlighter>();
            this.arrow = GetComponentInChildren<SelectionArrow>();
            this.spriteEmbedder = GetComponentInChildren<SpriteEmbedder>();
            this.Cooldown = GetComponentInChildren<CooldownCurtain>();
            this.IsDead = false;
        }

        /// <summary>
        /// Get the instructions to build a specific segment.
        /// </summary>
        /// <param name="segment">The segment needed to be built</param>
        /// <param name="instruction">The output instructions struct</param>
        /// <returns>True if the instructions for the specified segment were found.</returns>
        private bool TryGetInstructions(RadialToolkit.Segment segment, out SegmentInstructions instruction) {
            bool exists = instructions.FindIndex(x => x.Segment == segment) != -1;
            instruction = exists ? instructions.Find(x => x.Segment == segment) : default;
            return exists;
        }

        /// <summary>
        /// Build this sector according to a given predefined segment type.
        /// </summary>
        /// <param name="segment">The segment according which to build this sector</param>
        /// <param name="character">The character to embed the sprite of which in this sector</param>
        public void Build(RadialToolkit.Segment segment, Persona character) {
            bool available = TryGetInstructions(segment, out SegmentInstructions instructions);
            if (!available) return;

            RadialToolkit.RadialDivision division = RadialToolkit.Originate(segment);
            int divisionValue = division.AsAmount();
            arrow.Build(instructions, character);
            Segment = segment;

            //customize each image confined by the sector
            foreach (Image image in sectoredImages) {
                image.fillOrigin = (int) instructions.FillOrigin;
                image.fillAmount = 1f / divisionValue;
                image.fillClockwise = instructions.Clockwise;
                Vector3 eulerRot = Vector3.forward * instructions.Roll;
                image.rectTransform.localRotation = Quaternion.Euler(eulerRot);
            }

            //set character sprite
            spriteMask.sizeDelta *= instructions.SpriteMaskRate;
            spriteEmbedder.Build(instructions, character);
            Cooldown.Build(instructions, character);
        }

        /// <summary>
        /// Kill the character in this sector.
        /// This sector will no longer be available for selection.
        /// </summary>
        /// <returns>True if the commander can die successfully.</returns>
        public bool Kill() {
            if (!IsDead) {
                IsDead = true;
                highlighter.Kill();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resurrect the character in this sector.
        /// This sector will once again be available for selection.
        /// </summary>
        /// <returns>True if the commander can ressurect successfully.</returns>
        public bool Resurrect() {
            if (IsDead) {
                IsDead = false;
                highlighter.Resurrect();
                return true;
            }

            return false;
        }
    }
}