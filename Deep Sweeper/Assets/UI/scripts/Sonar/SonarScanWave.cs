using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SonarScanWave : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The central blank mask in front of the minimap mask.")]
    [SerializeField] private GameObject minimap;

    [Tooltip("The central blank mask in front of the minimap mask.")]
    [SerializeField] private GameObject scanningCircle;
    #endregion

    #region Constants
    private static readonly float CIRCLE_SIZE_OFFSET = 2;
    #endregion

    #region Class Members
    private float maxRad;
    private bool started, scanWhenReady;
    private RectTransform rectTransform;
    private RectTransform scanningCircleRect;
    private RawImage minimapDisplay;
    private Image scanningCircleImg;
    #endregion

    #region Public Properties
    public float ScanTime { get; set; }
    public float SignalFadeTime { get; set; }
    #endregion

    private void Start() {
        RectTransform parentRectTransform = transform.parent.GetComponent<RectTransform>();
        RectTransform minimapRectTransform = minimap.GetComponent<RectTransform>();
        this.rectTransform = GetComponent<RectTransform>();
        this.maxRad = parentRectTransform.sizeDelta.x;
        this.minimapDisplay = minimap.GetComponent<RawImage>();
        this.scanningCircleRect = scanningCircle.GetComponent<RectTransform>();
        this.scanningCircleImg = scanningCircle.GetComponent<Image>();
        this.started = true;

        //resize component according to parent
        rectTransform.sizeDelta = parentRectTransform.sizeDelta;
        minimapRectTransform.sizeDelta = parentRectTransform.sizeDelta;

        if (scanWhenReady) StartCoroutine(Wave());
    }

    /// <summary>
    /// Activate the sonar wave indefinitely.
    /// </summary>
    /// <seealso cref="Scan"/>
    private IEnumerator Wave() {
        minimap.SetActive(true);
        scanningCircle.SetActive(true);
        float minimapTimer = 0;

        while (minimapTimer <= ScanTime) {
            minimapTimer += Time.deltaTime;

            float minimapRad = Mathf.Lerp(0, maxRad, minimapTimer / ScanTime);
            Vector2 size = Vector2.one * minimapRad;
            rectTransform.sizeDelta = size;
            scanningCircleRect.sizeDelta = size;

            Color minimapColor = minimapDisplay.color;
            minimapColor.a = Mathf.Lerp(1, 0, minimapTimer / SignalFadeTime);
            minimapDisplay.color = minimapColor;

            Color circleColor = scanningCircleImg.color;
            circleColor.a = Mathf.Lerp(0, 1, minimapTimer / SignalFadeTime);
            scanningCircleImg.color = circleColor;

            yield return null;
        }
    }

    /// <summary>
    /// Activate the sonar wave indefinitely.
    /// </summary>
    public void Scan() {
        if (started) StartCoroutine(Wave());
        else scanWhenReady = true;
    }
}
