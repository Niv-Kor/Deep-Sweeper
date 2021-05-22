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

        [Tooltip("The time it takes a fast dash movement to decay back to normal.")]
        [SerializeField] private float dashDecayTime = 1;
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
        private Coroutine velClampRevertCoroutine;
        private float velClamp;
        #endregion

        #region Properties
        public MobilityAbilityModel MobilitySettings { get; set; }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.velClamp = maxVelocity;
            this.verEngineConstraints = RigidbodyConstraints.FreezeRotation;
            this.horEngineConstraints = verEngineConstraints | RigidbodyConstraints.FreezePositionY;
        }

        private void Start() {
            this.directionUnit = DirectionUnit.Instance;

            //bind events
            controller.DashEvent += DashHorizontally;
            controller.HorizontalMovementEvent += delegate (Vector2 vector) { MoveHorizontally(vector); };
            controller.VerticalMovementEvent += delegate (float value) { MoveVertically(value); };
            controller.HorizontalMovementStopEvent += delegate { velClamp = maxVelocity; };
            controller.VerticalMovementStopEvent += delegate {
                velClamp = maxVelocity;
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
        /// <param name="speedMultiplier">A multiplier for the calculated speed</param>
        private void MoveHorizontally(Vector2 vector, float speedMultiplier = 1) {
            float speed = MobilitySettings.HorizontalSpeed * speedMultiplier;
            Vector3 zDirection = directionUnit.transform.forward * vector.y;
            Vector3 xDirection = directionUnit.transform.right * vector.x;
            Vector3 direction = zDirection + xDirection;
            Vector3 forceVector = direction * speed;
            Move(forceVector);
        }

        /// <summary>
        /// Dash towards a horizontal direction.
        /// </summary>
        /// <param name="direction">
        /// X > 0: right;
        /// X < 0: left;
        /// Y > 0: forward;
        /// Y < 0: bottom
        /// </param>
        private void DashHorizontally(Vector2 direction) {
            float multiplier = MobilitySettings.DashMultiplier;

            if (multiplier > 1) {
                velClamp = maxVelocity * multiplier;
                MoveHorizontally(direction, multiplier);

                if (velClampRevertCoroutine != null) StopCoroutine(velClampRevertCoroutine);
                velClampRevertCoroutine = StartCoroutine(RevertVelocityClamp());
            }
        }

        /// <summary>
        /// Move the submarine vertically (across the Y axis).
        /// </summary>
        /// <param name="vector">
        /// A positive (0:1] value when ascending
        /// or a negative [-1:0) when descending.
        /// </param>
        /// <param name="speedMultiplier">A multiplier for the calculated speed</param>
        private void MoveVertically(float value, float speedMultiplier = 1) {
            if (freezeYCoroutine != null) StopCoroutine(freezeYCoroutine);
            rigidBody.constraints = verEngineConstraints;

            float speed = MobilitySettings.VerticalSpeed * speedMultiplier;
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

        /// <summary>
        /// Revert the velocity clamp value back to the configured
        /// max velocity value after a successful dash.
        /// </summary>
        private IEnumerator RevertVelocityClamp() {
            float startClamp = velClamp;
            float timer = 0;

            while (timer <= dashDecayTime) {
                timer += Time.deltaTime;
                velClamp = Mathf.Lerp(startClamp, maxVelocity, timer / dashDecayTime);
                yield return null;
            }
        }

        /// <inheritdoc/>
        protected override void Move(Vector3 vector) {
            rigidBody.AddRelativeForce(vector);

            //prevent the submarine's velocity from diverging
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, velClamp);
        }
    }
}