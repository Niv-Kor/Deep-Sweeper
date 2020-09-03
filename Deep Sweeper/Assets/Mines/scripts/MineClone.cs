using UnityEngine;

public class MineClone : MeshClone
{
    private SphereCollider sphereCol;

    protected override void Awake() {
        base.Awake();
        MineSelector selector = GetComponentInParent<MineSelector>();
        selector.ModeApplicationEvent += ApplySelectionMode;
        this.sphereCol = GetComponent<SphereCollider>();
    }

    /// <summary>
    /// Activate when the source mesh changes selection mode.
    /// This function immediately changes the color of the clone.
    /// </summary>
    /// <param name="material"></param>
    private void ApplySelectionMode(SelectionMode _, Material material) {
        currentMaterial = material;
        if (isShown) render.material = material;
    }

    /// <inheritdoc/>
    public override void DisplayMesh(bool flag) {
        base.DisplayMesh(flag);
        sphereCol.enabled = flag;
    }
}