public class DisposableAudioManager : Singleton<DisposableAudioManager>
{
    #region Class Members
    private Jukebox jukebox;
    #endregion

    private void Awake() {
        this.jukebox = GetComponent<Jukebox>();
    }

    /// <summary>
    /// Create a new disposable AudioSource component.
    /// </summary>
    public void ExportTune(Tune tune) {
        jukebox.Add(tune);
    }

    /// <summary>
    /// Play a tune.
    /// </summary>
    /// <param name="tune">The tune to play</param>
    public void Play(Tune tune) {
        jukebox.Play(tune);
    }

    /// <summary>
    /// Stop a tune.
    /// </summary>
    /// <param name="tune">The tune to stop</param>
    public void Stop(Tune tune) {
        jukebox.Stop(tune);
    }
}