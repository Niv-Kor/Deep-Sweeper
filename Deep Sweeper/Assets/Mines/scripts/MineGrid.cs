using Constants;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Level.Mine
{
    [RequireComponent(typeof(DetonationSystem))]
    [RequireComponent(typeof(SelectionSystem))]
    [RequireComponent(typeof(MineActivator))]
    [RequireComponent(typeof(LootGeneratorObject))]
    public class MineGrid : MonoBehaviour
    {
        #region Class Members
        private MineBouncer mine;
        private bool m_tempTarget;
        private int originLayer;
        #endregion

        #region Properties
        public GameObject Avatar => mine.gameObject;
        public DetonationSystem DetonationSystem { get; private set; }
        public SelectionSystem SelectionSystem { get; private set; }
        public IndicationSystem IndicationSystem { get; private set; }
        public LootGeneratorObject LootGenerator { get; private set; }
        public MineActivator Activator { get; private set; }
        public MineField Field { get; set; }
        public Vector2Int Position { get; set; }
        public int Layer {
            get => Avatar.layer;
            set {
                if (!TempTarget) Layers.ApplyLayer(Avatar, value, true);
                originLayer = value;
            }
        }

        public bool TempTarget {
            get => m_tempTarget;
            set {
                if (value != m_tempTarget) {
                    m_tempTarget = value;

                    if (value) Layers.ApplyLayer(Avatar, Layers.TARGET_MINE, true);
                    else Layer = originLayer;
                }
            }
        }

        public List<MineGrid> Section {
            get {
                int row = Position.x;
                int col = Position.y;
                return Field.GetSection(row, col);
            }
        }
        #endregion

        private void Awake() {
            this.Activator = GetComponent<MineActivator>();
            this.IndicationSystem = GetComponentInChildren<IndicationSystem>();
            this.DetonationSystem = GetComponent<DetonationSystem>();
            this.LootGenerator = GetComponent<LootGeneratorObject>();
            this.SelectionSystem = GetComponent<SelectionSystem>();
            this.mine = GetComponentInChildren<MineBouncer>();

            //set this object as a parent grid to its systems
            IndicationSystem.SetParentGrid(this);
            DetonationSystem.SetParentGrid(this);
            SelectionSystem.SetParentGrid(this);

            //bind events
            DetonationSystem.DetonationEvent += Activator.Unlock;
            SelectionSystem.ModeApplicationEvent += ChangeLayer;
        }

        /// <summary>
        /// Change the layer of the mine avatar and mine clone,
        /// according to their flag state.
        /// </summary>
        /// <param name="oldMode">The previous mine selection mode</param>
        /// <param name="newMode">The applied mine selection mode</param>
        private void ChangeLayer(SelectionMode oldMode, SelectionMode newMode) {
            bool flagMode = SelectionSystem.IsFlagMode(newMode);
            Layer = flagMode ? Layers.FLAGGED_MINE : Layers.MINE;
        }
    }
}