using DeepSweeper.CameraSet;
using DeepSweeper.Player;
using DeepSweeper.Player.ShootingSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeepSweeper.Level.Mine
{
    public class DetonationSystem : MineSystem
    {
        #region Exposed Editor Parameters
        [Header("Shake")]
        [Tooltip("The intensity at which the camera will shake each time a non-fatal mine detonates.")]
        [SerializeField] [Range(0f, 1f)] private float cameraShakeIntensity = 1;
        #endregion

        #region Class Members
        private SphereCollider col;
        private MeshRenderer[] renders;
        private SensorsManager sensors;
        private Bullet hitTrigger;
        #endregion

        #region Events
        public event UnityAction DetonationEvent;
        #endregion

        #region Properties
        public bool IsDetonated { get; private set; }
        #endregion

        private void Awake() {
            GameObject avatar = Grid.Avatar;
            this.sensors = avatar.GetComponentInChildren<SensorsManager>();
            this.renders = avatar.GetComponentsInChildren<MeshRenderer>();
            this.col = avatar.GetComponentInChildren<SphereCollider>();
            this.IsDetonated = false;
        }

        private void Start() {
            //assign an event trigger to the main camera
            DetonationEvent += ShakeCamera;
        }

        /// <summary>
        /// Shake the camera as an explosion effect.
        /// </summary>
        /// <param name="maxForce">
        /// True if the camera should shake at max force,
        /// or false if it should be relative to the mine's distance
        /// </param>
        private void ShakeCamera() {
            CameraShaker camShaker = IngameCameraManager.Instance.FPCam.GetComponent<CameraShaker>();
            SightRay ray = SightRay.Instance;

            if (camShaker != null) {
                float shakeStrength = cameraShakeIntensity;

                //relative to distance from mine
                if (!Grid.IndicationSystem.IsFatal) {
                    Transform player = Submarine.Instance.transform;
                    float dist = Vector3.Distance(transform.position, player.position);
                    float clampedDist = Mathf.Clamp(dist, 0, ray.MaxDistance);
                    shakeStrength = 1 - RangeMath.NumberOfRange(clampedDist, 0, ray.MaxDistance);
                }

                camShaker.Shake(shakeStrength);
            }
        }

        /// <summary>
        /// Vanish the mine.
        /// </summary>
        /// <param name="explosion">True to explode the mine using particle effects</param>
        private void Dismiss(bool explosion) {
            if (IsDetonated) return;

            DetonationEvent?.Invoke();
            foreach (var render in renders) render.enabled = false;
            col.enabled = false;
            IsDetonated = true;

            ///TODO 1f
            sensors.BreakSensors(1f, explosion);
        }

        /// <summary>
        /// Explode the mine.
        /// </summary>
        public void Explode() { Dismiss(true); }

        /// <summary>
        /// Quietly vanish the mine without explision.
        /// </summary>
        public void Vanish() { Dismiss(false); }

        /// <summary>
        /// Explode the mine.
        /// </summary>
        /// <param name="explosion">True to activate an explosion effect on revelation</param>
        /// <param name="allowDrop">True to allow the mine to drop an item</param>
        private void Detonate(bool explosion, bool allowDrop = true) {
            bool ignored = Grid.SelectionSystem.IsFlagged;
            if (Grid.IndicationSystem.IsRevealed || ignored) return;

            //lose
            if (Grid.IndicationSystem.IsFatal) {
                Dismiss(true);
                DeathTint.Instance.Tint();
                LevelFlow.Instance.Lose();
            }
            else {
                Grid.IndicationSystem.AllowRevelation(true);
                Grid.IndicationSystem.Activate();
                Grid.Activator.ActivateAndLock();
                Grid.SelectionSystem.ApplyFlag(false);

                int neighbours = Grid.IndicationSystem.Value;
                List<MineGrid> section = Grid.Section;

                if (!allowDrop) Grid.LootGenerator.Chance = 0;
                if (explosion) Dismiss(true);
                else Dismiss(false);

                //keep revealing grids recursively
                if (neighbours == 0)
                    foreach (MineGrid mineGrid in section)
                        if (mineGrid != null)
                            mineGrid.DetonationSystem.TriggerHit(null, explosion, allowDrop);

                LevelFlow.Instance.TryNextPhase();
            }
        }

        /// <summary>
        /// Trigger the mine's hit.
        /// A hit of type 'SingleHit' means that the mine itself has been hit with a bullet,
        /// while a hit type of 'SectionHit' means that the mine's indicator has been hit,
        /// and the bullet is meant for each of the mine's neighbours.
        /// </summary>
        /// <param name="hitType">The type of hit that occured</param>
        /// <param name="explosion">True to activate an explosion effect on revelation</param>
        /// <param name="allowDrop">True to allow the mine to drop an item</param>
        public void TriggerHit(Bullet bullet, bool explosion, bool allowDrop = true) {
            if (bullet != null && hitTrigger == bullet) return;
            else hitTrigger = bullet;

            Detonate(explosion, allowDrop);
        }
    }
}