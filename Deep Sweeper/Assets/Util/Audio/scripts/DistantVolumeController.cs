using UnityEngine;

public class DistantVolumeController : Singleton<DistantVolumeController>
{
    #region Exposed Editor Parameters
    [Tooltip("The object from which all distances are calculated (normally the player).")]
    [SerializeField] private GameObject sceneAnchor;

    [Tooltip("The minimum (x) and maximum (y) distance from the anchor.\n"
           + "If a distance from the anchor is smaller or equals to the minimum value, "
           + "the volume of the tune will be the lowest possible, "
           + "while if the distance is larger or equals the maximum value, "
           + "the volume of the tune will be the highest possible.")]
    [SerializeField] private Vector2 minMaxDistance;

    [Tooltip("The percentage of the original volume of a tune to set as its minimum volume value.")]
    [SerializeField] [Range(0f, 1f)] private float minVolumePercent = .1f;
    #endregion

    /// <summary>
    /// Calculate the correct volume of the tune,
    /// relative to the distance of its wrapping object from the anchor.
    /// If the anchor is not defined, the volume that will be returned is
    /// the original volume value of the tune.
    /// </summary>
    /// <param name="tune">The tune to play</param>
    /// <returns>A value within the range of 0 to the original volume value of the tune.</returns>
    public float CalcVolume(Tune tune) {
        if (tune == null || tune.Source == null) return 0;
        else if (sceneAnchor == null) return tune.Volume;

        float minVolume = minVolumePercent * tune.Volume;
        Vector2 minMaxVolume = new Vector2(minVolume, tune.Volume);
        Vector3 tuneObjPos = tune.OrganicParent.transform.position;
        Vector3 anchorPos = sceneAnchor.transform.position;
        float dist = Vector3.Distance(tuneObjPos, anchorPos);
        float distPercent = 1 - RangeMath.NumberOfRange(dist, minMaxDistance);
        float clampedDistPercent = Mathf.Clamp(distPercent, 0, 1);
        return RangeMath.PercentOfRange(clampedDistPercent, minMaxVolume);
    }
}