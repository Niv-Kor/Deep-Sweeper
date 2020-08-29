using UnityEngine;

public class SandPile : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("All available pile prefabs - to be selected randomly.")]
    [SerializeField] private GameObject[] piles;

    [Header("Settings")]
    [Tooltip("The generated position of the randomly selected pile.")]
    [SerializeField] private Vector3 position;

    [Tooltip("The generated euler rotation of the randomly selected pile.")]
    [SerializeField] private Vector3 rotation;

    [Tooltip("The generated scale of the randomly selected pile.")]
    [SerializeField] private Vector3 scale;

    [Tooltip("The time it takes the pile to finish dissolving.")]
    [SerializeField] private float dissolveTime;

    private static readonly string OBJECT_NAME = "Mesh";
    private static readonly string DISSOLVE_SHADER_PROPERTY = "_Metallic";

    private GameObject mesh;

    private void Awake() {
        if (piles.Length > 0) this.mesh = CreateMesh();
    }

    /// <summary>
    /// Create a random instance of a sand pile mesh.
    /// </summary>
    /// <returns>The created instance.</returns>
    private GameObject CreateMesh() {
        int selectedIndex = Random.Range(0, piles.Length);
        GameObject instance = Instantiate(piles[selectedIndex]);
        instance.transform.position = Vector3.zero;
        instance.transform.SetParent(transform);
        instance.transform.localPosition = position;
        instance.transform.localRotation = Quaternion.Euler(rotation);
        instance.transform.localScale = scale;
        instance.name = OBJECT_NAME;
        return instance;
    }
}