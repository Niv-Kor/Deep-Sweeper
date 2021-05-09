using Constants;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    public class BulletWarhead : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("Hit particles prefab to instantiate.")]
        [SerializeField] private GameObject hitParticlesPrefab;
        #endregion

        #region Class Members
        private Bullet bullet;
        private ParticleSystem partSystem;
        private Transform bulletParent;
        private LayerMask damageableSurfaces;
        private List<ParticleCollisionEvent> collisionEvents;
        #endregion

        private void Start() {
            this.bullet = GetComponentInParent<Bullet>();
            this.partSystem = GetComponent<ParticleSystem>();
            this.collisionEvents = new List<ParticleCollisionEvent>();
            this.bulletParent = FXManager.Instance.gameObject.transform;
            this.damageableSurfaces = partSystem.collision.collidesWith;
        }

        private void OnParticleCollision(GameObject obj) {
            if (Layers.ContainedInMask(obj.layer, damageableSurfaces)) {
                int hits = ParticlePhysicsExtensions.GetCollisionEvents(partSystem, obj, collisionEvents);

                //instantiate the hit particles
                if (hits > 0) {
                    GameObject particlesObj = Instantiate(hitParticlesPrefab);
                    particlesObj.transform.SetParent(bulletParent);
                    particlesObj.transform.position = collisionEvents[0].intersection;
                }
            }

            //dispose bullet
            bullet.Deconstruct();
        }
    }
}