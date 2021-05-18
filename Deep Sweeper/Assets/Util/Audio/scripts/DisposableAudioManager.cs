public class DisposableAudioManager : Singleton<DisposableAudioManager>
{
    #region Class Members
    private Jukebox jukebox;
    #endregion

    protected override void Awake() {
        base.Awake();
        this.jukebox = GetComponent<Jukebox>();
    }

    /// <summary>
    /// Create a new disposable AudioSource component.
    /// </summary>
    /// <param name="tune">The tune to export</param>
    /// <param name="parent">The parent jukebox of the exported tune</param>
    /// <returns>The external tune's clone.</returns>
    public Tune ExportTune(Tune tune, Jukebox parent) {
        Tune tuneClone = new Tune(tune);
        tuneClone.Externalize(parent);
        jukebox.Add(tuneClone);
        return tuneClone;
    }

    /// <summary>
    /// Play a tune.
    /// </summary>
    /// <param name="tune">The tune to play</param>
    public void Play(Tune tune) { jukebox.Play(tune); }

    /// <summary>
    /// Stop a tune.
    /// </summary>
    /// <param name="tune">The tune to stop</param>
    public void Stop(Tune tune) { jukebox.Stop(tune); }
}