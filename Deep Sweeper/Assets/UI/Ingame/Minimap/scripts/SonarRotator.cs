using System.Collections;
using UnityEngine;

public class SonarRotator : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The speed of the sonar's rotation towards the camera's rotation\n" +
             "(Insert 0 for immediate transition).")]
    [SerializeField] private float rotationSpeed = 0;
    #endregion

    #region Constants
    private static readonly float CHANGE_TOLERANCE = .1f;
    #endregion

    #region Class Members
    private RectTransform rect;
    private Transform camTransform;
    #endregion

    private void Start() {
        this.camTransform = CameraManager.Instance.Rig.transform;
        this.rect = GetComponent<RectTransform>();
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate() {
        Quaternion startingRot = transform.rotation;
        Vector3 targetRot = transform.rotation.eulerAngles;

        while (true) {
            Vector3 camRot = Vector3.forward * camTransform.eulerAngles.y;
            Quaternion camRotQuat = Quaternion.Euler(camRot);

            if (rotationSpeed == 0) rect.rotation = camRotQuat;
            else {
                //check if final cam setting has been changed
                bool changed = !VectorSensitivity.EffectivelyReached(camRot, targetRot, CHANGE_TOLERANCE);
                if (changed) {
                    startingRot = transform.rotation;
                    targetRot = camRot;
                }

                //rotate
                float step = Time.deltaTime * rotationSpeed;
                rect.rotation = Quaternion.Lerp(startingRot, camRotQuat, step);
            }

            yield return null;
        }
    }
}