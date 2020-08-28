using UnityEngine;

public class MineClone : MeshClone
{
    protected override void Awake() {
        base.Awake();
        MineSelector selector = GetComponentInParent<MineSelector>();
        selector.ModeApplicationEvent += ApplySelectionMode;
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
}