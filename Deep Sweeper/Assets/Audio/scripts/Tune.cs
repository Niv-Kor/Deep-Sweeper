using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Tune
{
    #region Exposed Editor Parameters
    [Tooltip("The name of the tune.")]
    [SerializeField] private string name;

    [Tooltip("The audio file to play.")]
    [SerializeField] private AudioClip clip;

    [Tooltip("The volume of the sound [0:1].")]
    [SerializeField] private float volume = .5f;

    [Tooltip("The pitch value of the sound (1 is natural).")]
    [SerializeField] private float pitch = 1;

    [Tooltip("A delay to add before the sound plays (in seconds).")]
    [SerializeField] private float delay = 0;

    [Tooltip("True to play the tune infinitely.")]
    [SerializeField] private bool loop = false;

    [Tooltip("The mixer to which this tune belongs.")]
    [SerializeField] private Genre genre;
    #endregion

    #region Class Members
    private AudioSource m_source;
    #endregion

    #region Events
    public event UnityAction StopEvent;
    #endregion

    #region Properties
    public string Name { get { return name; } }
    public float Delay { get { return delay; } }
    public Genre Genre { get { return genre; } }
    public AudioClip Clip { get { return clip; } }
    public Coroutine Coroutine { get; set; }
    public AudioSource Source {
        get { return m_source; }
        set {
            m_source = value;
            m_source.clip = clip;
            m_source.volume = volume;
            m_source.pitch = pitch;
        }
    }

    public bool IsLoop {
        get { return loop; }
        set { loop = value; }
    }

    public float Volume {
        get { return volume; }
        set {
            if (Source != null) {
                Source.volume = value;
                volume = value;
            }
        }
    }

    public float Pitch {
        get { return pitch; }
        set {
            if (Source != null) {
                Source.pitch = value;
                pitch = value;
            }
        }
    }

    public float Duration {
        get {
            if (clip == null) return 0;
            else return clip.length;
        }
    }

    public bool IsPlaying {
        get {
            if (Source == null) return false;
            else return Source.isPlaying;
        }
    }
    #endregion

    /// <summary>
    /// Stop the tune.
    /// </summary>
    public void Stop() {
        Source.Stop();
        StopEvent?.Invoke();
    }

    /// <param name="vol">The new volume of the tune</param>
    public void SetVolume(float vol) {
        Source.volume = vol;
        volume = vol;
    }

    /// <param name="ptch">The new pitch of the tune</param>
    public void SetPitch(float ptch) {
        Source.pitch = ptch;
        pitch = ptch;
    }
}