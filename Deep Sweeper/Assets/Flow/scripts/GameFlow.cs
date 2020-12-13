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
            Handles.Label(areaCenter, FIELD_NAME + " " + i);
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
            fieldObj.gameObject.layer = Layers.GetLayerValue(Layers.MINE_FIELD);

            //configurate
            MineField mineFieldCmp = fieldObj.GetComponent<MineField>();
            mineFieldCmp.DefineArea(phaseConfig.Confine);

            //append
            Phase phaseObj;
            
            if (i == 0) phaseObj = new Phase(i, mineFieldCmp, initialGate, phaseConfig);
            else phaseObj = new Phase(i, mineFieldCmp, prevPhase, phaseConfig);

            //pop a window promt when the entrance gate is crossed.
            void openPromt() {
                IngameWindowManager.Instance.Pop(PromtTypes.FieldMeta);
                phaseObj.EntranceGate.GateCrossEvent -= openPromt;
            }
            phaseObj.EntranceGate.GateCrossEvent += openPromt;
            
            if (prevPhase != null) prevPhase.FollowPhase = phaseObj;
            prevPhase = phaseObj;
            Phases.Add(phaseObj);
        }
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
                Phases[phaseIndex++].Conclude();
                DuringPhase = false;
            }
            else ++phaseIndex; //first phase

            ///Phases[phaseIndex].Initiate(); ///TODO MAYBE DELETE
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
}