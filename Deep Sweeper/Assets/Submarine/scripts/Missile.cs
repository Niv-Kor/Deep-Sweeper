using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float speed = 1;

    [SerializeField] private Vector2 randomXRange;

    [SerializeField] private Vector2 randomYRange;

    [SerializeField] private Vector2 randomZRange;

    private Vector3 startingPos;
    private Vector3 intermediateTarget, finalTarget;
    private Quaternion destRotation;
    private bool metIntermediate, metTarget, fired;
    private float lerpedTime;

    private void Awake() {
        this.metIntermediate = false;
        this.metTarget = false;
        this.fired = false;
        this.lerpedTime = 0;
    }

    private void Update() {
        if (metTarget || !fired) return;

        if (lerpedTime < speed) {
            lerpedTime += Time.deltaTime;
            Vector3 destination = metIntermediate ? finalTarget : intermediateTarget;
            transform.position = Vector3.Slerp(startingPos, destination, lerpedTime / speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, destRotation, lerpedTime / speed);
        }
        else {
            lerpedTime = 0;
            if (!metIntermediate) {
                startingPos = transform.position;
                destRotation = Quaternion.LookRotation(finalTarget);
                metIntermediate = true;
            }
            else metTarget = true;
        }
    }

    public void Fire(Transform target) {
        startingPos = transform.position;
        finalTarget = target.position;
        intermediateTarget = GenerateNearTarget(target);
        destRotation = Quaternion.LookRotation(intermediateTarget);
        fired = true;
    }

    private Vector3 GenerateNearTarget(Transform target) {
        float x = Random.Range(randomXRange.x, randomXRange.y);
        float y = Random.Range(randomYRange.x, randomYRange.y);
        float z = Random.Range(randomZRange.x, randomZRange.y);
        return target.position + new Vector3(x, y, z);
    }
}