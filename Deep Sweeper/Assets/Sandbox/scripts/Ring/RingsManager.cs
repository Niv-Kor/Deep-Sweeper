using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Menu.Sandbox.Ring
{
    public class RingsManager : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The prefab of a selectable level ring.")]
        [SerializeField] private LevelRing levelRingPrefab;
        #endregion

        #region Class Members
        private List<LevelRing> rings;
        #endregion

        private void Awake() {
            this.rings = new List<LevelRing>();
        }

        /// <summary>
        /// Instantiate a level ring.
        /// </summary>
        /// <param name="levelTransform">The requesting level object's transform component</param>
        /// <returns>The instance of the created level ring.</returns>
        public LevelRing CreateRing(Transform levelTransform) {
            float cubicScale = levelRingPrefab.transform.localScale.x;
            LevelRing instance = Instantiate(levelRingPrefab);
            instance.transform.SetParent(levelTransform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one * cubicScale;
            instance.Manager = this;
            rings.Add(instance);
            return instance;
        }

        /// <summary>
        /// Report a selected ring.
        /// This function deselects all other rings.
        /// </summary>
        /// <param name="selectedRing">The reporting ring</param>
        public void ReportSelection(LevelRing selectedRing) {
            foreach (LevelRing ring in rings)
                if (ring != selectedRing) ring.Selected = false;
        }
    }
}