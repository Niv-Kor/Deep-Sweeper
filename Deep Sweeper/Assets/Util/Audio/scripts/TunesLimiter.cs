using System.Collections.Generic;
using UnityEngine;

public class TunesLimiter : Singleton<TunesLimiter>
{
    private class TuneLimitData
    {
        #region Properties
        public Tune Tune { get; private set; }
        public float Cooldown { get; private set; }
        public float RemainTime { get; private set; }
        public bool Timeout { get { return RemainTime <= 0; } }
        #endregion

        /// <param name="tune">The tune to limit</param>
        public TuneLimitData(Tune tune) {
            this.Tune = tune;
            this.Cooldown = tune.Cooldown;
            this.RemainTime = 0;
        }

        /// <summary>
        /// Reset the limitation time of the tune.
        /// </summary>
        public void ResetTime() { RemainTime = Cooldown; }

        /// <summary>
        /// Subtract an amount of seconds from the tune's cooldown.
        /// </summary>
        /// <param name="delta">
        /// Amount of time to subtract from the cooldown's
        /// remaining time (in seconds)
        /// </param>
        public void UpdateTime(float delta) { RemainTime -= delta; }
    }

    #region Class Members
    private IDictionary<string, TuneLimitData> subscriptors;
    #endregion

    protected override void Awake() {
        base.Awake();
        this.subscriptors = new Dictionary<string, TuneLimitData>();
    }

    private void Update() {
        foreach (string key in subscriptors.Keys) {
            subscriptors.TryGetValue(key, out TuneLimitData data);
            data.UpdateTime(Time.deltaTime);
        }
    }

    /// <summary>
    /// Check if a tune is already subscribed.
    /// </summary>
    /// <param name="tune">The tune to check</param>
    /// <returns>True if the tune is already subscribed.</returns>
    public bool Exists(Tune tune) {
        return subscriptors.TryGetValue(tune.Name, out TuneLimitData _);
    }

    /// <summary>
    /// Subscribe a tune.
    /// </summary>
    /// <param name="tune">The tune to subscribe</param>
    public void Subscribe(Tune tune) {
        if (tune.Cooldown <= 0 || Exists(tune)) return;

        TuneLimitData newData = new TuneLimitData(tune);
        subscriptors.Add(tune.Name, newData);
    }

    /// <summary>
    /// Unsubscribe a tune.
    /// </summary>
    /// <param name="tune">The tune of which to remove the subscription</param>
    public void Unsubscribe(Tune tune) {
        if (Exists(tune)) subscriptors.Remove(tune.Name);
    }

    /// <summary>
    /// Check if a tune is already allowed to be used again.
    /// </summary>
    /// <param name="tune">The tune to check</param>
    /// <returns>True if the tune is allowed to be used.</returns>
    public bool GetPermission(Tune tune) {
        bool subscribed = subscriptors.TryGetValue(tune.Name, out TuneLimitData data);

        if (subscribed) {
            bool timeout = data.Timeout;
            if (timeout) data.ResetTime();
            return timeout;
        }
        else return true;
    }
}