using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeepSweeper.ShootingSystem
{
    public class TorpedoLauncher : PrimarySubmarineGun
    {
        #region Properties
        public override GunSubType SubType => GunSubType.Torpedo;
        #endregion

        /// <inheritdoc/>
        protected override void FireAtIndicator(SightRay.TargetInfo target) {
            //only fire the bullets if the indicator is fulfilled
            if (target.Indicator.IsIndicationFulfilled) {
                IEnumerable<MineGrid> section = from neighbour in target.Grid.Section
                                                where neighbour != null && !neighbour.DetonationSystem.IsDetonated && !neighbour.SelectionSystem.IsFlagged
                                                select neighbour;

                //fire a bullet at each of the neighbours
                if (section.Count() > 0) {
                    Recoil(recoilForce);
                    foreach (MineGrid neighbour in section) {
                        Vector3 neighbourPos = neighbour.Avatar.transform.position;
                        Vector3 neighbourDir = Vector3.Normalize(neighbourPos - transform.position);
                        Fire(neighbourDir, false, neighbour, true);
                    }
                }
                else FireAtNull();
            }
            else FireAtNull();
        }

        /// <inheritdoc/>
        protected override void FireAtMine(SightRay.TargetInfo target) {
            Fire(transform.forward, true, target.Grid);
        }

        /// <inheritdoc/>
        protected override void FireAtNull() {
            Fire(transform.forward, true, null);
        }
    }
}