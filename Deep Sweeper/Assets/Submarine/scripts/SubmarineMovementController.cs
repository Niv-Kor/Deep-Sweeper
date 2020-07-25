using UnityEditor;
using UnityEngine;

public class SubmarineMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Minimum height of the submarine above the ground.")]
    [SerializeField] private float minHeight = 1;

    [Tooltip("The submarine's movement speed.")]
    [SerializeField] private float speed = 1;

    [Tooltip("The maximum achievable turbo speed.")]
    [SerializeField] [Range(1.5f, 5)] private float maxTurbo = 2;

    [Tooltip("The submarine's turn speed.")]
    [SerializeField] private float turnSpeed = 1;

    [Tooltip("The submarine's ascending speed.")]
    [SerializeField] private float ascendSpeed = 1;

    [Tooltip("The submarine's descending speed.")]
    [SerializeField] private float descendSpeed = 1;

    [Tooltip("The maximum velocity the submariine is able to reach.")]
    [SerializeField] private float maxVelocity = 5;

    [Header("Wave Floating Settings")]
    [Tooltip("True to allow the submarine to float over the underwater waves at rest.")]
    [SerializeField] bool useFloat = true;

    [Tooltip("The floating speed over the waves.")]
    [SerializeField] private float floatSpeed = 1;

    [Tooltip("The delay between two waves.")]
    [SerializeField] private float waveDelay = .5f;

    [Tooltip("The float height of each wave.")]
    [SerializeField] private float waveLength = 1;

    private Rigidbody rigidBody;
    private DirectionUnit directionUnit;
    private Vector3 prevVel;
    private Vector3 startingFloatPos;
    private Vector3 lastDirection;
    private float delayFloatTimer;
    private float lerpedFloatTime;
    private bool floatUp;
    private bool resting;
    private bool finishFloatWave;

    private void Start() {
        this.rigidBody = GetComponent<Rigidbody>();
        this.directionUnit = DirectionUnit.Instance;
        this.lastDirection = Vector3.zero;
        this.prevVel = Vector3.zero;
        this.delayFloatTimer = waveDelay;
        this.startingFloatPos = transform.position;
        this.lerpedFloatTime = 0;
        this.resting = true;
        this.floatUp = Random.Range(0, 1) > .5f;
        this.finishFloatWave = false;
    }

    private void Update() {
        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");
        float ascendInput = Input.GetAxis("Ascend");
        float turboInput = Input.GetAxis("Turbo") * (maxTurbo - 1) + 1;

        print("turbo: " + turboInput);

        lastDirection = Move(horInput, verInput, ascendInput, turboInput);
        if (Mathf.Abs(verInput) > 0 || Mathf.Abs(horInput) > 0) Turn(lastDirection);

        //unfreeze position y if ascending or descending
        bool unfreezeCond = ascendInput > 0 || transform.position.y > minHeight;
        var defaultConstaint = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        var yFreezeConstaint = defaultConstaint | RigidbodyConstraints.FreezePositionY;
        rigidBody.constraints = unfreezeCond ? defaultConstaint : yFreezeConstaint;
        if (!resting) Descend(ascendInput);

        if (useFloat) {
            bool atMinHeight = Mathf.Abs(transform.position.y - minHeight) < .1f;

            if (!resting && ascendInput == 0 && atMinHeight) {
                resting = true;
                startingFloatPos = transform.position;
            }
            else if (ascendInput > 0) resting = false;

            if (resting) Float();
        }
    }

    /// <summary>
    /// Move the submarine.
    /// </summary>
    /// <param name="horInput">Horizontal movement power [-1:1]</param>
    /// <param name="verInput">Vertical movement power [-1:1]</param>
    /// <param name="ascendInput">Ascending movement power [0:1]</param>
    /// <param name="turboInput">Additional turbo multiplier [1:2]</param>
    /// <returns>The direction to which the user is directing.</returns>
    private Vector3 Move(float horInput, float verInput, float ascendInput, float turboInput) {
        /*//prevent the submarine's velocity from diverging
        if (rigidBody.velocity.magnitude > maxVelocity)
            return lastDirection;*/

        float finalSpeed = speed * turboInput;
        Vector3 verDirection = directionUnit.transform.forward * verInput;
        Vector3 horDirection = directionUnit.transform.right * horInput;
        Vector3 direction = verDirection + horDirection;
        Vector3 ascendVector = Vector3.up * ascendInput * ascendSpeed;
        Vector3 forceVector = direction * finalSpeed + ascendVector;
        direction.y = 0;
        rigidBody.AddForce(forceVector);
        prevVel = rigidBody.velocity;
        return direction;
    }

    /// <summary>
    /// Turn the submarine to face a specific direction.
    /// </summary>
    /// <param name="direction">The direction towards which the submarine should look</param>
    private void Turn(Vector3 direction) {
        if (direction.sqrMagnitude > 0) {
            direction.Normalize();
            Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * turnSpeed);
        }
    }

    /// <summary>
    /// Discharge the submarine's vertical velocity by pulling it back to the ground.
    /// This function only works if ascending input is 0 and the submarine is above its minimum height.
    /// </summary>
    /// <param name="ascendInput">Ascending movement power [0:1]</param>
    private void Descend(float ascendInput) {
        if (ascendInput == 0 && transform.position.y > minHeight)
            rigidBody.AddForce(Vector3.down * descendSpeed);
    }

    /// <summary>
    /// Allow the submarine to float up and down at rest.
    /// </summary>
    private void Float() {
        delayFloatTimer += Time.deltaTime;

        //start wave
        if (delayFloatTimer >= waveDelay) {
            if (!finishFloatWave) {
                //generate a parabolic variable
                lerpedFloatTime += Time.deltaTime;
                if (lerpedFloatTime > 1) lerpedFloatTime = 2 - lerpedFloatTime;

                Vector3 pos = transform.position;
                float targetDirection = floatUp ? 1 : -1;
                float targetHeight = startingFloatPos.y + waveLength * targetDirection;
                float currentHeight = Mathf.Lerp(pos.y, targetHeight, Time.deltaTime * lerpedFloatTime * floatSpeed);
                transform.position = new Vector3(pos.x, currentHeight, pos.z);

                //finish wave
                if (Mathf.Abs(targetHeight - pos.y) < .05f) finishFloatWave = true;
            }
            else {
                delayFloatTimer = 0;
                lerpedFloatTime = 0;
                floatUp = !floatUp;
                finishFloatWave = false;
                startingFloatPos = transform.position;
            }
        }
    }
}