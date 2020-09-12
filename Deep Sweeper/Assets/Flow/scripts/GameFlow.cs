using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameFlow : Singleton<GameFlow>
{
    [Header("Prefabs")]
    [Tooltip("The parent object under which all mine fields are instantiated.")]
    [SerializeField] private Transform fieldsParent;

    [Tooltip("A prefab of an object with a 'MineField' script attached to it.")]
    [SerializeField] private GameObject mineFieldPrefab;

    [Header("Phases")]
    [Tooltip("A list of mine fields.\nEach field represents a level phase.")]
    [SerializeField] private PhaseConfig[] phases;

    private static readonly string FIELD_NAME = "Field";
    private static readonly Color GIZMOS_COLOR = Color.yellow;

    private int phaseIndex;

    public List<Phase> Phases { get; private set; }
    public Phase CurrentPhase {
        get { return (phaseIndex != -1) ? Phases[phaseIndex] : null; }
    }

    private void Start() {
        this.Phases = new List<Phase>();
        this.phaseIndex = -1;
        InitFields();
        NextPhase();
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
            PhaseConfig phase = phases[i];

            //instantiate
            Transform fieldObj = Instantiate(mineFieldPrefab).transform;
            fieldObj.name = FIELD_NAME + " (" + i + ")";
            fieldObj.position = Vector3.zero;
            fieldObj.rotation = Quaternion.identity;
            fieldObj.SetParent(fieldsParent);

            //configurate
            MineField mineFieldCmp = fieldObj.GetComponent<MineField>();
            mineFieldCmp.Confine = phase.Confine;
            mineFieldCmp.MinesPercent = phase.MinesPercent;

            //append
            Phase phaseObj = new Phase(i, mineFieldCmp, phase.Gate, prevPhase);
            if (prevPhase != null) prevPhase.FollowPhase = phaseObj;
            prevPhase = phaseObj;
            Phases.Add(phaseObj);
        }
    }

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
        }
    }
}