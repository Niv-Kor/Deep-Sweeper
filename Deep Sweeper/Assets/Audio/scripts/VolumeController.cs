using UnityEngine.Audio;
using UnityEngine;

public class VolumeController : Singleton<VolumeController>
{
    #region Exposed Editor Parameters
    [Tooltip("The main mixer that the game works with.")]
    [SerializeField] private AudioMixer masterMixer;
    #endregion

    #region Constants
    private static readonly string EXPOSED_PARAMETER_SUFFIX = "vol";
    #endregion

    /// <summary>
    /// Get the volume of a single genre.
    /// </summary>
    /// <param name="genre">The genre of which to get the value</param>
    /// <returns>The value of the genre's volume.</returns>
    public float GetVolume(Genre genre) {
        AudioMixerGroup group = GetGenreGroup(genre);
        string exposedVolumeParameter = genre.ToString() + EXPOSED_PARAMETER_SUFFIX;
        group.audioMixer.GetFloat(exposedVolumeParameter, out float volume);
        return Mathf.Pow(10, volume / 20); //original value set
    }

    /// <summary>
    /// Set the volume of a single genre.
    /// </summary>
    /// <param name="genre">The genre of which to set the value</param>
    /// <param name="value">The value to set</param>
    public void SetVolume(Genre genre, float value) {
        AudioMixerGroup group = GetGenreGroup(genre);
        float volume = Mathf.Log10(value) * 20;
        print("log volume: " + volume);
        string exposedVolumeParameter = genre.ToString() + EXPOSED_PARAMETER_SUFFIX;
        group.audioMixer.SetFloat(exposedVolumeParameter, volume);
    }

    /// <summary>
    /// Get an audio mixer group that is compatible with a specified genre.
    /// </summary>
    /// <param name="genre">The genre of which to get the audio mixer group</param>
    /// <returns>The audio mixer group that is compatible with the specified genre.</returns>
    public AudioMixerGroup GetGenreGroup(Genre genre) {
        return masterMixer.FindMatchingGroups(genre.ToString())[0];
    }
}