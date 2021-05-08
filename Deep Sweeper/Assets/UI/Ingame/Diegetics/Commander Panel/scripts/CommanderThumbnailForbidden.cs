using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.Gameplay.UI.Diegetics.Commander
{
    public class CommanderThumbnailForbidden : MonoBehaviour {
        #region Exposed Editor Parameters
        [Header("Animation")]
        [Tooltip("The scale from which the forbidden sign start animation.")]
        [SerializeField] private float startScale = .5f;

        [Tooltip("The time it takes to complete the forbidden sign animation (in seconds).")]
        [SerializeField] private float animationTime = 1;

        [Header("Settings")]
        [Tooltip("The percentage of animation time after which is can be invoked once more.")]
        [SerializeField] [Range(0f, 1f)] private float activationCooldown = .5f;
        #endregion

        #region Constants
        private static readonly string FORBIDDEN_SFX = "forbid";
        private static readonly Color OPAQUE_COLOR = Color.white;
        private static readonly Color TRANSPARENT = new Color(0xff, 0xff, 0xff, 0);
        #endregion

        #region Class Members
        private RawImage image;
        private Jukebox jukebox;
        private bool onCooldown;
        #endregion

        private void Awake() {
            this.image = GetComponent<RawImage>();
            this.jukebox = GetComponent<Jukebox>();
            this.onCooldown = false;
        }

        /// <summary>
        /// Animate the forbidden sign.
        /// </summary>
        private IEnumerator Animate() {
            float timer = 0;
            Vector3 startVector = Vector3.one * startScale;
            image.rectTransform.localScale = startVector;
            image.color = OPAQUE_COLOR;

            while (timer <= animationTime) {
                timer += Time.deltaTime;

                //release cooldown
                if (onCooldown && timer >= activationCooldown * animationTime) onCooldown = false;

                float step = timer / animationTime;
                image.rectTransform.localScale = Vector3.Lerp(startVector, Vector3.one, step);
                image.color = Color.Lerp(OPAQUE_COLOR, TRANSPARENT, step);
                yield return null;
            }
        }

        /// <summary>
        /// Activate the forbidden sign animation.
        /// </summary>
        public void Activate() {
            if (onCooldown) return;
            
            if (activationCooldown > 0) onCooldown = true;
            jukebox.Play(FORBIDDEN_SFX);
            StopAllCoroutines();
            StartCoroutine(Animate());
        }
    }
}