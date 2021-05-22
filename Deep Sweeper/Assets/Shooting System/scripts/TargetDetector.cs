using Constants;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    public class TargetDetector : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The bullet to which this detector belongs.")]
        [SerializeField] private Bullet bullet;
        #endregion

        #region Class Members
        private ParticleSystem partSystem;
        private List<ParticleCollisionEvent> collisionEvents;
        #endregion

        #region Properties
        public Bullet Bullet => bullet;
        public LayerMask ImpactSurface { get; private set; }
        #endregion

        private void Awake() {
            this.partSystem = GetComponent<ParticleSystem>();
            this.collisionEvents = new List<ParticleCollisionEvent>();
            this.ImpactSurface = partSystem.collision.collidesWith;
        }

        private void OnParticleCollision(GameObject obj) {
            if (Layers.ContainedInMask(obj.layer, ImpactSurface)) {
                int hits = ParticlePhysicsExtensions.GetCollisionEvents(partSystem, obj, collisionEvents);

                if (hits > 0) {
                    Vector3 position = collisionEvents[0].intersection;
                    Vector3 rotation = collisionEvents[0].normal;
                    bullet.ReportHit(position, rotation);
                }
            }
        }
    }
}