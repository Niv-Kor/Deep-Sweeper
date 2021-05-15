using System.Collections.Generic;
using UnityEngine;

namespace GamedevUtil
{
    public abstract class Pool<T> : MonoBehaviour, IPool<T> where T : MonoBehaviour
    {
        #region Class Members
        protected Queue<T> pool;
        #endregion

        #region Properties
        public int PoolCount => pool.Count;
        #endregion

        protected virtual void Awake() {
            this.pool = new Queue<T>();
        }

        /// <inheritdoc/>
        public abstract T Make();

        /// <inheritdoc/>
        public void Return(T item) {
            if (!pool.Contains(item)) pool.Enqueue(item);
            item.gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        public T Take() {
            T item;

            if (PoolCount > 0) {
                item = pool.Dequeue();
                item.gameObject.SetActive(true);
            }
            else item = Make();

            return item;
        }

        /// <inheritdoc/>
        public virtual void Insert(int amount) {
            for (int i = 0; i < amount; i++) {
                T item = Make();
                item.gameObject.SetActive(false);
                pool.Enqueue(item);
            }
        }
    }
}