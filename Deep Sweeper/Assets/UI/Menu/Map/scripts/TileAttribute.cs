using UnityEngine;

namespace Menu.Map
{
    public abstract class TileAttribute : MonoBehaviour
    {
        #region Class Members
        protected TileAttributeState m_state;
        #endregion

        #region Properties
        protected abstract TileAttributeState DefaultState { get; }
        public TileAttributeState State {
            get { return m_state; }
            set {
                m_state = value;
                SetState(value);
            }
        }
        #endregion

        protected virtual void Start() {
            this.State = DefaultState;
        }

        /// <summary>
        /// Change the state of the attribute.
        /// </summary>
        /// <param name="state">The new state of the attribute</param>
        protected abstract void SetState(TileAttributeState state);
    }
}