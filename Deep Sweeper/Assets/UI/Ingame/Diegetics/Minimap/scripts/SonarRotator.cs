using DeepSweeper.CameraSet;
using System.Collections;
using UnityEngine;

public class SonarRotator : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The speed of the sonar's rotation towards the camera's rotation\n" +
             "(Insert 0 for immediate transition).")]
    [SerializeField] protected float rotationSpeed = 0;
    #endregion

    #region Constants
    protected static readonly float CHANGE_TOLERANCE = .001f;
    #endregion

    #region Class Members
    protected RectTransform rect;
    protected Transform camTransform;
    #endregion

    #region Properties
    public virtual bool Enabled {
        get { return true; }
        protected set {}
    }
    #endregion

    protected virtual void Start() {
        this.camTransform = CameraManager.Instance.GetRig(CameraRole.Main).transform;
        this.rect = GetComponent<RectTransform>();
        StartCoroutine(Rotate());
    }

    /// <summary>
    /// Calculate the next direction of the sonar.
    /// </summary>
    /// <returns>The next direction of the sonar.</returns>
    protected virtual float CalcAngle() {
        return camTransform.eulerAngles.y;
    }

    /// <summary>
    /// Rotate the sonar towards the calculated direction.
    /// </summary>
    protected virtual IEnumerator Rotate() {
        Quaternion startingRot = transform.rotation;
        Vector3 targetRot = transform.rotation.eulerAngles;

        while (true) {
            while (Enabled) {
                Vector3 rot = Vector3.forward * CalcAngle();
                Quaternion rotQuat = Quaternion.Euler(rot);

                if (rotationSpeed == 0) rect.rotation = rotQuat;
                else {
                    //check if final rotation has been changed
                    bool changed = !VectorSensitivity.EffectivelyReached(rot, targetRot, CHANGE_TOLERANCE);
                    if (changed) {
                        startingRot = transform.rotation;
                        targetRot = rot;
                    }

                    //rotate
                    float step = Time.deltaTime * rotationSpeed;
                    rect.rotation = Quaternion.Lerp(startingRot, rotQuat, step);
                }

                yield return null;
            }

            yield return null;
        }
    }
}