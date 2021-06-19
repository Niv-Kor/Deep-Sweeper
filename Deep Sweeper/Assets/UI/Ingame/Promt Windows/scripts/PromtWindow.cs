using DeepSweeper.CameraSet;
using DeepSweeper.Player;
using DeepSweeper.Player.ShootingSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Promt
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PromtWindow : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Timing")]
        [Tooltip("The time it takes the window to start popping after the call [s].")]
        [SerializeField] private float popDelay = 0;

        [Tooltip("The time it takes the losing window to pop [s].")]
        [SerializeField] private float popTime = .5f;
        #endregion

        #region Class Members
        private CanvasGroup canvas;
        #endregion

        #region Properties
        public abstract PromtType Type { get; }
        public bool IsDisplayed { get; private set; }
        #endregion

        protected virtual void Awake() {
            this.canvas = GetComponent<CanvasGroup>();
            Button[] buttons = GetComponentsInChildren<Button>();

            //close the window on each button click
            foreach (Button button in buttons)
                button.onClick.AddListener(Close);
        }

        protected virtual void Start() {
            //close this window by default
            this.IsDisplayed = true;
            Close();
        }

        /// <summary>
        /// Fade the window in or out.
        /// </summary>
        /// <param name="fadeIn">True to fade the window in or false to fade it out</param>
        /// <param name="time">The time it takes the animation to complete [s]</param>
        protected virtual IEnumerator Fade(bool fadeIn, float time) {
            //delay
            if (popDelay > 0) yield return new WaitForSeconds(popDelay);

            float from = canvas.alpha;
            float to = fadeIn ? 1 : 0;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(from, to, timer / time);
                yield return null;
            }
        }

        /// <summary>
        /// Scale the window up to full scale or down to zero.
        /// </summary>
        /// <param name="up">True to scale the window up or false to scale it down</param>
        /// <param name="time">The time it takes the animation to complete [s]</param>
        protected virtual IEnumerator Scale(bool up, float time) {
            //delay
            if (popDelay > 0) yield return new WaitForSeconds(popDelay);

            Vector3 from = transform.localScale;
            Vector3 to = up ? Vector3.one : Vector3.zero;
            float timer = 0;

            while (timer <= time) {
                timer += Time.deltaTime;
                transform.localScale = Vector3.Lerp(from, to, timer / time);
                yield return null;
            }
        }

        /// <summary>
        /// Display or hide this promt window.
        /// </summary>
        /// <param name="flag">True to display or false to hide</param>
        /// <returns>True if the action is successful.</returns>
        protected virtual bool Display(bool flag) {
            if (IsDisplayed == flag) return false;

            //enable or disable player input
            CursorViewer.Instance.Enable(flag, true);
            //Submarine.Instance.Controller.IsMovable = !flag;
            CameraRig rig = CameraManager.Instance.GetRig(CameraRole.Main);
            WeaponManager.Instance.Enable(!flag, true);
            rig.Enable(!flag, true);

            //display or hide window
            StopAllCoroutines();
            float displayTime = flag ? popTime : 0;
            StartCoroutine(Fade(flag, displayTime));
            StartCoroutine(Scale(flag, displayTime));

            IsDisplayed = flag;
            return true;
        }

        /// <summary>
        /// Open this promt window.
        /// </summary>
        public virtual void Open() {
            if (Display(true)) OnOpen();
        }

        /// <summary>
        /// Close this promt window.
        /// </summary>
        public virtual void Close() {
            if (Display(false)) OnClose();
        }

        /// <summary>
        /// Activate when this window opens.
        /// </summary>
        protected abstract void OnOpen();

        /// <summary>
        /// Activate when this window Closes.
        /// </summary>
        protected abstract void OnClose();
    }
}