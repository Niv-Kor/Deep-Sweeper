using DeepSweeper.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.Gameplay.UI.Diegetics.Commander
{
    public class CommanderThumbnail : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The commander's avatar image component.")]
        [SerializeField] private RawImage avatarCmp;
        #endregion

        #region Class Members
        private CommanderThumbnailGlow glow;
        private CommanderThumbnailCooldown cooldownTimer;
        private CommanderThumbnailAvatarShader avatarShader;
        private CommanderThumbnailForbidden forbiddenEffect;
        private int specialCooldown;
        #endregion

        #region Properties
        public CharacterPersona Character { get; private set; }
        public bool IsSelected { get; private set; }
        public int DefaultCooldown { get; private set; }
        public bool OnCooldown => cooldownTimer.CooldownActive;
        #endregion

        private void Awake() {
            this.glow = GetComponentInChildren<CommanderThumbnailGlow>();
            this.cooldownTimer = GetComponentInChildren<CommanderThumbnailCooldown>();
            this.avatarShader = GetComponentInChildren<CommanderThumbnailAvatarShader>();
            this.forbiddenEffect = GetComponentInChildren<CommanderThumbnailForbidden>();
            this.specialCooldown = -1;
        }

        private void Start() {
            //brighten the avatar as soon as the cooldown is over
            cooldownTimer.CooldownOverEvent += delegate { avatarShader.Apply(true); };
        }

        /// <summary>
        /// Set a commander for this thumbnail.
        /// </summary>
        /// <param name="config">The configuration parameters</param>
        /// <param name="defaultCooldown">The default cooldown time to apply to this commander</param>
        public void SetCommander(CommanderThumbnailConfig config, int defaultCooldown) {
            Character = config.Character;
            avatarCmp.texture = config.Sprite;
            avatarCmp.color = Color.white;
            glow.Color = config.Theme;
            DefaultCooldown = defaultCooldown;
        }

        /// <summary>
        /// Select or deselect the commander.
        /// </summary>
        /// <param name="flag">True to select or false to deselect</param>
        /// <param name="force">
        /// True to force the thumbnail into selection, even if it's already selected.
        /// This flag will also lerp the selection colors immidiately without an animation.
        /// </param>
        public void Select(bool flag, bool force = false) {
            if (flag == IsSelected && !force) return;

            IsSelected = flag;
            glow.Apply(flag);

            //apply cooldown
            if (!flag) {
                int cooldown = (specialCooldown == -1) ? DefaultCooldown : specialCooldown;

                if (cooldown >= 0) {
                    if (cooldown > 0) {
                        cooldownTimer.Set(cooldown);
                        avatarShader.Apply(false);
                    }

                    CancelNextCooldownTime();
                }
            }
        }

        /// <summary>
        /// Manually set the next cooldown time.
        /// This special cooldown time will only be applied once.
        /// </summary>
        /// <param name="cooldown"></param>
        public void SetNextCooldownTime(int cooldown) {
            if (cooldown >= 0) specialCooldown = cooldown;
        }

        /// <summary>
        /// Cancel the special set cooldown time.
        /// </summary>
        public void CancelNextCooldownTime() {
            specialCooldown = -1;
        }

        /// <summary>
        /// Show and animate a forbidden sign.
        /// </summary>
        public void ShowForbiddenSign() {
            forbiddenEffect.Activate();
        }

        /// <summary>
        /// Reset the commander's current cooldown.
        /// </summary>
        public void ResetCooldown() {
            cooldownTimer.Stop();
        }
    }
}