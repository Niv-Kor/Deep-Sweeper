namespace DeepSweeper.Player.Input
{
    public interface ILockedInput
    {
        /// <summary>
        /// Enable the input.
        /// </summary>
        void OnEnableInput();

        /// <summary>
        /// Disable the input.
        /// </summary>
        void OnDisableInput();

        /// <summary>
        /// Enable or disable this input.
        /// </summary>
        /// <param name="flag">True to enable or false to disable</param>
        /// <param name="force">
        /// True to force this change even if the input is locked.
        /// </param>
        void Enable(bool flag, bool force = false);
    }
}