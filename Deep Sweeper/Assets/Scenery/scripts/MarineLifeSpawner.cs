using System.Collections;
using UnityEngine;

public class MarineLifeSpawner : MarineSpawner
{
    [Header("Emission Settings")]
    [Tooltip("The minimum 3D distance between each individual from the center instance.")]
    [SerializeField] public Vector3 MinIndividualDistance;

    [Tooltip("The maximum 3D distance between each individual from the center instance.")]
    [SerializeField] public Vector3 MaxIndividualDistance;

    private static readonly string INSTANCE_NAME_TAG = "instance";
    private static readonly string CLONE_TAG = "(Clone)";
    private static readonly float MAX_SPAWN_DISTANCE_OF_RAD = .7f;

    protected override void ApplyEmission(GameObject instance, int emission) {
        //insert the source instance to a lower level parent
        GameObject subParent = new GameObject(RemoveCloneTag(instance.name));
        MarineLife marineLifeComponent = instance.GetComponent<MarineLife>();
        FishPack pack = subParent.AddComponent<FishPack>();
        subParent.transform.SetParent(instance.transform.parent);
        instance.transform.SetParent(subParent.transform);
        instance.name = RemoveCloneTag(instance.name) + "_" + INSTANCE_NAME_TAG;
        pack.Join(marineLifeComponent, true);

        //emit
        for (int i = 0; i < emission - 1; i++)
            StartCoroutine(Emit(instance, subParent, 1, pack));
    }

    /// <summary>
    /// Emit a new instance after a random delay.
    /// </summary>
    /// <param name="source">Source prefab to intantiate</param>
    /// <param name="subParent">The parent of the new emitted object</param>
    /// <param name="maxDelayTime">Maximum amount of time [s] until the emission</param>
    /// <param name="pack">The marine life's intended pack</param>
    private IEnumerator Emit(GameObject source, GameObject subParent, float maxDelayTime, FishPack pack) {
        //wait for x seconds
        float delay = Random.Range(0, maxDelayTime);
        yield return new WaitForSeconds(delay);

        //instantiate
        GameObject instance = Instantiate(source);
        MarineLife marineLifeComponent = instance.GetComponent<MarineLife>();
        instance.transform.SetParent(subParent.transform);
        instance.transform.position = GenerateIndividualDistance(instance.transform.position);
        instance.name = RemoveCloneTag(instance.name);
        pack.Join(marineLifeComponent);
    }

    /// <summary>
    /// Generate a random 3D distance from a point.
    /// </summary>
    /// <param name="point">The point from which to measure the generated distance</param>
    /// <returns></returns>
    private Vector3 GenerateIndividualDistance(Vector3 point) {
        float x = Random.Range(MinIndividualDistance.x, MaxIndividualDistance.x);
        float y = Random.Range(MinIndividualDistance.y, MaxIndividualDistance.y);
        float z = Random.Range(MinIndividualDistance.z, MaxIndividualDistance.z);
        return point + new Vector3(x, y, z);
    }

    /// <summary>
    /// Remove the '(Clone)' suffix of a string.
    /// </summary>
    /// <param name="name">A string from which to remove the suffix</param>
    /// <returns>The string without the '(Clone)' suffix.</returns>
    private string RemoveCloneTag(string name) {
        return name.Replace(CLONE_TAG, string.Empty);
    }

    protected override Vector3 GeneratePosition() {
        Vector3 pos;
        float centerDist;

        //don't generate a position that's too close to the edges
        do {
            pos = base.GeneratePosition();
            centerDist = Vector3.Distance(pos, AreaCenter);
        }
        while (centerDist > AreaRadius * MAX_SPAWN_DISTANCE_OF_RAD);
        return pos;
    }
}