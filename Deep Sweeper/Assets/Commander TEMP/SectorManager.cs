using DeepSweeper.Characters;
using GamedevUtil;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    [RequireComponent(typeof(Animator))]
    public class SectorManager : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [SerializeField] private RawImage spriteCmp;
        [SerializeField] private SpriteOrientation preferredOrientation;
        [SerializeField] private List<SpriteConfiguration> spritesConfig; ///REMOVE
        [SerializeField] private List<SegmentInstructions> instructions;
        [SerializeField] private List<Image> sectoredImages;
        #endregion

        #region Constants
        private static readonly string HOVERED_PARAM = "hovered";
        private static readonly string ARROW_PARAM = "arrow";
        #endregion

        #region Class Members
        private Puppeteer puppeteer;
        private CharacterPersona m_character;
        private bool m_selected;
        #endregion

        #region Properties
        public RadialToolkit.Segment Segment { get; private set;}
        public CharacterPersona Character {
            get => m_character;
            set {
                if (m_character == value) return;

                SpriteConfiguration config = spritesConfig.Find(x => x.Character == value);

                if (value == config.Character) {
                    m_character = value;
                    SetCharacter(config);
                }
            }
        }

        public bool Selected {
            get => m_selected;
            set {
                if (m_selected == value) return;

                m_selected = value;
                puppeteer.Manipulate(HOVERED_PARAM, value);

                if (value) {
                    print(gameObject + " arrow");
                    puppeteer.Manipulate(ARROW_PARAM);
                }
            }
        }
        #endregion

        private void Awake() {
            Animator animator = GetComponent<Animator>();
            this.puppeteer = new Puppeteer(animator);
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