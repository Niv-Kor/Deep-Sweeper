using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighthouse : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Prefabs")]
    [Tooltip("The root of the spotlight at the top of the tower.")]
    [SerializeField] private Transform lightroot;

    [Tooltip("The angle at which the tower's light rotates each frame.")]
    [SerializeField] private float rotationAngle = 1;
    #endregion

    private void Update() {
        if (lightroot == null) return;
        lightroot.Rotate(Vector3.up * rotationAngle);
    }
}