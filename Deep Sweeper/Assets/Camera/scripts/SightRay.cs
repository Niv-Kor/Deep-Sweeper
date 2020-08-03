using System.Collections.Generic;
using UnityEngine;

public class SightRay : MonoBehaviour
{
    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private float maxDistance = 100f;

    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private LayerMask hitLayers;

    private MineGrid selectedMine;
    private Transform camTransform;
    private SubmarineGun gun;

    private void Start() {
        this.camTransform = CameraBase.Instance.FPCam.transform;
        this.gun = FindObjectOfType<SubmarineGun>();
    }

    private void Update() {
        CastRay();

        bool mouseRight = Input.GetMouseButtonDown(1);
        bool mouseLeft = Input.GetMouseButtonDown(0);

        if (mouseLeft) gun.Fire();

        if (selectedMine != null) {
            if (mouseRight) selectedMine.ToggleFlag();
            if (mouseLeft) {
                selectedMine.IsFlagged = false;
                selectedMine.Reveal(true);
                selectedMine = null;
                Crosshair.Instance.Release();
            }
        }
    }

    /// <summary>
    /// Cast a ray at mines to select them.
    /// </summary>
    private void CastRay() {
        Vector3 origin = camTransform.position;
        Vector3 direction = camTransform.forward;
        bool hit = Physics.Raycast(origin, direction, out RaycastHit raycastHit, maxDistance, hitLayers);

        if (hit) {
            GameObject mineObj = raycastHit.collider.gameObject;

            if (selectedMine == null) SelectMine(mineObj);
            else if (selectedMine != mineObj) SelectMine(mineObj);
        }
        else if (selectedMine != null) {
            selectedMine = null;
            Crosshair.Instance.Release();
        }
    }

    /// <summary>
    /// Select a mine object.
    /// </summary>
    /// <param name="mine">The object to select</param>
    private void SelectMine(GameObject mine) {
        selectedMine = mine.GetComponentInParent<MineGrid>();
        Crosshair.Instance.Lock();
    }
}
