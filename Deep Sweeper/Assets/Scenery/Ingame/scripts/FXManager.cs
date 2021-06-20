using UnityEngine;

namespace DeepSweeper.Level
{
    public class FXManager : Singleton<FXManager>
    {
        /// <summary>
        /// Render a temporary effect object under this object.
        /// </summary>
        /// <param name="obj">Temporary effect object</param>
        public void Adopt(Transform obj) {
            obj.SetParent(transform);
            obj.localPosition = Vector3.zero;
        }
    }
}