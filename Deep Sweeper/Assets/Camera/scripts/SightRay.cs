using UnityEngine;

public class SightRay : MonoBehaviour
{
    private class MineInfo
    {
        private GameObject mineObj;
        private MineGrid grid;
        private MineHighlighter highlighter;
        private MineExploder exploder;

        /// <param name="mine">A mine object</param>
        public MineInfo(GameObject mine) {
            this.mineObj = mine;
            this.grid = mineObj.GetComponentInParent<MineGrid>();
            this.highlighter = mineObj.GetComponent<MineHighlighter>();
            this.exploder = mineObj.GetComponent<MineExploder>();
        }

        /// <summary>
        /// Highlight the mine.
        /// </summary>
        /// <param name="flag">True to highlight or false to cancel</param>
        public void Highlight(bool flag) {
            highlighter.Highlight(flag);
        }

        /// <summary>
        /// Check if a game object is the same as the one this object references.
        /// </summary>
        /// <param name="obj">The game object to check</param>
        /// <returns>True if both game objects are the same.</returns>
        public bool Equals(GameObject obj) {
            return mineObj == obj;
        }

        /// <summary>
        /// Put a flag on the mine or dispose it.
        /// </summary>
        public void ToggleFlag() {
            grid.IsFlagged = !grid.IsFlagged;
        }

        /// <summary>
        /// Explode the mine.
        /// </summary>
        public void Explode() {
            grid.IsFlagged = false;
            exploder.Explode();
        }
    }

    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private float maxDistance = 100f;

    [Tooltip("Maximum raycast distance from the sight's center.")]
    [SerializeField] private LayerMask hitLayers;

    private MineInfo selectedMine;

    private void Update() {
        CastRay();

        //follow user input
        if (selectedMine != null) {
            bool leftMouse = Input.GetMouseButtonDown(0);
            bool rightMouse = Input.GetMouseButtonDown(1);

            if (rightMouse) selectedMine.ToggleFlag();
            if (leftMouse) {
                selectedMine.ToggleFlag();
                selectedMine.Explode();
                selectedMine = null;
            }
        }
    }

    /// <summary>
    /// Cast a ray at mines to select them.
    /// </summary>
    private void CastRay() {
        RaycastHit raycastHit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        bool hit = Physics.Raycast(origin, direction, out raycastHit, maxDistance, hitLayers);

        if (hit) {
            GameObject mineObj = raycastHit.collider.gameObject;

            if (selectedMine == null) SelectMine(mineObj);
            else if (!selectedMine.Equals(mineObj)) {
                selectedMine.Highlight(false);
                SelectMine(mineObj);
            }
        }
        else if (selectedMine != null) {
            selectedMine.Highlight(false);
            selectedMine = null;
        }
    }

    /// <summary>
    /// Select a mine object.
    /// </summary>
    /// <param name="mine">The object to select</param>
    private void SelectMine(GameObject mine) {
        selectedMine = new MineInfo(mine);
        selectedMine.Highlight(true);
    }
}
