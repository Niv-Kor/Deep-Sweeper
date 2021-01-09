using com.ootii.Cameras;
using Constants;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GameFlow : Singleton<GameFlow>
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
    public event UnityAction<int> PhaseChangeEvent;
    public event UnityAction<PhaseConfig, PhaseDifficultyConfig, int> PhaseUpdatedEvent;
    #endregion

    #region Properties
    public List<Phase> Phases { get; private set; }
    public bool DuringPhase { get; private set; }
    public Phase CurrentPhase {
        get { return (phaseIndex != -1) ? Phases[phaseIndex] : null; }
    }

    public string RegionName {
        get { return region.ToString().Replace('_', ' '); }
    }
    #endregion

    private void Awake() {
        this.Phases = new List<Phase>();
        this.DuringPhase = false;
        this.phaseIndex = -1;
        initialGate.RequestOpen(false);
    }

    private void Start() {
        InitTerritories();
        NextPhase();
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
            Handles.Label(areaCenter, phase.MapName + " (" + (i + 1) + ")");
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
            fieldObj.name = FIELD_NAME + " (" + i + ")";
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
    /// Continue to the next phase of the game,
    /// but only if the current mine field is clear.
    /// </summary>
    public void TryNextPhase() {
        if (CurrentPhase == null) return;

        MineField currentField = CurrentPhase.Field;
        if (currentField.IsClear()) NextPhase();
    }

    /// <summary>
    /// Continue to the next phase of the game.
    /// </summary>
    public void NextPhase() {
        if (phaseIndex < Phases.Count - 1) {
            if (phaseIndex >= 0) {
                DuringPhase = false;
                Phases[phaseIndex++].Conclude();
            }
            else ++phaseIndex; //first phase

            PhaseChangeEvent?.Invoke(phaseIndex);
        }
    }

    /// <summary>
    /// Report an initialized phase that finished updating.
    /// </summary>
    /// <param name="phaseConfig">Phase configuration</param>
    /// <param name="difficultyConfig">Phase difficultyConfiguration</param>
    /// <param name="index">The phase's index</param>
    public void ReportPhaseUpdated(PhaseConfig phaseConfig, PhaseDifficultyConfig difficultyConfig, int index) {
        PhaseUpdatedEvent?.Invoke(phaseConfig, difficultyConfig, index);
        DuringPhase = true;
    }

    /// <summary>
    /// Retreat to the start of the current stage or start the Game Over scenario.
    /// </summary>
    public void Lose() {
        //retreat to the entrance gate and start over
        if (LifeSupply.Instance.LifeDown()) {
            print("w");
            SpatialsManager.Instance.Deactivate();
            CameraController camContrller = FindObjectOfType<CameraController>();
            MineField currentField = CurrentPhase.Field;
            camContrller.enabled = false;

            void FullyBlank() {
                print("h");
                //move player back to the entrance gate and rotate it
                Gate entranceGate = CurrentPhase.EntranceGate;
                Vector3 gatePos = entranceGate.transform.position;
                Vector3 lookPos = currentField.Center;
                Transform rig = CameraManager.Instance.Rig.transform;
                Transform player = Submarine.Instance.transform;
                player.position = gatePos;
                rig.LookAt(lookPos);
                Vector3 rot = rig.rotation.eulerAngles;
                rig.rotation = Quaternion.Euler(0, rot.y, rot.z);

                //reset field and clear loot history
                currentField.ResetAll();
                LootManager.Instance.ClearPhaseItems(phaseIndex);
                Suitcase.Instance.RemovePhaseItems(phaseIndex);

                camContrller.enabled = true;
                print("a");
            }

            void FullyTransparent() {
                DifficultyLevel difficulty = Contract.Instance.Difficulty;
                int timer = CurrentPhase.DifficultyConfig.Clock;
                SpatialsManager.Instance.Activate(difficulty, timer);
            }

            //blank screen
            float lerpTime = blankScreenLerpTime;
            float pauseTime = blankScreenPauseTime;
            BlankScreen.Instance.Apply(lerpTime, pauseTime, FullyBlank, FullyTransparent);
        }
        //game over
        else {
            ///TODO
        }
    }
}