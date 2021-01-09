﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarineLifeSpawner : MarineSpawner
{
    #region Exposed Editor Parameters
    [Header("Emission Settings")]
    [Tooltip("The minimum 3D distance between each individual from the center instance.")]
    [SerializeField] public Vector3 MinIndividualDistance;

    [Tooltip("The maximum 3D distance between each individual from the center instance.")]
    [SerializeField] public Vector3 MaxIndividualDistance;
    #endregion

    #region Constants
    private static readonly string INSTANCE_NAME_TAG = "instance";
    private static readonly string CLONE_TAG = "(Clone)";
    private static readonly float MAX_SPAWN_DISTANCE_OF_RAD = .7f;
    private static readonly float MAX_EMISSION_DELAY_TIME = 1;
    #endregion

    #region Class Members
    private int emissionAmount;
    private FishPack pack;
    private Queue<MarineLife> collectedPack;
    #endregion

    /// <inheritdoc/>
    protected override void ApplyEmission(GameObject instance, int emission) {
        //spawn tools initialization
        this.emissionAmount = emission;
        this.collectedPack = new Queue<MarineLife>();

        //insert the source instance to a lower level parent
        GameObject subParent = new GameObject(RemoveCloneTag(instance.name));
        MarineLife marineLifeCmp = instance.GetComponent<MarineLife>();
        subParent.transform.SetParent(instance.transform.parent);

        //establish pack connection
        this.pack = subParent.AddComponent<FishPack>();
        pack.PackDeadEvent += OnPackDies;

        //emit pack members
        for (int i = 0; i < emission - 1; i++)
            StartCoroutine(Emit(instance, subParent, MAX_EMISSION_DELAY_TIME));

        //emit leader
        instance.transform.SetParent(subParent.transform);
        instance.name = RemoveCloneTag(instance.name) + "_" + INSTANCE_NAME_TAG;
        OnEmissionFinish(marineLifeCmp);
    }

    /// <inheritdoc/>
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
    private void OnPackDies(int _ = 0) {
        base.Spawn(1);
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
    /// <param name="spawn">The spawn that had been successfully emitted.</param>
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
    /// <returns>A random 3D distance from the specified point.</returns>
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