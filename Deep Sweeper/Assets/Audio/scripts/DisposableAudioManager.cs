using System.Collections.Generic;
using UnityEngine;

public class DisposableAudioManager : Singleton<DisposableAudioManager>
{
    private List<AudioSource> list;

    private void Awake() {
        this.list = new List<AudioSource>();
    }

    /// <summary>
    /// Create a new disposable AudioSource component.
    /// </summary>
    /// <returns>A new disposable AudioSource component.</returns>
    public AudioSource CreateSource() {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        list.Add(source);
        return source;
    }
}