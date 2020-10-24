using com.ootii.Cameras;
using System.Collections;
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

    [Tooltip("The number with which the submarine's speed multiplies when using the turbo feature.")]
    [SerializeField] private float turboMultiplier = 2f;

    [Header("Wave Floating Settings")]
    [Tooltip("True to allow the submarine to float over the underwater waves at rest.")]
    [SerializeField] bool useFloat = true;

    [Tooltip("The floating speed over the waves.")]
    [SerializeField] private float floatSpeed = 1;

    [Tooltip("The float height of each wave.")]
    [SerializeField] private Vector2 waveLengthRange;

    private Rigidbody rigidBody;
    private DirectionUnit directionUnit;
    private Vector3 startRestingPos;
    private float waveLength;
    private bool resting;

    public bool MovementAllowd { get; set; }

    private void Start() {
        this.rigidBody = GetComponent<Rigidbody>();
        this.directionUnit = DirectionUnit.Instance;
        this.waveLength = RangeMath.PercentOfRange(WaterPhysics.Instance.IntensityPercentage, waveLengthRange);
        this.startRestingPos = transform.position;
        this.resting = true;
        this.MovementAllowd = true;
        if (useFloat) StartCoroutine(Float());
    }

    private void Update() {
        //get user input
        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");
        float ascendInput = Input.GetAxis("Ascend");
        float descendInput = Input.GetAxis("Descend");
        float turboInput = Input.GetAxis("Turbo");
        float heightInput = (Mathf.Abs(ascendInput) > Mathf.Abs(descendInput)) ? ascendInput : descendInput;

        Move(horInput, verInput, heightInput, turboInput);

        //unfreeze position y if ascending or descending
        bool unfreezeCond = ascendInput > 0 || transform.position.y > minHeight;
        var defaultConstaint = RigidbodyConstraints.FreezeRotationX |
                               RigidbodyConstraints.FreezeRotationY |
                               RigidbodyConstraints.FreezeRotationZ;

        var yFreezeConstaint = defaultConstaint | RigidbodyConstraints.FreezePositionY;
        rigidBody.constraints = unfreezeCond ? defaultConstaint : yFreezeConstaint;

        //float
        if (useFloat) {
            bool prevRestingState = resting;
            bool ascending = ascendInput > 0;
            bool descending = Mathf.Abs(descendInput) > 0;

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
        }
    }

    /// <summary>
    /// Move the submarine.
    /// </summary>
    /// <param name="horInput">Horizontal movement power [-1:1]</param>
    /// <param name="verInput">Vertical movement power [-1:1]</param>
    /// <param name="heightInput">Ascending or descending movement power [0:1]</param>
    /// <param name="turboInput">Turbo movement power [0:1]</param>
    private void Move(float horInput, float verInput, float heightInput, float turboInput) {
        if (!MovementAllowd) return;

        float turboPercent = turboInput / 1;
        float speedMultiplier = turboPercent * (turboMultiplier - 1) + 1;
        float speed = horizontalSpeed * speedMultiplier;
        Vector3 verDirection = directionUnit.transform.forward * verInput;
        Vector3 horDirection = directionUnit.transform.right * horInput;
        Vector3 direction = verDirection + horDirection;
        Vector3 verticalVector = Vector3.up * heightInput * verticalSpeed;
        Vector3 forceVector = direction * speed + verticalVector;
        direction.y = 0;
        rigidBody.AddForce(forceVector);

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