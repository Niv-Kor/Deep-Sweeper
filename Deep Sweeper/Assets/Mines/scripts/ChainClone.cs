using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ChainClone : MeshClone
{
    [SerializeField] private Transform mineClone;

    private Transform parentObj;
    private float mineHeightExtent;
    private float cylinderLen;

    private void Start() {
        Renderer mineRender = mineClone.gameObject.GetComponent<Renderer>();
        this.mineHeightExtent = mineRender.bounds.extents.y;
        this.parentObj = transform.parent;
        this.cylinderLen = render.bounds.size.y;
    }

    public override void DisplayMesh(bool flag) {
        base.DisplayMesh(flag);

        //rotate the chain clone towards the updated mine clone's position
        if (flag) {
            Vector3 mineVertex = mineClone.position - Vector3.up * mineHeightExtent;
            Vector3 zeroZScale = Vector3.Scale(parentObj.localScale, Vector3.right + Vector3.up);
            float dist = Vector3.Distance(parentObj.position, mineVertex);
            parentObj.LookAt(mineVertex);
            parentObj.localScale = zeroZScale + Vector3.forward * dist / cylinderLen;
        }
    }
}