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
        private bool enableDetonationCallback;
        private bool enableDrop;
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
            this.enableDetonationCallback = true;
            this.IsDetonated = false;

            DetonationEvent += ShakeCamera;
            sensors.AllSensorsBrokenEvent += OnMineDetonated;
        }

        /// <summary>
        /// Activate when the mine is detonated completely.
        /// </summary>
        private void OnMineDetonated() {
            bool fatal = Grid.IndicationSystem.IsFatal;

            //dismiss grid
            foreach (var render in renders) render.enabled = false;
            col.enabled = false;
            IsDetonated = true;

            //always send callback if mine is fatal
            enableDetonationCallback |= fatal;

            if (!fatal) {
                Grid.IndicationSystem.AllowRevelation(true);
                Grid.IndicationSystem.Activate();
                Grid.Activator.ActivateAndLock();
                Grid.SelectionSystem.ApplyFlag(false);
                Grid.LootGenerator.PreventDrop = !enableDrop;

                int neighbours = Grid.IndicationSystem.Value;
                List<MineGrid> section = Grid.Section;

                //keep detonating neighbour grids recursively
                if (neighbours == 0)
                    foreach (MineGrid mineGrid in section)
                        if (mineGrid != null)
                            mineGrid.DetonationSystem.TriggerHit(null, enableDetonationCallback, enableDrop);

                //try winning the phase
                LevelFlow.Instance.TryNextPhase();
            }

            if (enableDetonationCallback) DetonationEvent?.Invoke();

            //lose
            if (fatal) {
                DeathTint.Instance.Tint();
                LevelFlow.Instance.Lose();
            }
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
        /// Hit the mine.
        /// </summary>
        /// <param name="power">The power of the hit [0:1]</param>
        private void Hit(float power) {
            if (!IsDetonated) sensors.BreakSensors(power);
        }

        /// <summary>
        /// Explode the mine.
        /// </summary>
        /// <param name="power">The power of the hit [0:1]</param>
        /// <param name="sendDetonationCallback">True to send a callback upon detonation</param>
        /// <param name="allowDrop">True to allow the mine to drop an item</param>
        private void Detonate(float power, bool sendDetonationCallback, bool allowDrop = true) {
            bool ignored = Grid.SelectionSystem.IsFlagged;
            if (Grid.IndicationSystem.IsRevealed || ignored) return;

            bool fatal = Grid.IndicationSystem.IsFatal;
            enableDetonationCallback = sendDetonationCallback || fatal;
            enableDrop = allowDrop;
            Hit(power);
        }

        /// <summary>
        /// Trigger the mine's hit.
        /// A hit of type 'SingleHit' means that the mine itself has been hit with a bullet,
        /// while a hit type of 'SectionHit' means that the mine's indicator has been hit,
        /// and the bullet is meant for each of the mine's neighbours.
        /// </summary>
        /// <param name="bullet">The hitting bullet</param>
        /// <param name="sendDetonationCallback">True to send a callback upon detonation</param>
        /// <param name="allowDrop">True to allow the mine to drop an item</param>
        public void TriggerHit(Bullet bullet, bool sendDetonationCallback, bool allowDrop = true) {
            if (bullet != null && hitTrigger == bullet) return;
            else hitTrigger = bullet;

            float power = (bullet != null) ? bullet.Power : 1;
            Detonate(power, sendDetonationCallback, allowDrop);
        }
    }
}