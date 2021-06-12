using DeepSweeper.Characters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class SpriteEmbedder : MonoBehaviour, IDynamicSectorComponent
    {
        #region Exposed Editor Parameters
        [Header("Settings")]
        [Tooltip("A list of sprite configurations for each selectable character.")]
        [SerializeField] private List<SpriteConfiguration> spritesConfig;

        [Tooltip("The distance of the character's sprite from the center of the circle, "
               + "as a percentage of the circle's radius.")]
        [SerializeField] [Range(0f, 1f)] private float distFromCenter;
        #endregion

        #region Class Members
        private RectTransform maskRect;
        private RectTransform rect;
        private RawImage spriteCmp;
        #endregion

        #region Events
        /// <param type=typeof(RectTransform)>The embedded sprite's transform</param>
        public event UnityAction<RectTransform> SpriteEmbeddedEvent;
        #endregion

        private void Awake() {
            this.maskRect = transform.parent.GetComponent<RectTransform>();
            this.rect = GetComponent<RectTransform>();
            this.spriteCmp = GetComponent<RawImage>();
        }

        /// <inheritdoc/>
        public void Build(SegmentInstructions instructions, Persona character = Persona.None) {
            Vector2 pixelsDist = maskRect.sizeDelta / 2 * distFromCenter;
            Vector2 basePose = Vector2.Scale(instructions.Segment.AsCoordinates(), pixelsDist);
            Vector3 BaseRot = (basePose.x >= 0) ? Vector3.zero : Vector3.up * 180;
            BaseRot -= Vector3.forward * instructions.Roll;

            rect.anchoredPosition = basePose + instructions.SpriteOffset;
            rect.localRotation = Quaternion.Euler(BaseRot);
            rect.localScale *= instructions.SpriteScale;

            //set sprite
            SpriteConfiguration spriteConfig = spritesConfig.Find(x => x.Character == character);

            if (spriteConfig.Character != Persona.None) {
                spriteCmp.texture = spriteConfig.Sprite;
                rect.anchoredPosition += spriteConfig.Offset;
                SpriteEmbeddedEvent?.Invoke(rect);
            }
        }
    }
}