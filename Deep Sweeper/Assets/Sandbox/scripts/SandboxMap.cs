using DeepSweeper.Flow;
using DeepSweeper.Menu.UI.Campaign.Sandbox.Ring;
using UnityEngine.Events;

namespace DeepSweeper.Menu.UI.Campaign.Sandbox
{
    public class SandboxMap : Singleton<SandboxMap>
    {
        #region Events
        /// <param type="Region">The newly selected region</param>
        public event UnityAction<Region> RegionSelectedEvent;
        #endregion

        private void Start() {
            //aggregate ring selection events
            var ringsMngrs = GetComponentsInChildren<RingsManager>();
            foreach (var mngr in ringsMngrs) {
                mngr.RingSelectedEvent += delegate(LevelRing ring) {
                    RegionSelectedEvent?.Invoke(ring.Region);
                };
            }
        }
    }
}