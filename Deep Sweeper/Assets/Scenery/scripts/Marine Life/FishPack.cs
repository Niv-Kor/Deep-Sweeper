using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FishPack : MonoBehaviour
{
    private struct FishBorder
    {
        public LayerMask BorderLayer;
        public float SafetyDistance;
    }

    private static readonly FishBorder[] BORDERS = {
        new FishBorder {
            BorderLayer = Constants.Layers.FISH_BORDERS,
            SafetyDistance = 20f
        },
        new FishBorder {
            BorderLayer = Constants.Layers.GROUND,
            SafetyDistance = 10f
        }
    };

    #region Constants
    private static readonly int MAX_ROTATION_ATTEMPTS = 5;
    private static readonly float ALLOWD_ESCAPE_PERCENT = .3f;
    #endregion

    #region Class Members
    private List<MarineLife> members;
    private MarineLifeManager marineLifeMngr;
    private float m_yawDirection;
    #endregion

    #region Events
    public event UnityAction<float> YawDirectionChangeEvent;
    public event UnityAction<int> PackDeadEvent;
    #endregion

    #region Properties
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
    #endregion

    private void Awake() {
        this.members = new List<MarineLife>();
        this.YawDirection = GenerateYawDirection();
        this.marineLifeMngr = FindObjectOfType<MarineLifeManager>();
    }

    private void Start() {
        MarineLifeSpawnControl spawnControl = GetComponentInParent<MarineLifeSpawnControl>();
        if (spawnControl != null) spawnControl.SubmitPack(this);
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
            //kill the pack if the leader somehow managed to escape the borders
            else if (CheckBorderEscape(pos)) KillPack();
            yield return null;
        }
    }

    /// <summary>
    /// Check if a raycast hits any of the fish borders.
    /// </summary>
    /// <param name="position">Raycast source point</param>
    /// <param name="forward">Raycast direction</param>
    /// <returns>True if the raycast hit any of the borders.</returns>
    private bool CheckBorderHit(Vector3 position, Vector3 forward) {
        foreach (FishBorder border in BORDERS) {
            LayerMask layer = border.BorderLayer;
            float dist = border.SafetyDistance;
            bool hit = Physics.Raycast(position, forward * dist, dist, layer);

            if (hit) return true;
        }

        return false;
    }

    /// <summary>
    /// Check if a point is outside the defined fish borders.
    /// This function considers a constant allowed distance percent outside the borders.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool CheckBorderEscape(Vector3 position) {
        float confineRadius = marineLifeMngr.AreaRadius;
        float centerDistance = Vector3.Distance(position, marineLifeMngr.AreaCenter);
        return (centerDistance > confineRadius * (1 + ALLOWD_ESCAPE_PERCENT));
    }

    /// <summary>
    /// Generate a random horizontal direction (Y axis).
    /// </summary>
    /// <returns>A random yaw direction.</returns>
    private float GenerateYawDirection(MarineLife leader = null) {
        float yaw;
        bool hit = false;
        int attempts = MAX_ROTATION_ATTEMPTS;

        do {
            yaw = Random.Range(-180f, 180f);

            if (leader != null) {
                Vector3 pos = leader.transform.position;
                Vector3 rot = leader.transform.rotation.eulerAngles;
                Vector3 fwd = new Vector3(rot.x, yaw, rot.z);
                hit = CheckBorderHit(pos, fwd);
            }

            //kill the pack and reaspawn them if they're stuck
            if (hit && --attempts <= 0) {
                KillPack();
                break;
            }
        }
        while (hit);
        return yaw;
    }

    /// <summary>
    /// Kill the entire pack.
    /// </summary>
    private void KillPack() {
        StopAllCoroutines();

        int membersAmount = members.Count;
        foreach (MarineLife fish in members) fish.Kill();

        members.Clear();
        PackDeadEvent?.Invoke(membersAmount);
        Destroy(gameObject);
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
        fish.Spawn();
    }
}