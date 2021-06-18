using UnityEngine.Events;

namespace DeepSweeper.Player.Input
{
    public class InputLocker
    {
        #region Class Members
        private UnityAction enableFunc;
        private UnityAction disableFunc;
        #endregion

        #region Properties
        public bool Locked { get; private set; } = false;
        #endregion

        /// <param name="enableAction">A method that enables the input</param>
        /// <param name="disableAction">A method that disables the input</param>
        public InputLocker(UnityAction enableAction, UnityAction disableAction) {
            this.enableFunc = enableAction;
            this.disableFunc = disableAction;
        }

        /// <summary>
        /// Enable or disable this input.
        /// </summary>
        /// <param name="flag">True to enable or false to disable</param>
        /// <param name="force">
        /// True to force this change even if the input is locked.
        /// </param>
        public void Enable(bool flag, bool force = false) {
            if (!force && Locked) return;

            if (flag) {
                if (force) Locked = true;
                enableFunc();
            }
            else {
                if (Locked && force) Locked = false;
                disableFunc();
            }
        }
    }
}