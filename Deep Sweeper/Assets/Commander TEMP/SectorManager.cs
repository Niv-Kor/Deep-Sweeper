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
        [SerializeField] private List<SpriteConfiguration> spritesConfig;
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
                if (m_selected != value) {
                    m_selected = value;
                    puppeteer.Manipulate(HOVERED_PARAM, value);
                    if (value) puppeteer.Manipulate(ARROW_PARAM);
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
    }
}