using DeepSweeper.Menu.Sandbox.Ring;
using UnityEngine;

namespace DeepSweeper.Menu.Sandbox
{
    public class SandboxLevel : MonoBehaviour
    {
        #region Properties
        public LevelRing Ring { get; private set; }
        #endregion

        private void Start() {
            var LevelsMngr = GetComponentInParent<RingsManager>();
            this.Ring = LevelsMngr.CreateRing(transform);
        }
    }
}