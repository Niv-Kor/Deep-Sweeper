using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathTint : Singleton<DeathTint>
{
    #region Exposed Editor Parameters
    [Tooltip("The amount of time (in seconds) it takes to tint in and out.")]
    [SerializeField] private float flashTime;

    [Tooltip("The maximum alpha value of the tint color.")]
    [SerializeField] private float maxAlpha = 1;
    #endregion

    #region Class Members
    private Image tintImage;
    private Color transparent, destColor;
    #endregion

    private void Start() {
        this.tintImage = GetComponent<Image>();
        this.destColor = tintImage.color;
        this.transparent = tintImage.color;
        destColor.a = maxAlpha;
        transparent.a = 0;
        tintImage.color = transparent;
    }

    /// <summary>
    /// Activate tint effect.
    /// </summary>
    public void Tint() {
        StartCoroutine(FlickerTint());
    }

    /// <summary>
    /// Quickly display and then hide the tint effect.
    /// </summary>
    private IEnumerator FlickerTint() {
        float threshold = flashTime / 2;
        float timer = 0;

        while (timer <= threshold) {
            timer += Time.deltaTime;
            tintImage.color = Color.Lerp(transparent, destColor, timer / threshold);
            yield return null;
        }

        timer = 0;
        while (timer <= threshold) {
            timer += Time.deltaTime;
            tintImage.color = Color.Lerp(destColor, transparent, timer / threshold);
            yield return null;
        }
    }
}