namespace GamedevUtil
{
    public interface IPool<T>
    {
        /// <summary>
        /// Manually insert a fixed amount of items into the pool.
        /// </summary>
        /// <param name="amount">The amount of items to insert</param>
        void Insert(int amount);

        /// <summary>
        /// Take an item from the pool,
        /// or create one if the pool is empty.
        /// </summary>
        /// <returns>The retrieved item.</returns>
        T Take();

        /// <summary>
        /// Return an item to the pool.
        /// </summary>
        /// <param name="item">The item to return</param>
        void Return(T item);

        /// <summary>
        /// Create an item.
        /// </summary>
        /// <returns>The newly created item.</returns>
        T Make();
    }
}