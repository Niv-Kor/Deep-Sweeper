using System.Collections.Generic;

namespace DeepSweeper.UI.Ingame.Promt
{
    public class IngameWindowManager : Singleton<IngameWindowManager>
    {
        #region Class Members
        private List<PromtWindow> windows;
        #endregion

        protected override void Awake() {
            base.Awake();
            this.windows = new List<PromtWindow>(GetComponentsInChildren<PromtWindow>());
        }

        /// <summary>
        /// Pop a promt window on the screen.
        /// </summary>
        /// <param name="type">The type of the window to pop</param>
        /// <returns>The popped window, or null if it couldn't be found.</returns>
        public PromtWindow Pop(PromtType type) {
            PromtWindow window = windows.Find(x => x.Type == type);
            window?.Open();
            return window;
        }
    }
}