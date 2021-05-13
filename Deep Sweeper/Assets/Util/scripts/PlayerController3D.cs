using UnityEngine;

namespace GamedevUtil.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class PlayerController3D : MonoBehaviour
    {
        #region Class Members
        protected Rigidbody rigidBody;
        protected PlayerController controller;
        protected bool m_movable;
        #endregion

        #region Properties
        public bool IsMovable {
            get { return m_movable; }
            set {
                m_movable = value;
                rigidBody.isKinematic = !value;
            }
        }
        #endregion

        protected virtual void Awake() {
            this.controller = PlayerController.Instance;
            this.rigidBody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Move the player across the 3D scene.
        /// </summary>
        /// <param name="vector">Direction + force vector</param>
        protected abstract void Move(Vector3 vector);
    }
}