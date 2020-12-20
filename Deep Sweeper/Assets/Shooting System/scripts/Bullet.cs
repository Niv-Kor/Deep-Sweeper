using Constants;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("Hit particles prefab to instantiate.")]
    [SerializeField] private GameObject hitParticlesPrefab;

    [Tooltip("The time (in seconds) it takes the bullet to be inactive since it was fired, "
           + "if it didn't hit a target beforehand.")]
    [SerializeField] private float timeToLive = 1;
    #endregion

    #region Constants
    private static readonly float MIN_TTL = .1f;
    #endregion

    #region Class Members
    private Transform bulletsParent;
    private LayerMask damageableSurfaces;
    private List<ParticleCollisionEvent> collisionEvents;
    private float ttlTimer;
    #endregion

    #region Properties
    public bool ActiveBullet { get { return ttlTimer < timeToLive; } }
    #endregion

    private void Start() {
        ParticleSystem partSystem = GetComponent<ParticleSystem>();
        this.collisionEvents = new List<ParticleCollisionEvent>();
        this.bulletsParent = FXManager.Instance.gameObject.transform;
        this.damageableSurfaces = partSystem.collision.collidesWith;
        this.ttlTimer = 0;
    }

    private void OnValidate() {
        timeToLive = Mathf.Max(timeToLive, MIN_TTL);
    }

    private void Update() {
        ttlTimer += Time.deltaTime;
    }

    private void OnParticleCollision(GameObject obj) {
        if (Layers.ContainedInMask(obj.layer, damageableSurfaces)) {
            ParticleSystem partSystem = GetComponent<ParticleSystem>();
            int hits = ParticlePhysicsExtensions.GetCollisionEvents(partSystem, obj, collisionEvents);

            //instantiate the hit particles
            if (hits > 0) {
                GameObject particlesObj = Instantiate(hitParticlesPrefab);
                particlesObj.transform.SetParent(bulletsParent);
                particlesObj.transform.position = collisionEvents[0].intersection;
            }
        }

        //dispose bullet
        Destroy(gameObject);
    }
}