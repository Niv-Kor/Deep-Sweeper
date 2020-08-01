using UnityEngine;

public class Flag : MonoBehaviour
{
    [Tooltip("The force of the wind's affection on the flag.")]
    [SerializeField] [Range(0, 5f)] private float windForce;

    private Cloth cloth;
    private float wildTime;

    void Start() {
        this.cloth = GetComponent<Cloth>();
    }

    void Update() {
        wildTime -= Time.deltaTime;

        if (wildTime <= 0) {
            wildTime = 5 - windForce;
            cloth.useGravity = !cloth.useGravity;
        }
    }
}
