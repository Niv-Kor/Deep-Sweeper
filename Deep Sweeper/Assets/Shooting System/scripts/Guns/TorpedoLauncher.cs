using System.Collections.Generic;
using DeepSweeper.Level.Mine;
using System.Linq;
using UnityEngine;

namespace DeepSweeper.Player.ShootingSystem
{
    public class TorpedoLauncher : SubmarineGun
    {
        #region Properties
        public override GunType Type => GunType.TorpedoLauncher;
        public override GunMechanism Mechanism => GunMechanism.SemiAutomatic;
        #endregion

        /// <inheritdoc/>
        protected override void FireAtIndicator(TargetInfo target) {
            //only fire the bullets if the indicator is fulfilled
            if (target.Indicator.IsIndicationFulfilled) {
                IEnumerable<MineGrid> section = (from neighbour in target.Grid.Section
                                                 where neighbour != null && !neighbour.DetonationSystem.IsDetonated && !neighbour.SelectionSystem.IsFlagged
                                                 select neighbour);

                //fire a bullet at each of the neighbours
                if (section.Count() > 0) {
                    Recoil(recoilForce);

                    foreach (MineGrid neighbour in section) {
                        Vector3 neighbourPos = neighbour.Avatar.transform.position;
                        Vector3 neighbourDir = Vector3.Normalize(neighbourPos - transform.position);
                        PullTrigger(neighbourDir, neighbour, false, false, true);
                    }
                }
                else FireAtNull();
            }
            else FireAtNull();
        }

        /// <inheritdoc/>
        protected override void FireAtMine(TargetInfo target) {
            PullTrigger(transform.forward, target.Grid, true, true);
        }

        /// <inheritdoc/>
        protected override void FireAtNull() {
            PullTrigger(transform.forward, null, true, true);
        }

        /// <inheritdoc/>
        protected override void OnGunTriggerStop() {}
    }
}