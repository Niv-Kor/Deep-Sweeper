using System.Collections;
using System.Collections.Generic;
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

    private int emissionAmount;
    private FishPack pack;
    private Queue<MarineLife> collectedPack;

    protected override void ApplyEmission(GameObject instance, int emission) {
        //spawn tools initialization
        this.emissionAmount = emission;
        this.collectedPack = new Queue<MarineLife>();

        //insert the source instance to a lower level parent
        GameObject subParent = new GameObject(RemoveCloneTag(instance.name));
        MarineLife marineLifeComponent = instance.GetComponent<MarineLife>();
        subParent.transform.SetParent(instance.transform.parent);

        //establish pack connection
        pack = subParent.AddComponent<FishPack>();
        pack.PackDeadEvent += OnPackDies;

        //emit leader
        instance.transform.SetParent(subParent.transform);
        instance.name = RemoveCloneTag(instance.name) + "_" + INSTANCE_NAME_TAG;
        OnEmissionFinish(marineLifeComponent);

        //emit pack members
        for (int i = 0; i < emission - 1; i++)
            StartCoroutine(Emit(instance, subParent, 1, pack));
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

    /// <summary>
    /// Activate when the pack members are dead.
    /// This function respawns the pack.
    /// </summary>
    /// <param name="members">Amount of members in the dead pack</param>
    private void OnPackDies(int members) {
        base.Spawn(members);
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

        //report emission finish
        OnEmissionFinish(marineLifeComponent);
    }

    /// <summary>
    /// Activate when each emission process is done.
    /// When all emissions are done, this function joins together all marine lives into one pack.
    /// </summary>
    /// <param name="spawn"></param>
    private void OnEmissionFinish(MarineLife spawn) {
        collectedPack.Enqueue(spawn);

        //all emissions are done
        if (--emissionAmount == 0) {
            bool joinedLeader = false;

            while (collectedPack.Count > 0) {
                pack.Join(collectedPack.Dequeue(), !joinedLeader);
                joinedLeader = true;
            }
        }
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
}