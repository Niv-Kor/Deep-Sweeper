using DeepSweeper.Player;
using System.Collections;
using UnityEngine;

namespace DeepSweeper.Level.Mine
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MineMark : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Settings")]
        [Tooltip("The target alpha value of a mark.")]
        [SerializeField] [Range(0f, 1f)] private float alphaValue = 1;
        #endregion

        #region Class Members
        private SpriteRenderer render;
        private Transform parentTransform;
        private Submarine player;
        #endregion

        #region Properties
        public bool IsDisplayed { get; private set; }
        #endregion

        private void Start() {
            this.render = GetComponent<SpriteRenderer>();
            this.player = Submarine.Instance;
            this.parentTransform = transform.parent;
            this.IsDisplayed = false;
        }

        private void Update() {
            //rotate the mark towards the player
            if (IsDisplayed) {
                Vector3 playerDir = Vector3.Normalize(parentTransform.position - player.transform.position);
                Quaternion playerRot = Quaternion.LookRotation(playerDir);
                Vector3 playerRotVect = Vector3.Scale(playerRot.eulerAngles, Vector3.up);
                parentTransform.rotation = Quaternion.Euler(playerRotVect);
            }
        }

        /// <summary>
        /// Display or hide the mark.
        /// </summary>
        /// <param name="flag">True to display or false to hide</param>
        /// <param name="time">The time it takes the mark to be displayed or hidden</param>
        /// <param name="sprite">The mark's sprite</param>
        public void Display(bool flag, float time, Sprite sprite = null) {
            if (flag != IsDisplayed) {
                IsDisplayed = flag;

                //change renderer sprite
                if (sprite != null) render.sprite = sprite;

                //lerp
                StopAllCoroutines();
                StartCoroutine(Appear(flag, time));
            }
        }

        /// <summary>
        /// Make the mark appear or disappear
        /// </summary>
        /// <param name="flag">True to appear or false to disappear</param>
        /// <param name="time">The time it takes the mark to appear or disappear</param>
        private IEnumerator Appear(bool flag, float time) {
            Color baseColor = render.color;
            float startAlpha = baseColor.a;
            float targetAlpha = flag ? alphaValue : 0;
            float lerpedTime = 0;

            while (lerpedTime <= time) {
                lerpedTime += Time.deltaTime;
                float nextAlpha = Mathf.Lerp(startAlpha, targetAlpha, lerpedTime / time);
                Color nextColor = new Color(baseColor.r, baseColor.g, baseColor.b, nextAlpha);
                render.color = nextColor;
                yield return null;
            }
        }
    }
}