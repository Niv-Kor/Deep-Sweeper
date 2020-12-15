public class DiegeticsManager : UIManager<DiegeticsManager>
{
    /// <summary>
    /// Activate or deactivate the diegestic UI components.
    /// </summary>
    /// <param name="flag">True to activate or false to deactivate</param>
    /// <param name="time">The time of the fade (in/out)</param>
    public void Activate(bool flag, float time) {
        if (time != -1 && time >= 0) {
            float originFadeTime = fadeTime;
            fadeTime = time;
            void RestoreFadeTime() { fadeTime = originFadeTime; }
            Activate(flag, RestoreFadeTime);
        }
        else Activate(flag);
    }
}