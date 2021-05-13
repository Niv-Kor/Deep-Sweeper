using Constants;
using DeepSweeper.Flow;
using DeepSweeper.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeepSweeper.Level.Mine
{
    public class IndicationSystem : MineSystem
    {
        #region Exposed Editor Parameters
        [Header("Float Settings")]
        [Tooltip("The vertical distance of the float.")]
        [SerializeField] private float waveLength = 1;

        [Tooltip("The speed of vertically floating.")]
        [SerializeField] private float floatSpeed = 1;
        #endregion

        #region Class Members
        private IndicationNumber indicationNum;
        private Transform textTransform;
        private SphereCollider sphereCol;
        private Submarine player;
        private bool revealable;
        #endregion

        #region Properties
        public bool IsFatal { get; set; }
        public float FatalityChance { get; private set; }
        public bool IsRevealed => revealable || indicationNum.Alpha > 0;
        public int Value {
            get => IsFatal ? -1 : indicationNum.Value;
            set { indicationNum.Value = value; }
        }

        public bool IsIndicationFulfilled {
            get {
                int flaggedNeighbours = (from neighbour in Grid.Section
                                         where neighbour != null && neighbour.SelectionSystem.IsFlagged
                                         select neighbour).Count();

                return Value == flaggedNeighbours;
            }
        }
        #endregion

        private void Awake() {
            this.indicationNum = GetComponentInChildren<IndicationNumber>();
            this.textTransform = indicationNum.transform.parent;
            this.player = Submarine.Instance;
            this.sphereCol = GetComponent<SphereCollider>();
            this.revealable = false;
            this.IsFatal = false;
        }

        private void Start() {
            FlagsManager flagsMngr = FlagsManager.Instance;

            //calculate the fatality chance every time a change occurs to one of the neighbours
            foreach (MineGrid neighbour in Grid.Section) {
                if (neighbour == null) continue;
                neighbour.DetonationSystem.DetonationEvent += CalcFatalityChance;
                flagsMngr.FlagsAmountUpdateEvent += CalcFatalityChance;
                flagsMngr.FlagReturnedEvent += delegate (bool _) { CalcFatalityChance(); };
                flagsMngr.FlagTakenEvent += delegate (bool _) { CalcFatalityChance(); };
            }

            CalcFatalityChance();
        }

        /// <summary>
        /// Activate the indicator so that it can be interactable with the player.
        /// </summary>
        public void Activate() {
            void SetActiveLayer() {
                gameObject.layer = Layers.GetLayerValue(Layers.MINE_INDICATION);
            }

            Phase currentPhase = LevelFlow.Instance.CurrentPhase;
            if (currentPhase != null && currentPhase.Field == Grid.Field) SetActiveLayer();
            else Grid.Field.FieldActivatedEvent += SetActiveLayer;
        }

        /// <summary>
        /// Display the text indicator.
        /// Set 'alphaPercent' to 0 in order to hide it.
        /// </summary>
        /// <param name="alphaPercent">Alpha value of the text [0:1]</param>
        public void Reveal(float alphaPercent) {
            if (revealable) {
                alphaPercent = Mathf.Clamp(alphaPercent, 0, 1);
                indicationNum.Alpha = alphaPercent;
            }
        }

        /// <summary>
        /// Allow or forbid the display of the indicator's text.
        /// Once forbidden, the text is no longer displayed.
        /// </summary>
        /// <param name="flag">True to allow or false to forbid</param>
        public void AllowRevelation(bool flag) {
            sphereCol.enabled = flag;
            revealable = flag;

            if (flag) StartCoroutine(Float());
            else {
                StopAllCoroutines();
                Reveal(0);
            }
        }

        /// <summary>
        /// Float up and down.
        /// </summary>
        private IEnumerator Float() {
            float startHeight = textTransform.position.y;
            float timer = Random.Range(-1f, 1f); //randomize sine wave

            while (true) {
                if (IsRevealed) {
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

        /// <summary>
        /// Calculate the chance of this mine being fatal.
        /// </summary>
        public void CalcFatalityChance() {
            if (Grid.DetonationSystem.IsDetonated) {
                FatalityChance = 0;
                return;
            }

            FlagsManager flagsMngr = FlagsManager.Instance;
            List<float> chances = new List<float>();
            float defaultChance = 1f / flagsMngr.AvailableFlags;
            chances.Add(defaultChance);

            foreach (MineGrid neighbour in Grid.Section) {
                if (neighbour == null) continue;

                bool dismissed = neighbour.DetonationSystem.IsDetonated;
                int number = neighbour.IndicationSystem.Value;

                if (dismissed && number > 0) {
                    List<MineGrid> neighbourSection = neighbour.Section;
                    int emptyGrids = 0;
                    int flaggedGrids = 0;

                    foreach (MineGrid farNeighbour in neighbourSection) {
                        if (farNeighbour == null) continue;
                        else if (!farNeighbour.DetonationSystem.IsDetonated) {
                            if (farNeighbour.SelectionSystem.IsFlagged) flaggedGrids++;
                            else emptyGrids++;
                        }
                    }

                    int unfulfilled = number - flaggedGrids;
                    float neighbourChance = (float)unfulfilled / emptyGrids;
                    chances.Add(neighbourChance);
                }
            }

            //find maximum chance
            float maxChance = defaultChance;
            foreach (float chance in chances)
                if (chance > maxChance) maxChance = chance;

            FatalityChance = maxChance;
        }
    }
}