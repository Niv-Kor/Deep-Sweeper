using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    protected static readonly float X_ROTATION = 90;
    protected static readonly float Z_ROTATION = 0;

    protected Vector3 flatMask;

    public MinimapIconEvents.YawAngle YawAngleFunc { get; set; }

    protected virtual void Start() {
        this.flatMask = new Vector3(X_ROTATION, 0, Z_ROTATION);
    }

    protected virtual void Update() {
        float angle = (YawAngleFunc != null) ? YawAngleFunc.Invoke() : 0;
        Flatten(angle);
    }

    /// <summary>
    /// Rotate the icon towards a certain direction, while flattenning it.
    /// </summary>
    /// <param name="yawAngle">The yaw angle into which the icon will rotate</param>
    protected virtual void Flatten(float yawAngle) {
        Vector3 yaw = Vector3.up * (yawAngle);
        transform.rotation = Quaternion.Euler(flatMask + yaw);
    }
}
