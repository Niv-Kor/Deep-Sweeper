using DeepSweeper.Player.Input;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.UI
{
    public class CursorViewer : Singleton<CursorViewer>, ILockedInput
    {
        #region Exposed Editor Parameters
        [Tooltip("View the cursor as the game begins.")]
        [SerializeField] private bool displayOnStart = true;
        #endregion

        #region Class Members
        private InputLocker inputLocker;
        private bool m_isDisplayed;
        #endregion

        #region Events
        /// <param type=typeof(bool)>True of the cursor is displayed</param>
        public UnityAction<bool> StatusChangeEvent;
        #endregion

        #region Properties
        public bool IsDisplayed {
            get => m_isDisplayed;
            private set {
                if (m_isDisplayed != value) {
                    m_isDisplayed = value;
                    StatusChangeEvent?.Invoke(value);
                }
            }
        }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.IsDisplayed = displayOnStart;
            this.inputLocker = new InputLocker(OnEnableInput, OnDisableInput);
            Enable(false, true);

            //bind events
            PlayerController.Instance.CursorDisplayEvent += delegate { IsDisplayed = true; };
            PlayerController.Instance.CursorDisplayHide += delegate { IsDisplayed = false; };
        }

        /// <inheritdoc/>
        public void OnEnableInput() {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
        }

        /// <inheritdoc/>
        public void OnDisableInput() {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <inheritdoc/>
        public void Enable(bool flag, bool force = false) {
            inputLocker.Enable(flag, force);
        }
    }
}