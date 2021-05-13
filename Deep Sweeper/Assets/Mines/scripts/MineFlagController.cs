using UnityEngine;

namespace DeepSweeper.Level.Mine
{
    public class MineFlagController : MonoBehaviour
    {
        #region Class Members
        private ParticleSystem[] partSystems;
        #endregion

        private void Start() {
            this.partSystems = GetComponentsInChildren<ParticleSystem>();
            MineGrid grid = GetComponentInParent<MineGrid>();
            grid.SelectionSystem.ModeApplicationEvent += OnSelectionChangeEvent;
            ActivateParticles(false);
        }

        /// <summary>
        /// Activate when the selection mode of the mine changes.
        /// </summary>
        /// <param name="oldMode"></param>
        /// <param name="newMode"></param>
        private void OnSelectionChangeEvent(SelectionMode oldMode, SelectionMode newMode) {
            bool oldFlagged = SelectionSystem.IsFlagMode(oldMode);
            bool newFlagged = SelectionSystem.IsFlagMode(newMode);
            bool fromFlag = oldFlagged && !newFlagged;
            bool toFlag = !oldFlagged && newFlagged;

            if (fromFlag) ActivateParticles(false);
            else if (toFlag) ActivateParticles(true);
        }

        /// <summary>
        /// Activate or deactivate the flag particles.
        /// </summary>
        /// <param name="flag">True to activate or false to deactivate</param>
        private void ActivateParticles(bool flag) {
            foreach (var system in partSystems) system.gameObject.SetActive(flag);
        }
    }
}