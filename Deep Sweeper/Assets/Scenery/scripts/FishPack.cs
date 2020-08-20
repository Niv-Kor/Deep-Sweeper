using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPack : MonoBehaviour
{
    private static readonly float MAX_RAY_DISTANCE = 40f;
    private static readonly LayerMask BORDERS_MASK_LAYER = Constants.Layers.FISH_BORDERS;

    private List<MarineLife> members;
    private float m_yawDirection;

    public delegate void YawDirectionChange(float value);
    public event YawDirectionChange YawDirectionChangeEvent;

    public float YawDirection {
        get { return m_yawDirection; }
        set {
            m_yawDirection = value;
            YawDirectionChangeEvent?.Invoke(value);
        }
    }
    public MarineLife Leader {
        get {
            if (members.Count > 0) return members[0];
            else return null;
        }
    }

    private void Awake() {
        this.members = new List<MarineLife>();
        this.YawDirection = GenerateYawDirection();
    }

    /// <summary>
    /// Check the leader's surroundings and make it turn
    /// when ever it encounters a border.
    /// </summary>
    /// <param name="leader">The leader of the pack</param>
    private IEnumerator TrackLeader(MarineLife leader) {
        SkinnedMeshRenderer mesh = leader.GetComponentInChildren<SkinnedMeshRenderer>();
        float torso = mesh.bounds.extents.y;

        while (leader != null) {
            Vector3 fwd = leader.transform.forward;
            Vector3 pos = leader.transform.position + fwd * torso;

            //encounter border
            if (CheckBorderHit(pos, fwd)) {
                YawDirection = GenerateYawDirection(leader);
                break;
            }

            yield return null;
        }
    }

    private void OnDrawGizmos() {
        if (Leader != null) {
            SkinnedMeshRenderer mesh = Leader.GetComponentInChildren<SkinnedMeshRenderer>();
            float torso = mesh.bounds.extents.y;
            Gizmos.color = Color.green;
            Gizmos.DrawCube(Leader.transform.position, Vector3.one * 1f);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(Leader.transform.position + Leader.transform.forward * torso, Vector3.one * 1f);

            Gizmos.color = Color.white;
            Vector3 pos = Leader.transform.position + Leader.transform.forward * torso;
            Vector3 dir = Leader.transform.forward * MAX_RAY_DISTANCE;
            Gizmos.DrawRay(pos, dir);
        }
    }

    /// <summary>
    /// Check if a raycast hits any of the fish borders.
    /// </summary>
    /// <param name="position">Raycast source point</param>
    /// <param name="forward">Raycast direction</param>
    /// <returns>True if the raycast hit any of the borders.</returns>
    private bool CheckBorderHit(Vector3 position, Vector3 forward) {
        return Physics.Raycast(position, forward * MAX_RAY_DISTANCE, MAX_RAY_DISTANCE, BORDERS_MASK_LAYER);
    }

    /// <summary>
    /// Generate a random horizontal direction (Y axis).
    /// </summary>
    /// <returns>A random yaw direction.</returns>
    private float GenerateYawDirection(MarineLife leader = null) {
        float yaw;
        bool hit = false;

        do {
            yaw = Random.Range(-180f, 180f);

            if (leader != null) {
                Vector3 pos = leader.transform.position;
                Vector3 rot = leader.transform.rotation.eulerAngles;
                Vector3 fwd = new Vector3(rot.x, yaw, rot.z);
                hit = CheckBorderHit(pos, fwd);
            }
        }
        while (hit);
        return yaw;
    }

    /// <summary>
    /// Activate when the leader of the pack finishes turning.
    /// </summary>
    /// <param name="leader">The pack's leader fish</param>
    private void OnLeaderFinishTurning(MarineLife leader) {
        StopAllCoroutines();
        StartCoroutine(TrackLeader(leader));
    }

    /// <summary>
    /// Append a fish to the pack.
    /// </summary>
    /// <param name="fish">The fish to join</param>
    /// <param name="leader">True if the fish is the leader of the pack</param>
    public void Join(MarineLife fish, bool leader = false) {
        if (leader) {
            //remove trigger from previous leader
            if (Leader != null) Leader.FishTurnEvent -= OnLeaderFinishTurning;

            members.Insert(0, fish);
            fish.FishTurnEvent += OnLeaderFinishTurning;
            StopAllCoroutines();
            StartCoroutine(TrackLeader(fish));
        }
        else members.Add(fish);
    }
}