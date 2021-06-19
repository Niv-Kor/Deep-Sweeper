using DeepSweeper.CameraSet;
using DeepSweeper.Characters;
using DeepSweeper.Flow;
using DeepSweeper.Level.Mine;
using DeepSweeper.Level.PhaseGate;
using DeepSweeper.Player;
using DeepSweeper.Player.ShootingSystem;
using DeepSweeper.UI.Ingame;
using DeepSweeper.UI.Ingame.Promt;
using DeepSweeper.UI.Ingame.Spatials.Commander;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Flow
{
    public class LevelFlow : Singleton<LevelFlow>
    {
        #region Exposed Editor Parameters
        [Header("Level Settings")]
        [Tooltip("The region to which this map belongs.")]
        [SerializeField] private Region region;

        [Header("Prefabs")]
        [Tooltip("The parent object under which all mine fields are instantiated.")]
        [SerializeField] private Transform fieldsParent;

        [Tooltip("A prefab of an object with a 'MineField' script attached to it.")]
        [SerializeField] private GameObject mineFieldPrefab;

        [Tooltip("The first gate in the scene, just besides the player's spawn point.")]
        [SerializeField] private Gate initialGate;

        [Header("Phases")]
        [Tooltip("A list of mine fields.\nEach field represents a level phase.")]
        [SerializeField] private PhaseConfig[] phases;

        [Header("Death")]
        [Tooltip("The time it takes to get to a fully blank screen when losing.")]
        [SerializeField] private float blankScreenLerpTime;

        [Tooltip("The time it takes to start lerping again after "
               + "reaching a fully blank screen when losing.")]
        [SerializeField] private float blankScreenPauseTime;
        #endregion

        #region Constants
        private static readonly string FIELD_NAME = "Field";
        private static readonly Color GIZMOS_COLOR = Color.yellow;
        #endregion

        #region Class Members
        private int phaseIndex;
        #endregion

        #region Events
        /// <param type=typeof(int)>The starting phase</param>
        public event UnityAction<Phase> PhaseStartEvent;

        /// <param type=typeof(int)>The paused phase</param>
        public event UnityAction<Phase> PhasePauseEvent;

        /// <param type=typeof(int)>The resumed phase</param>
        public event UnityAction<Phase> PhaseResumeEvent;

        /// <param type=typeof(int)>The finished phase</param>
        /// <param type=typeof(bool)>
        /// True if the phase ended successfully,
        /// or false if it ended due to the player's loss.
        /// </param>
        public event UnityAction<Phase, bool> PhaseEndEvent;

        /// <param type=typeof(PhaseConfig)>The updated phase's configuration</param>
        /// <param type=typeof(int)>The updated phase</param>
        public event UnityAction<Phase> PhaseUpdatedEvent;
        #endregion

        #region Properties
        public List<Phase> Phases { get; private set; }
        public Phase CurrentPhase => (phaseIndex != -1) ? Phases[phaseIndex] : null;
        public string RegionName => region.ToString().Replace('_', ' ');
        public bool DuringPhase { get; private set; }
        public bool IsPaused { get; private set; }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.Phases = new List<Phase>();
            this.DuringPhase = false;
            this.phaseIndex = -1;
            initialGate.RequestOpen(false);
        }

        private void Start() {
            InitTerritories();
            PrepareNextPhase();
        }

        private void OnValidate() {
            //check that each phase config contains the entire difficulty configurations set
            var levels = Enum.GetValues(typeof(DifficultyLevel));

            for (int i = 0; i < phases.Length; i++) {
                PhaseConfig phaseConfig = phases[i];

                if (phaseConfig.Levels.Count != levels.Length) {
                    List<PhaseDifficultyConfig> configList = new List<PhaseDifficultyConfig>();

                    foreach (DifficultyLevel level in levels) {
                        PhaseDifficultyConfig config = new PhaseDifficultyConfig {
                            Difficulty = level,
                            MinesPercent = 0,
                            PhaseReward = 0,
                            Clock = 999
                        };

                        configList.Add(config);
                    }

                    phases[i].Levels = configList;
                }
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = GIZMOS_COLOR;

            for (int i = 0; i < phases.Length; i++) {
                PhaseConfig phase = phases[i];
                Vector3 areaCenter = phase.Confine.Offset + phase.Confine.Size / 2;
                Gizmos.DrawWireCube(areaCenter, phase.Confine.Size);
                Handles.Label(areaCenter, phase.MapName + $"({i + 1})");
            }
        }

        /// <summary>
        /// Instantiate and initialize all pre-defined mine fields.
        /// </summary>
        private void InitTerritories() {
            Phase prevPhase = null;

            for (int i = 0; i < phases.Length; i++) {
                PhaseConfig phaseConfig = phases[i];

                //instantiate
                Transform fieldObj = Instantiate(mineFieldPrefab).transform;
                fieldObj.name = FIELD_NAME + $"({i})";
                fieldObj.position = Vector3.zero;
                fieldObj.rotation = Quaternion.identity;
                fieldObj.SetParent(fieldsParent);

                //configurate
                MineField mineFieldCmp = fieldObj.GetComponent<MineField>();
                mineFieldCmp.DefineArea(phaseConfig.Confine);

                //append
                Phase phaseObj;

                if (i == 0) phaseObj = new Phase(i, mineFieldCmp, initialGate, phaseConfig);
                else phaseObj = new Phase(i, mineFieldCmp, prevPhase, phaseConfig);

                if (prevPhase != null) prevPhase.FollowPhase = phaseObj;
                prevPhase = phaseObj;
                Phases.Add(phaseObj);
            }

            //init all fields
            foreach (Phase phase in Phases)
                phase.Field.Init(phase.DifficultyConfig);
        }

        /// <summary>
        /// Prepare the transition to the next phase.
        /// This function is crucial before starting a phase.
        /// </summary>
        private void PrepareNextPhase() {
            phaseIndex++;
            CurrentPhase.EntranceGate.GateCrossEvent += StartPhase;
        }

        /// <summary>
        /// End the current phase of the game.
        /// </summary>
        private void EndPhase(bool success) {
            if (!DuringPhase || CurrentPhase is null) return;

            DuringPhase = false;
            IsPaused = false;
            PhaseEndEvent?.Invoke(CurrentPhase, success);

            if (success) {
                CurrentPhase.Conclude();
                PrepareNextPhase();
            }
            else {
                //lose
            }
        }

        /// <summary>
        /// End the current phase of the game,
        /// but only if the current mine field is clear.
        /// </summary>
        public void TryEndPhase() {
            if (!DuringPhase || CurrentPhase is null) return;

            MineField currentField = CurrentPhase.Field;
            if (currentField.IsClear()) EndPhase(true);
        }

        /// <summary>
        /// Start the current phase.
        /// </summary>
        public void StartPhase() {
            if (DuringPhase) return;

            IsPaused = false;
            PhaseStartEvent?.Invoke(CurrentPhase);
            DuringPhase = true;
        }

        /// <summary>
        /// Pause the current phase.
        /// </summary>
        public void PausePhase() {
            if (!DuringPhase || CurrentPhase is null) return;

            IsPaused = true;
            PhasePauseEvent?.Invoke(CurrentPhase);
        }

        /// <summary>
        /// Resume the current phase after a pause.
        /// </summary>
        public void ResumePhase() {
            if (!DuringPhase || CurrentPhase is null || !IsPaused) return;

            IsPaused = false;
            PhaseResumeEvent?.Invoke(CurrentPhase);
        }

        /// <summary>
        /// Report an initialized phase that finished updating.
        /// </summary>
        /// <param name="phaseConfig">Phase configuration</param>
        /// <param name="difficultyConfig">Phase difficultyConfiguration</param>
        /// <param name="index">The phase's index</param>
        public void ReportPhaseUpdated(Phase phase) {
            PhaseUpdatedEvent?.Invoke(phase);
        }

        /// <summary>
        /// Win the level.
        /// </summary>
        public void Win() {}

        /// <summary>
        /// Lose the level.
        /// </summary>
        public void Lose() {
            CommanderSpatial commander = SpatialsManager.Instance.Get(typeof(CommanderSpatial)) as CommanderSpatial;
            List<Persona> aliveCommanders = commander.AnnounceDeath();
            WeaponManager.Instance.StripCurrentAbility();
            EndPhase(false);

            if (aliveCommanders.Count > 0) {
                IngameWindowManager.Instance.Pop(PromtType.LosingWindow);
            }
        }
        /*
            //retreat to the entrance gate and start over
            if (LifeSupply.Instance.LifeDown()) {
                SpatialsManager.Instance.Deactivate();
                CameraRig rig = CameraManager.Instance.GetRig(CameraRole.Main);
                MineField currentField = CurrentPhase.Field;
                rig.Pause();

                //move player back to the entrance gate and rotate it
                void PlayerRelocation() {
                    Gate entranceGate = CurrentPhase.EntranceGate;
                    Vector3 gatePos = entranceGate.transform.position;
                    Vector3 lookPos = currentField.Center;
                    Transform player = Submarine.Instance.transform;
                    player.position = gatePos;
                    rig.transform.LookAt(lookPos);
                    Vector3 rot = rig.transform.rotation.eulerAngles;
                    rig.transform.rotation = Quaternion.Euler(0, rot.y, rot.z);
                }

                //reset field and clear loot history
                void ResetField() {
                    currentField.ResetAll();
                    LootManager.Instance.ClearPhaseItems(phaseIndex);
                    Suitcase.Instance.RemovePhaseItems(phaseIndex);
                    FlagsManager.Instance.ResetGauge();
                }

                void Finish() { rig.Resume(); }

                LoadingProcess process = new LoadingProcess();
                process.Enroll(PlayerRelocation, "retreating to the nearest phase entry");
                process.Enroll(ResetField, "resetting the mine field");
                process.Enroll(Finish);

                void FullyTransparent() {
                    DifficultyLevel difficulty = Contract.Instance.Difficulty;
                    int timer = CurrentPhase.DifficultyConfig.Clock;
                    SpatialsManager.Instance.Activate(difficulty, timer);
                }

                //blank screen
                float lerpTime = blankScreenLerpTime;
                float pauseTime = blankScreenPauseTime;
                BlankScreen.Instance.Apply(lerpTime, pauseTime, process, FullyTransparent);
            }
            //game over
            else {
                ///TODO
            }*/
    }
}