using Constants;
using UnityEngine;

public class MinimapNavigator : SonarRotator
{
    #region Class Members
    private Transform player;
    private GameFlow flow;

    private Vector3 pos, target;
    #endregion

    #region Properties
    public Vector3 Target { get; private set; }
    public Vector3 Direction { get; private set; }
    public float Distance { get; private set; }
    #endregion

    protected override void Start() {
        base.Start();
        this.player = CameraManager.Instance.Rig.transform;
        this.flow = GameFlow.Instance;
    }

    private void Update() {
        if (flow.DuringPhase) return;

        MineField field = flow.CurrentPhase.Field;
        pos = player.position;
        Vector3 heightVec = Vector3.up * pos.y;
        target = Vector3.Scale(field.Center, Vector3.right + Vector3.forward) + heightVec;
        Vector3 dir = target - pos;
        bool hit = Physics.Raycast(pos, dir, out RaycastHit info, Mathf.Infinity, Layers.MINE_FIELD);
        Target = hit ? target : Vector3.zero;
        Direction = hit ? dir : Vector3.zero;
        Distance = hit ? info.distance : Mathf.Infinity;
    }

    /// <inheritdoc/>
    protected override float CalcAngle() {
        print("dir: " + Direction);
        return base.CalcAngle() + Direction.x;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(pos, target);
    }
}