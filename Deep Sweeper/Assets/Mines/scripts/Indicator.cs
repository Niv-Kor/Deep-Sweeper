using System.Collections;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [Header("Float Settings")]
    [Tooltip("The vertical distance of the float.")]
    [SerializeField] private float waveLength = 1;

    [Tooltip("The speed of vertically floating.")]
    [SerializeField] private float floatSpeed = 1;

    private IndicationNumber indicationNum;
    private Transform textTransform;
    private Submarine player;
    private SphereCollider sphereCol;
    private bool allowReveal;

    public int MinedNeighbours {
        get { return indicationNum.Value; }
        set { indicationNum.Value = value; }
    }

    private void Awake() {
        this.indicationNum = GetComponentInChildren<IndicationNumber>();
        this.textTransform = indicationNum.transform.parent;
        this.player = Submarine.Instance;
        this.sphereCol = GetComponent<SphereCollider>();
        this.allowReveal = false;
        StartCoroutine(Float());
    }

    /// <summary>
    /// Display the text indicator.
    /// Set 'alphaPercent' to 0 in order to hide it.
    /// </summary>
    /// <param name="alphaPercent">Alpha value of the text [0:1]</param>
    public void Display(float alphaPercent) {
        if (allowReveal) indicationNum.Alpha = alphaPercent;
    }

    /// <returns>True if the indicator is enabled and displayed.</returns>
    public bool IsDisplayed() { return allowReveal || indicationNum.Alpha > 0; }

    /// <summary>
    /// Allow or forbid the display of the indicator's text.
    /// Once forbidded, the text is no longer displayed.
    /// </summary>
    /// <param name="flag">True to allow or false to forbid</param>
    public void AllowRevelation(bool flag) {
        sphereCol.enabled = flag;
        allowReveal = flag;

        if (flag) StartCoroutine(Float());
        else {
            StopAllCoroutines();
            Display(0);
        }
    }

    /// <summary>
    /// Float up and down.
    /// </summary>
    private IEnumerator Float() {
        float startHeight = textTransform.position.y;
        float timer = Random.Range(-1f, 1f); //randomize sine

        while (true) {
            if (IsDisplayed()) {
                //float
                timer += Time.deltaTime;
                float sineWave = Mathf.Sin(timer * floatSpeed);
                float nextHeight = startHeight + waveLength * sineWave;
                Vector3 pos = transform.position;
                Vector3 nextPos = new Vector3(pos.x, nextHeight, pos.z);
                textTransform.position = nextPos;
                sphereCol.center = textTransform.localPosition;

                //rotate towards the player
                Vector3 playerDir = Vector3.Normalize(textTransform.position - player.transform.position);
                Quaternion playerRot = Quaternion.LookRotation(playerDir);
                Vector3 playerRotVect = Vector3.Scale(playerRot.eulerAngles, Vector3.up);
                textTransform.rotation = Quaternion.Euler(playerRotVect);
            }

            yield return null;
        }
    }
}