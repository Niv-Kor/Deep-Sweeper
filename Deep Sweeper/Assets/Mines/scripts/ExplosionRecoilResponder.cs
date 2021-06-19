using DeepSweeper.Flow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Level.Mine
{
    [RequireComponent(typeof(Rigidbody))]
    public class ExplosionRecoilResponder : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Force")]
        [Tooltip("The minimum and maximum force being applied to the object's rigidbody upon mine destruction.\n"
               + "The minimum force is applied when the object is far away from the exploding mine, "
               + "and the maximum force is applied when it's close.")]
        [SerializeField] private Vector2 forceRange;

        [Tooltip("The minimum and maximum distance from the exploding "
               + "mine at which the object will be pushed back.")]
        [SerializeField] private Vector2 distanceRange;

        [Header("Torque")]
        [Tooltip("The range of the allowed pitch rotation (X axis) of the object upon mine destruction.")]
        [SerializeField] private Vector2 pitchRange = new Vector2(-360, 360);

        [Tooltip("The range of the allowed yaw rotation (Y axis) of the object upon mine destruction.")]
        [SerializeField] private Vector2 yawRange = new Vector2(-360, 360);

        [Tooltip("The range of the allowed roll rotation (Z axis) of the object upon mine destruction.")]
        [SerializeField] private Vector2 rollRange = new Vector2(-360, 360);

        [Header("Timing")]
        [Tooltip("The time it takes the object to start recovering from the recoil [s].")]
        [SerializeField] private float recoveryDelay;

        [Tooltip("The time it takes the object to recover from the recoil [s].")]
        [SerializeField] private float recoveryTime;
        #endregion

        #region Constants
        private static readonly float PUSHABLE_ANGULAR_DRAG = 10;
        #endregion

        #region Class Members
        private Rigidbody rigidBody;
        private float originAngularDrag;
        private RigidbodyConstraints originConstraints;
        private Quaternion originRotation;
        #endregion

        private void Awake() {
            this.rigidBody = GetComponent<Rigidbody>();
            this.originAngularDrag = rigidBody.angularDrag;
            this.originConstraints = rigidBody.constraints;
            this.originRotation = transform.rotation;
        }

        private void Start() {
            //bind each of the level mines' detonation events
            List<Phase> allPhases = LevelFlow.Instance.Phases;
            foreach (Phase phase in allPhases) {
                List<MineGrid> grids = phase.Field.Grids;
                
                foreach (MineGrid grid in grids) {
                    if (grid.IndicationSystem.IsFatal) {
                        grid.DetonationSystem.DetonationEvent += delegate {
                            PushBack(grid);
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Wait for a certain amount of time before
        /// recovering the rigidbody's stability.
        /// </summary>
        /// <param name="delay">The time it takes the recovery to start [s]</param>
        /// <param name="delay">The time it takes the recovery complete [s]</param>
        private IEnumerator Recover(float delay, float time) {
            yield return new WaitForSeconds(delay);
            rigidBody.constraints = originConstraints;
            rigidBody.angularDrag = originAngularDrag;

            Vector3 targetEulerRot = originRotation.eulerAngles;
            Vector3 currentRot;
            float timer = 0;

            do {
                currentRot = transform.rotation.eulerAngles;
                timer += Time.deltaTime;
                float step = timer / time;
                transform.rotation = Quaternion.Lerp(transform.rotation, originRotation, step);
                yield return null;
            }
            while (currentRot.EffectivelyReached(targetEulerRot, .1f));
        }

        /// <summary>
        /// Push the object backwards from the direction of the exploding mine.
        /// </summary>
        /// <param name="grid">The exploding mine</param>
        public void PushBack(MineGrid grid) {
            Vector3 pos = transform.position;
            Vector3 gridPos = grid.transform.position;
            Vector3 direction = Vector3.Normalize(pos - gridPos);
            float dist = Vector3.Distance(pos, gridPos);
            float distPercent = RangeMath.NumberOfRange(dist, distanceRange);
            float force = RangeMath.PercentOfRange(distPercent, forceRange);
            Vector3 vector = direction * force;
            Quaternion torque = VectorUtils.GenerateRotation(pitchRange, yawRange, rollRange);

            //apply force
            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.angularDrag = PUSHABLE_ANGULAR_DRAG;
            rigidBody.AddRelativeForce(vector);
            rigidBody.AddTorque(torque.eulerAngles * force);

            StartCoroutine(Recover(recoveryDelay, recoveryTime));
        }
    }
}