using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Jukebox))]
public class SoundMixer : StateMachine
{
    #region Class Members
    private Jukebox jukebox;
    #endregion

    /// <inheritdoc />
    protected override void Awake() {
        this.jukebox = GetComponent<Jukebox>();
        base.Awake();
    }

    /// <inheritdoc />
    public override void Activate(string param, bool flag) {
        if (string.IsNullOrEmpty(param)) return;

        if (flag) jukebox.Play(param);
        else jukebox.Stop(param);
    }

    /// <inheritdoc />
    public override bool IsAtState(string state) {
        Tune tune = jukebox.Get(state);
        return tune != null && tune.IsPlaying;
    }

    /// <inheritdoc />
    protected override List<string> RetrieveStates() {
        return jukebox.TuneNames;
    }

    /// <inheritdoc />
    public override void Idlize() {}

    /// <inheritdoc />
    public override void StrongIdlize() {}
}