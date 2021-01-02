using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("A list containing all of the object's tunes.")]
    [SerializeField] private List<Tune> tunes;
    #endregion

    #region Constants
    private static readonly string PARENT_NAME = "Audio";
    #endregion

    #region Class Members
    private TunesLimiter limiter;
    #endregion

    #region Properties
    public List<Tune> Tunes { get { return tunes; } }
    public List<string> TuneNames {
        get {
            List<string> names = new List<string>();
            foreach (Tune tune in tunes) names.Add(tune.Name);
            return names;
        }
    }
    #endregion

    private void Awake() {
        this.limiter = TunesLimiter.Instance;
        GameObject audioParent = new GameObject(PARENT_NAME);
        audioParent.transform.SetParent(transform);

        //create an audio source component for each tune
        foreach (Tune tune in tunes) {
            AudioSource audioSource = audioParent.AddComponent<AudioSource>();
            tune.Source = audioSource;
            audioSource.loop = tune.IsLoop;
            audioSource.outputAudioMixerGroup = VolumeController.Instance.GetGenreGroup(tune.Genre);
            limiter.Subscribe(tune);

            //auto play the tune
            if (tune.PlayOnAwake) Play(tune);
        }
    }

    private void OnDestroy() {
        foreach (Tune tune in tunes) Stop(tune);
    }

    /// <param name="name">The name of the tune</param>
    /// <returns>The correct tune, or null if it doesn't exist.</returns>
    public Tune Get(string name) {
        return tunes.Find(x => x.Name == name);
    }

    /// <summary>
    /// Play a tune.
    /// </summary>
    /// <param name="tune">The tune to play</param>
    public void Play(Tune tune) {
        if (tune != null && limiter.GetPermission(tune)) {
            tune.Source.PlayDelayed(tune.Delay);

            if (!tune.IsLoop) {
                float time = tune.Delay + tune.Duration;
                tune.Coroutine = StartCoroutine(StopAfterSeconds(tune, time));
            }
        }
    }

    /// <see cref="Play(string)"/>
    /// <param name="name">The tune's name</param>
    public void Play(string name) { Play(Get(name)); }

    /// <summary>
    /// Stop a tune.
    /// </summary>
    /// <param name="name">The tune's name</param>
    public void Stop(Tune tune) {
        if (tune != null && tune.Coroutine != null) {
            StopCoroutine(tune.Coroutine);
            tune.Coroutine = null;
            tune.Stop();
        }
    }

    /// <see cref="Stop(string)"/>
    /// <param name="name">The tune's name</param>
    public void Stop(string name) { Stop(Get(name)); }

    /// <summary>
    /// Stop the tune after a fixed amount of seconds.
    /// </summary>
    /// <param name="seconds">Amount of seconds after which the tune is stopped</param>
    private IEnumerator StopAfterSeconds(Tune tune, float seconds) {
        yield return new WaitForSeconds(seconds);
        if (tune.IsPlaying) tune.Stop();
    }
}