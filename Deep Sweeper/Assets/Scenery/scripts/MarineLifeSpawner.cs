using System.Collections;
using UnityEngine;

public class MarineLifeSpawner : MarineSpawner
{
    [Header("Emission Settings")]
    [Tooltip("The minimum 3D distance between each individual from the center instance.")]
    [SerializeField] private Vector3 minIndividualDistance;

    [Tooltip("The maximum 3D distance between each individual from the center instance.")]
    [SerializeField] private Vector3 maxIndividualDistance;

    private static readonly string INSTANCE_NAME_TAG = "instance";

    protected override void ApplyEmission(GameObject instance, int emission) {
        //insert the source instance to a lower level parent
        GameObject subParent = new GameObject(RemoveCloneTag(instance.name));
        subParent.transform.SetParent(instance.transform.parent);
        instance.transform.SetParent(subParent.transform);
        instance.name = RemoveCloneTag(instance.name) + "_" + INSTANCE_NAME_TAG;

        //emit
        for (int i = 0; i < emission - 1; i++)
            StartCoroutine(Emit(instance, subParent, 1));
    }

    /// <summary>
    /// Emit a new instance after a random delay.
    /// </summary>
    /// <param name="source">Source prefab to intantiate</param>
    /// <param name="subParent">The parent of the new emitted object</param>
    /// <param name="maxDelayTime">Maximum amount of time [s] until the emission</param>
    private IEnumerator Emit(GameObject source, GameObject subParent, float maxDelayTime) {
        //wait for x seconds
        float delay = Random.Range(0, maxDelayTime);
        yield return new WaitForSeconds(delay);

        //instantiate
        GameObject instance = Instantiate(source);
        instance.transform.position = RandomizeIndividualDistance(instance.transform.position);
        instance.transform.SetParent(subParent.transform);
        instance.name = RemoveCloneTag(instance.name);
    }

    /// <summary>
    /// Generate a random 3D distance from a point.
    /// </summary>
    /// <param name="point">The point from which to measure the generated distance</param>
    /// <returns></returns>
    private Vector3 RandomizeIndividualDistance(Vector3 point) {
        float x = Random.Range(minIndividualDistance.x, maxIndividualDistance.x);
        float y = Random.Range(minIndividualDistance.y, maxIndividualDistance.y);
        float z = Random.Range(minIndividualDistance.z, maxIndividualDistance.z);
        return point + new Vector3(x, y, z);
    }

    /// <summary>
    /// Remove the '(Clone)' suffix of a string.
    /// </summary>
    /// <param name="name">A string from which to remove the suffix</param>
    /// <returns>The string without the '(Clone)' suffix.</returns>
    private string RemoveCloneTag(string name) {
        return name.Replace("(Clone)", "");
    }
}