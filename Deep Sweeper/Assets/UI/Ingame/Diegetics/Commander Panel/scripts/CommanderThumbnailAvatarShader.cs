using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.Gameplay.UI.Diegetics.Commander
{
    public class CommanderThumbnailAvatarShader : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Color")]
        [Tooltip("The shade to apply over the avatar when it's selected.")]
        [SerializeField] private Color lightShade;

        [Tooltip("The shade to apply over the avatar when it's not selected.")]
        [SerializeField] private Color darkShade;

        [Header("Timing")]
        [SerializeField] private float shadingTime;
        #endregion

        #region Class Members
        private RawImage avatar;
        #endregion

        #region Properties
        public bool IsLight { get; private set; }
        #endregion

        private void Awake() {
            this.avatar = GetComponent<RawImage>();
        }

        /// <summary>
        /// Slowly apply a shade to the avatar.
        /// </summary>
        /// <param name="fadeIn">True to apply the shade or false to remove it</param>
        private IEnumerator ApplyShade(Color shade) {
            Color from = avatar.color;
            float timer = 0;

            while (timer <= shadingTime) {
                timer += Time.deltaTime;
                avatar.color = Color.Lerp(from, shade, timer / shadingTime);
                yield return null;
            }
        }

        /// <summary>
        /// Apply a shade to the avatar.
        /// </summary>
        /// <param name="flag">True to apply a light shade or false to apply a dark shade</param>
        public void Apply(bool light) {
            IsLight = light;
            Color shade = light ? lightShade : darkShade;
            StopAllCoroutines();
            StartCoroutine(ApplyShade(shade));
        }
    }
}