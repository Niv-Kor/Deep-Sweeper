using Constants;
using DeepSweeper.Camera;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Radar : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The radius of the radar.")]
    [SerializeField] private float radius;
    #endregion

    #region Constants
    private static readonly Color GIZMOS_COLOR = Color.green;
    private static readonly int MAX_COLLISIONS = 64;
    #endregion

    #region Class Members
    private Collider[] colRes;
    private List<Collider> prevRes;
    #endregion

    private void Start() {
        this.colRes = new Collider[MAX_COLLISIONS];
        this.prevRes = new List<Collider>();
    }

    private void Update() {
        int results = Physics.OverlapSphereNonAlloc(transform.position, radius,
                                                    colRes, Layers.MINE_INDICATION);

        //activate indicators that are visible by the radar
        for (int i = 0; i < results; i++) {
            Collider col = colRes[i];
            if (col == null) break;

            Indicator indicator = col.GetComponent<Indicator>();
            float dist = CalcDistanceFromRay(col.transform.position);
            float alpha = 1 - dist / radius;
            indicator?.Display(alpha);
        }

        //find previous result's symmetric difference
        List<Collider> colList = colRes.ToList();
        List<Collider> symmerticDiff = prevRes.Except(colList).Union(colList.Except(prevRes)).ToList();
        symmerticDiff = symmerticDiff.FindAll(x => x != null);

        //deactivate indicators that are no longer visible by the radar
        foreach (Collider col in symmerticDiff) {
            Indicator indicator = col.GetComponent<Indicator>();
            indicator?.Display(0);
        }

        prevRes.Clear();
        prevRes.AddRange(colList);
    }

    private void OnDrawGizmos() {
        Gizmos.color = GIZMOS_COLOR;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    /// <summary>
    /// Calculate the shortest distance of the point from the entire radar's line ray.
    /// </summary>
    /// <param name="point">The point to check</param>
    /// <returns>The distance of a point from the radar's line ray.</returns>
    private float CalcDistanceFromRay(Vector3 point) {
        Vector3 dir = IngameCameraManager.Instance.FPCam.transform.forward;
        Vector3 pos = transform.position - dir * radius;
        return Vector3.Cross(dir, point - pos).magnitude;
    }
}