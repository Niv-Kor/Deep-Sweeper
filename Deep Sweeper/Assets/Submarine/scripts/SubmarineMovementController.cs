using UnityEngine;

public class SubmarineMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Minimum height of the submarine above the ground.")]
    [SerializeField] private float minHeight = 1;

    [Tooltip("The submarine's movement speed.")]
    [SerializeField] private float horizontalSpeed = 1;

    [Tooltip("The submarine's ascending speed.")]
    [SerializeField] private float verticalSpeed = 1;

    [Tooltip("The maximum velocity magnitude is determined by current speed divided by relativeMaxMagnitude.")]
    [SerializeField] [Range(1, 50f)] private float maxVelocity = 10;

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
        this.delayFloatTimer = waveDelay;
        this.startingFloatPos = transform.position;
        this.lerpedFloatTime = 0;
        this.resting = true;
        this.floatUp = Random.Range(0, 1) > .5f;
        this.finishFloatWave = false;
    }

    private void Update() {
        //get user input
        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");
        float ascendInput = Input.GetAxis("Ascend");
        float descendInput = Input.GetAxis("Descend");
        float heightInput = (Mathf.Abs(ascendInput) > Mathf.Abs(descendInput)) ? ascendInput : descendInput;

        lastDirection = Move(horInput, verInput, heightInput);

        //unfreeze position y if ascending or descending
        bool unfreezeCond = ascendInput > 0 || transform.position.y > minHeight;
        var defaultConstaint = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        var yFreezeConstaint = defaultConstaint | RigidbodyConstraints.FreezePositionY;
        rigidBody.constraints = unfreezeCond ? defaultConstaint : yFreezeConstaint;

        //float
        if (useFloat) {
            bool ascending = ascendInput > 0;
            bool descending = Mathf.Abs(descendInput) > 0;

            if (!resting && !ascending && !descending) {
                resting = true;
                startingFloatPos = transform.position;
            }
            else if (ascending || descending) resting = false;

            if (resting) Float();
        }
    }

    /// <summary>
    /// Move the submarine.
    /// </summary>
    /// <param name="horInput">Horizontal movement power [-1:1]</param>
    /// <param name="verInput">Vertical movement power [-1:1]</param>
    /// <param name="heightInput">Ascending or descending movement power [0:1]</param>
    /// <returns>The direction to which the user is directing.</returns>
    private Vector3 Move(float horInput, float verInput, float heightInput) {
        Vector3 verDirection = directionUnit.transform.forward * verInput;
        Vector3 horDirection = directionUnit.transform.right * horInput;
        Vector3 direction = verDirection + horDirection;
        Vector3 verticalVector = Vector3.up * heightInput * verticalSpeed;
        Vector3 forceVector = direction * horizontalSpeed + verticalVector;
        direction.y = 0;
        rigidBody.AddForce(forceVector);

        //prevent the submarine's velocity from diverging
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxVelocity);
        return direction;
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

    public void Shock(float force) {
        Vector3 backwards = CameraBase.Instance.FPCam.transform.forward * -1;
        rigidBody.AddForce(backwards * force);
    }
}