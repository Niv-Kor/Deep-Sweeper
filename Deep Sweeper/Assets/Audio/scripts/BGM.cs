using UnityEngine;

[RequireComponent(typeof(SoundMixer))]
public class BGM : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The background music's volume.")]
    [SerializeField] public float Volume = .5f;

    [Tooltip("The time it takes the background music to start playing \n" + 
             "(Only available when 'playOnAwake' is checked).")]
    [SerializeField] private float startDelay;

    [Tooltip("True to start playing automatically as soon as the game starts.")]
    [SerializeField] private bool playOnAwake = true;
    #endregion

    #region Class Members
    private SoundMixer soundMixer;
    private Jukebox jukebox;
    private Tune song;
    private float startTimer;
    private bool reachDelayTime;
    #endregion

    private void Start() {
        this.soundMixer = GetComponent<SoundMixer>();
        this.jukebox = GetComponent<Jukebox>();
        this.reachDelayTime = false;
        this.startTimer = 0;
        this.song = jukebox.Tunes.Find(x => x.Genre == Genre.BGM);

        if (song != null) {
            song.Volume = Volume;
            song.IsLoop = true;
        }
    }

    private void Update() {
        if (playOnAwake && !reachDelayTime) {
            if (startTimer < startDelay) startTimer += Time.deltaTime;
            else {
                Play();
                reachDelayTime = true;
            }
        }
    }

    /// <summary>
    /// Play the song.
    /// </summary>
    public void Play() {
        if (song != null) soundMixer.Activate(song.Name);
    }

    /// <summary>
    /// Stop the song.
    /// </summary>
    public void Stop() {
        if (song != null) soundMixer.Activate(song.Name, false);
    }

    /// <returns>True if the song is playing at the moment.</returns>
    internal bool IsPlaying() {
        if (song != null) return soundMixer.IsAtState(song.Name);
        else return false;
    }
}