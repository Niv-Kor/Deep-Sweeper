using UnityEngine;

namespace DeepSweeper.UI.Ingame.Diegetics.Sonar
{
    public class MinimapIcon : MonoBehaviour
    {
        #region Constants
        protected static readonly float X_ROTATION = 90;
        protected static readonly float Z_ROTATION = 0;
        #endregion

        #region Class Members
        protected TerrainOutlineRenderer minimapTerrainRenderer;
        protected Vector3 flatMask;
        #endregion

        #region Properties
        public MinimapIconEvents.YawAngle YawAngleFunc { get; set; }
        #endregion

        protected virtual void Start() {
            this.flatMask = new Vector3(X_ROTATION, 0, Z_ROTATION);
            this.minimapTerrainRenderer = TerrainOutlineRenderer.Instance;
            minimapTerrainRenderer.PlaneHeightChangeEvent += SetDefaultHeight;
            SetDefaultHeight();
        }

        protected virtual void Update() {
            float angle = (YawAngleFunc != null) ? YawAngleFunc.Invoke() : 0;
            Flatten(angle);
        }

        private void OnDestroy() {
            minimapTerrainRenderer.PlaneHeightChangeEvent -= SetDefaultHeight;
        }

        /// <summary>
        /// Rotate the icon towards a certain direction, while flattenning it.
        /// </summary>
        /// <param name="yawAngle">The yaw angle into which the icon will rotate</param>
        protected virtual void Flatten(float yawAngle) {
            Vector3 yaw = Vector3.up * (yawAngle);
            transform.rotation = Quaternion.Euler(flatMask + yaw);
        }

        /// <summary>
        /// Set the default height of the icon just above the minimap's terrain plane.
        /// </summary>
        protected void SetDefaultHeight() {
            Vector3 pos = transform.position;
            pos.y = minimapTerrainRenderer.MinimapIconsHeight;
            transform.position = pos;
        }
    }
}