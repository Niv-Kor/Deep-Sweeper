using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public class TargetInfo
    {
        #region Class Members
        private GameObject avatar;
        #endregion

        #region Properties
        public MineGrid Grid { get; private set; }
        public ObjectActivator Activator { get; private set; }
        public IndicationSystem Indicator { get; private set; }
        public SelectionSystem Selector { get; private set; }
        public bool IsValueable {
            get => !Grid.DetonationSystem.IsDetonated || Indicator.Value > 0;
        }
        #endregion

        public TargetInfo(GameObject mine) {
            this.avatar = mine;
            this.Grid = mine.GetComponentInParent<MineGrid>();
            this.Activator = Grid.Activator;
            this.Indicator = Grid.IndicationSystem;
            this.Selector = Grid.SelectionSystem;
        }

        /// <summary>
        /// Check if the refrenced object is the same as another object.
        /// </summary>
        /// <param name="other">The object to test</param>
        /// <returns>True if both objects reference the same location in memory.</returns>
        public bool Equals(GameObject other) {
            return avatar == other;
        }
    }
}