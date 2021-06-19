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

        /// <param name="lockedInput">The input object</param>
        public InputLocker(ILockedInput lockedInput) {
            this.enableFunc = lockedInput.OnEnableInput;
            this.disableFunc = lockedInput.OnDisableInput;
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