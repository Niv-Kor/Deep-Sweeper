using UnityEngine;

public class DirectionUnit : Singleton<DirectionUnit>
{
    [Tooltip("True to freeze rotation around the x axis (pitch).")]
    [SerializeField] private bool freezeX;

    [Tooltip("True to freeze rotation around the y axis (yaw).")]
    [SerializeField] private bool freezeY;

    [Tooltip("True to freeze rotation around the z axis (roll).")]
    [SerializeField] private bool freezeZ;

    void Update() {
        Vector3 currentRot = transform.rotation.eulerAngles;
        float newPitch = freezeX ? 0 : currentRot.x;
        float newYaw = freezeY ? 0 : currentRot.y;
        float newRoll = freezeZ ? 0 : currentRot.z;
        Vector3 newAngles = new Vector3(newPitch, newYaw, newRoll);
        transform.rotation = Quaternion.Euler(newAngles);
    }
}