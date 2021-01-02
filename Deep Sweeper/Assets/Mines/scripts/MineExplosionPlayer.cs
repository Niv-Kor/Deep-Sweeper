using UnityEngine;

[RequireComponent(typeof(Jukebox))]
public class MineExplosionPlayer : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The name of the mine explosion tune.")]
    [SerializeField] private string tuneName;

    [Tooltip("The minimum (x) and maximum (y) distance from a mine.\n"
           + "If the distance from a mine is smaller or equals to the minimum value, "
           + "the volume of the tune will be the lowest possible, "
           + "while if the distance is larger or equals the maximum value, "
           + "the volume of the tune will be the highest possible.")]
    [SerializeField] private Vector2 minMaxDistance;

    [Tooltip("The minimum and maximum volume values of the mine explosion tune.")]
    [SerializeField] private Vector2 minMaxVolume = new Vector2(.1f, 1);
    #endregion

    #region Class Members
    private Jukebox jukebox;
    private Tune tune;
    #endregion

    private void Awake() {
        this.jukebox = GetComponent<Jukebox>();
        this.tune = jukebox.Get(tuneName);
    }

    /// <summary>
    /// Calculate the correct volume of the tune,
    /// relative to the distance of the submarine from the mine.
    /// </summary>
    /// <param name="mineDist">The distance from the mine</param>
    /// <returns>A volume value within the possible range.</returns>
    private float CalcVolume(float mineDist) {
        float distPercent = 1 - RangeMath.NumberOfRange(mineDist, minMaxDistance);
        return RangeMath.PercentOfRange(distPercent, minMaxVolume);
    }

    /// <summary>
    /// Play the mine explosion sound with an appropriate volume,
    /// relative to the distance of the submarine from the mine.
    /// </summary>
    /// <param name="mineDist">The distance from the mine</param>
    public void Play(float mineDist) {
        if (tune == null) return;

        float originVolume = tune.Volume;
        tune.Volume = CalcVolume(mineDist);
        tune.StopEvent += delegate { tune.Volume = originVolume; };
        jukebox.Play(tune);
    }
}