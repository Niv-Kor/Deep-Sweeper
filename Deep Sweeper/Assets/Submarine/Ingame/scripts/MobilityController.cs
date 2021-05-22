using GamedevUtil.Player;
using System.Collections;
using UnityEngine;

namespace DeepSweeper.Player
{
    public class MobilityController : PlayerController3D
    {
        #region Exposed Editor Parameters
        [Tooltip("The maximum velocity magnitude at which the submarine can move.")]
        [SerializeField] private float maxVelocity = 10;
        #endregion

        #region Constants
        private static readonly float FREEZE_Y_INITIAL_DELAY = 1;
        private static readonly float VELOCITY_Y_REDUCE_TIME = 1.5f;
        #endregion

        #region Class Members
        private RigidbodyConstraints verEngineConstraints;
        private RigidbodyConstraints horEngineConstraints;
        private DirectionUnit directionUnit;
        private Coroutine freezeYCoroutine;
        #endregion

        #region Properties
        public MobilityAbilityModel MobilitySettings { get; set; }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.verEngineConstraints = RigidbodyConstraints.FreezeRotation;
            this.horEngineConstraints = verEngineConstraints | RigidbodyConstraints.FreezePositionY;
        }

        private void Start() {
            this.directionUnit = DirectionUnit.Instance;

            //bind events
            controller.HorizontalMovementEvent += MoveHorizontally;
            controller.VerticalMovementEvent += MoveVertically;
            controller.VerticalMovementStopEvent += delegate {
                freezeYCoroutine = StartCoroutine(SmoothFreezeY());
            };
        }

        /// <summary>
        /// Move the submarine horizontally (across the X and Z axes).
        /// </summary>
        /// <param name="vector">
        /// X slot represents a Z axis movement (backwards [-1:0) and forwards (0:1])
        /// and Y slot represents an X axis movement (left [-1:0) and right (0:1])
        /// </param>
        private void MoveHorizontally(Vector2 vector) {
            float speed = MobilitySettings.HorizontalSpeed;
            Vector3 zDirection = directionUnit.transform.forward * vector.y;
            Vector3 xDirection = directionUnit.transform.right * vector.x;
            Vector3 direction = zDirection + xDirection;
            Vector3 forceVector = direction * speed;
            Move(forceVector);
        }

        /// <summary>
        /// Move the submarine vertically (across the Y axis).
        /// </summary>
        /// <param name="vector">
        /// A positive (0:1] value when ascending
        /// or a negative [-1:0) when descending.
        /// </param>
        private void MoveVertically(float value) {
            if (freezeYCoroutine != null) StopCoroutine(freezeYCoroutine);
            rigidBody.constraints = verEngineConstraints;


            float speed = MobilitySettings.VerticalSpeed;
            Vector3 direction = Vector3.up * value;
            Vector3 forceVector = direction * speed;
            Move(forceVector);
        }

        /// <summary>
        /// Smoothly freeze the rigidbody's Y position after ascendance or descendence.
        /// </summary>
        private IEnumerator SmoothFreezeY() {
            //wait for the velocity change to kick in
            yield return new WaitForSeconds(FREEZE_Y_INITIAL_DELAY);

            //wait for pure gravity force
            float velY;
            bool ascending;
            bool descending;
            bool positiveVel = rigidBody.velocity.y > 0;
            float baseGravity = Physics.gravity.y;

            do {
                velY = rigidBody.velocity.y;
                ascending = positiveVel && velY > baseGravity;
                descending = !positiveVel && velY < 0;
                yield return null;
            }
            while (ascending || descending);

            //reduce Y velocity to 0
            yield return ReduceVelocityY();
            yield return new WaitForSeconds(VELOCITY_Y_REDUCE_TIME);

            //freeze Y poisition
            rigidBody.constraints = horEngineConstraints;
        }

        /// <summary>
        /// Smoothly reduce the Y velocity to 0.
        /// </summary>
        private IEnumerator ReduceVelocityY() {
            float timer = 0;
            float time = VELOCITY_Y_REDUCE_TIME;
            float startingY = rigidBody.velocity.y;
            Vector3 XZMask = Vector3.right + Vector3.forward;

            while (timer <= time) {
                timer += Time.deltaTime;
                Vector3 vel = rigidBody.velocity;
                Vector3 maskedVel = Vector3.Scale(vel, XZMask);
                float yStep = Mathf.Lerp(startingY, 0, timer / time);
                rigidBody.velocity = maskedVel + Vector3.up * yStep;
                yield return null;
            }
        }

        /// <inheritdoc/>
        protected override void Move(Vector3 vector) {
            rigidBody.AddRelativeForce(vector);

            //prevent the submarine's velocity from diverging
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxVelocity);
        }
    }
}