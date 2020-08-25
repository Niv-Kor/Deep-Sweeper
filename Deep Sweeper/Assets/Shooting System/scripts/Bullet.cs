using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Tooltip("Hit particles prefab to instantiate.")]
    [SerializeField] private GameObject hitParticlesPrefab;

    private Transform bulletsParent;
    private LayerMask damageableSurfaces;
    private List<ParticleCollisionEvent> collisionEvents;

    private void Start() {
        ParticleSystem partSystem = GetComponent<ParticleSystem>();
        this.collisionEvents = new List<ParticleCollisionEvent>();
        this.bulletsParent = FXManager.Instance.gameObject.transform;
        this.damageableSurfaces = partSystem.collision.collidesWith;
    }

    private void OnParticleCollision(GameObject obj) {
        if (!Constants.Layers.ContainedInMask(obj.layer, damageableSurfaces)) return;
        int hits = ParticlePhysicsExtensions.GetCollisionEvents(GetComponent<ParticleSystem>(), obj, collisionEvents);

        //instantiate the hit particles
        if (hits > 0) {
            GameObject particlesObj = Instantiate(hitParticlesPrefab);
            particlesObj.transform.SetParent(bulletsParent);
            particlesObj.transform.position = collisionEvents[0].intersection;
        }
    }
}