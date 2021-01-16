using UnityEngine;

public class MenuSubmarineSurfer : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Tooltip("The force at which the submarine surfs.")]
    [SerializeField] private float force = 1;

    [Tooltip("The maximum force at which the submarine rotates forward (x) and sideways (y).")]
    [SerializeField] private Vector2 axisRotation = Vector2.one;
    #endregion

    #region Class Members
    private Vector3 currentVec;
    private float timer;
    private float sineWave;
    private bool regenerated;
    #endregion

    private void Start() {
        this.currentVec = GenerateSurfVector();
        this.timer = 0;
        this.sineWave = 0;
        this.regenerated = false;
    }

    private void OnValidate() {
        axisRotation.x = Mathf.Clamp(axisRotation.x, 0, 1);
        axisRotation.y = Mathf.Clamp(axisRotation.y, 0, 1);
    }

    private void Update() {
        float prevWave = sineWave;
        timer += Time.deltaTime;
        sineWave = Mathf.Sin(timer);

        //regenerate vector
        if (!regenerated && prevWave > 0 && sineWave < 0) {
            currentVec = GenerateSurfVector();
            regenerated = true;
        }
        else if (regenerated && prevWave < 0 && sineWave > 0)
            regenerated = false;

        transform.RotateAround(transform.position, currentVec, sineWave * force);
    }

    /// <summary>
    /// Generate a random direction vector that involves
    /// the submarine's base x and z axes values.
    /// </summary>
    /// <returns>A randomly selected right and forward direction vector.</returns>
    private Vector3 GenerateSurfVector() {
        float xMultiplier = Random.Range(0f, axisRotation.x);
        float zMultiplier = Random.Range(0f, axisRotation.y);
        Vector3 x = transform.right * xMultiplier;
        Vector3 z = transform.forward * zMultiplier;
        return x + z;
    }
}