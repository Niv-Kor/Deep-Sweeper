public class SpatialsManager : UIManager<SpatialsManager>
{
    /// <summary>
    /// Activate the phase spatials.
    /// </summary>
    /// <param name="difficulty">Current phase's difficulty level</param>
    /// <param name="timer">Currernt phase's timer (in integer seconds)</param>
    public void Activate(DifficultyLevel difficulty, int timer) {
        canvas.alpha = 0;
        PhaseTimerSpatial.Instance.Set(difficulty, timer);
        PhaseNameSpatial.Instance.Display(true);
        FlagsGaugeSpatial.Instance.Display(true);

        Activate(true);
    }

    /// <summary>
    /// Deactivate the phase spatials.
    /// </summary>
    public void Deactivate() {
        void Callback() {
            PhaseTimerSpatial.Instance.Stop();
            PhaseNameSpatial.Instance.Display(false);
            FlagsGaugeSpatial.Instance.Display(false);
        }

        Activate(false, Callback);
    }
}