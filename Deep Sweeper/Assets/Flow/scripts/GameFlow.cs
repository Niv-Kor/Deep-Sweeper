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
    public event UnityAction<int> PhaseUpdatedEvent;
    #endregion

    #region Properties
    public List<Phase> Phases { get; private set; }
    public Phase CurrentPhase {
        get { return (phaseIndex != -1) ? Phases[phaseIndex] : null; }
    }

    public string RegionName {
        get { return region.ToString().Replace('_', ' '); }
    }
    #endregion

    private void Awake() {
        this.Phases = new List<Phase>();
        this.phaseIndex = -1;
        initialGate.RequestOpen(false);
        InitFields();
        NextPhase();
    }

    private void OnValidate() {
        //check that each phase config contains the entire difficulty configurations set
        for (int i = 0; i < phases.Length; i++) {
            PhaseConfig phaseConfig = phases[i];

            if (phaseConfig.Levels.Count != 3) {
                List<DifficultyConfig> configList = new List<DifficultyConfig>();
                var levels = Enum.GetValues(typeof(DifficultyLevel));

                foreach (DifficultyLevel level in levels) {
                    DifficultyConfig config = new DifficultyConfig {
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
    private void InitFields() {
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
            mineFieldCmp.Confine = phaseConfig.Confine;
            /// =====================================
            mineFieldCmp.MinesPercent = 20; ///TODO
            /// =====================================

            //append
            Phase phaseObj;
            
            if (i == 0) phaseObj = new Phase(i, mineFieldCmp, initialGate, phaseConfig);
            else phaseObj = new Phase(i, mineFieldCmp, prevPhase, phaseConfig);
            
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
            if (phaseIndex >= 0) Phases[phaseIndex++].Conclude();
            else ++phaseIndex; //first phase

            Phases[phaseIndex].InitiateWhenReady();
            PhaseChangeEvent?.Invoke(phaseIndex);
        }
    }

    /// <summary>
    /// Report an initialized phase that finished updating.
    /// </summary>
    public void ReportPhaseUpdated() {
        PhaseUpdatedEvent?.Invoke(phaseIndex);
    }
}