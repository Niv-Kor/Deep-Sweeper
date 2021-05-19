using GamedevUtil.Player;
using System.Collections;
using UnityEngine;

namespace DeepSweeper.Player
{
    public class MobilityController : PlayerController3D
    {
        #region Exposed Editor Parameters
        [Header("Movement Settings")]
        [Tooltip("Minimum height of the submarine above the ground.")]
        [SerializeField] private float minHeight = 1;

        [Tooltip("The maximum velocity magnitude is determined by current speed divided by 'relativeMaxMagnitude'.")]
        [SerializeField] [Range(1, 50f)] private float maxVelocity = 10;

        [Header("Wave Floating Settings")]
        [Tooltip("True to allow the submarine to float over the underwater waves at rest.")]
        [SerializeField] bool useFloat = true;

        [Tooltip("The floating speed over the waves.")]
        [SerializeField] private float floatSpeed = 1;

        [Tooltip("The float height of each wave.")]
        [SerializeField] private Vector2 waveLengthRange;
        #endregion

        #region Class Members
        private RigidbodyConstraints defaultConstraints;
        private RigidbodyConstraints yFreezeConstraint;
        private DirectionUnit directionUnit;
        private Vector3 startRestingPos;
        private float waveLength;
        private bool resting;
        #endregion

        #region Properties
        public MobilityConfig MobilitySettings { get; set; }
        #endregion

        protected override void Awake() {
            base.Awake();
            this.defaultConstraints = rigidBody.constraints;
            this.yFreezeConstraint = defaultConstraints | RigidbodyConstraints.FreezePositionY;
        }

        private void Start() {
            this.directionUnit = DirectionUnit.Instance;
            this.waveLength = RangeMath.PercentOfRange(WaterPhysics.Instance.IntensityPercentage, waveLengthRange);
            this.startRestingPos = transform.position;
            this.resting = true;

            //bind events
            controller.HorizontalMovementEvent += MoveHorizontally;
            controller.VerticalMovementEvent += MoveVertically;

            if (useFloat) StartCoroutine(Float());
        }

        private void Update() {
            /*if (!IsMovable) return;

            float ascendInput = controller.Vertical.y;
            Vector3 moveVector = new Vector3(controller.Horizontal.x, controller.Vertical.y, controller.Horizontal.y);
            if (moveVector != Vector3.zero) MoveHorizontally(moveVector);

            //unfreeze Y position if ascending or descending
            bool unfreezeCond = controller.Vertical.y > 0 || transform.position.y > minHeight;

            var yFreezeConstraint = defaultConstaint | RigidbodyConstraints.FreezePositionY;
            rigidBody.constraints = unfreezeCond ? defaultConstaint : yFreezeConstraint;

            //float
            if (useFloat) {
                bool prevRestingState = resting;
                bool ascending = ascendInput > 0;
                bool descending = ascendInput < 0;

                if (!resting && !ascending && !descending) {
                    resting = true;
                    startRestingPos = transform.position;
                }
                else if (ascending || descending) resting = false;

                //change in resting state
                if (prevRestingState != resting) {
                    StopAllCoroutines();

                    if (resting) StartCoroutine(Float());
                    else StopCoroutine(Float());
                }
            }*/
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
            float speed = MobilitySettings.VerticalSpeed;
            Vector3 direction = Vector3.up * value;
            Vector3 forceVector = direction * speed;
            Move(forceVector);
        }

        /// <inheritdoc/>
        protected override void Move(Vector3 vector) {
            rigidBody.AddForce(vector);

            //prevent the submarine's velocity from diverging
            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxVelocity);
        }

        /// <summary>
        /// Allow the submarine to float up and down at rest.
        /// </summary>
        private IEnumerator Float() {
            float timer = 0;

            while (resting) {
                timer += Time.deltaTime;
                Vector3 pos = transform.position;
                float sineWave = Mathf.Sin(timer * floatSpeed);
                float targetHeight = startRestingPos.y + waveLength * sineWave;
                transform.position = new Vector3(pos.x, targetHeight, pos.z);
                yield return null;
            }
        }
    }
}