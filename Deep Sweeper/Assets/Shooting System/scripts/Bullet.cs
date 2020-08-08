using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Tooltip("Hit particles prefab to instantiate.")]
    [SerializeField] private GameObject hitParticlesPrefab;

    private static readonly string BULLETS_PARENT_NAME = "Particles";

    private GameObject bulletsParent;
    private List<ParticleCollisionEvent> collisionEvents;

    private void Start() {
        this.collisionEvents = new List<ParticleCollisionEvent>();
        this.bulletsParent = new GameObject(BULLETS_PARENT_NAME);
        bulletsParent.transform.SetParent(transform);
    }

    private void OnParticleCollision(GameObject other) {
        int hits = ParticlePhysicsExtensions.GetCollisionEvents(GetComponent<ParticleSystem>(), other, collisionEvents);

        //instantiate the hit particles
        if (hits > 0) {
            GameObject particlesObj = Instantiate(hitParticlesPrefab);
            particlesObj.transform.SetParent(bulletsParent.transform);
            particlesObj.transform.position = collisionEvents[0].intersection;
        }
    }
}