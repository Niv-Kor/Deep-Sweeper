using Constants;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Float Settings")]
    [Tooltip("The vertical distance of the float.")]
    [SerializeField] private float waveLength = 1;

    [Tooltip("The speed of vertically floating.")]
    [SerializeField] private float floatSpeed = 1;
    #endregion

    #region Class Members
    private MineGrid grid;
    private IndicationNumber indicationNum;
    private Transform textTransform;
    private Submarine player;
    private SphereCollider sphereCol;
    private bool allowReveal;
    #endregion

    #region Properties
    public int Value {
        get => indicationNum.Value;
        set { indicationNum.Value = value; }
    }

    public bool IsIndicationFulfilled {
        get {
            int flaggedNeighbours = (from neighbour in grid.Section
                                     where neighbour != null && neighbour.IsFlagged
                                     select neighbour).Count();

            return Value == flaggedNeighbours;
        }
    }
    #endregion

    private void Awake() {
        this.grid = GetComponentInParent<MineGrid>();
        this.indicationNum = GetComponentInChildren<IndicationNumber>();
        this.textTransform = indicationNum.transform.parent;
        this.player = Submarine.Instance;
        this.sphereCol = GetComponent<SphereCollider>();
        this.allowReveal = false;
    }

    /// <summary>
    /// Activate the indicator so that it can be interactable with the player.
    /// </summary>
    public void Activate() {
        void SetActiveLayer() {
            gameObject.layer = Layers.GetLayerValue(Layers.MINE_INDICATION);
        }

        Phase currentPhase = LevelFlow.Instance.CurrentPhase;
        if (currentPhase != null && currentPhase.Field == grid.Field) SetActiveLayer();
        else grid.Field.FieldActivatedEvent += SetActiveLayer;
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
    /// Once forbidden, the text is no longer displayed.
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