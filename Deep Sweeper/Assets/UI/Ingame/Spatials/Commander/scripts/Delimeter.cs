using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class Delimeter : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("A prefab of a single delimeter arm.")]
        [SerializeField] private RawImage armPrefab;
        #endregion

        /// <summary>
        /// Build the entire delimiter.
        /// </summary>
        /// <param name="division">Amount of sectors in the circle</param>
        public void Build(int division) {
            float degSpace = 360f / division;

            for (int i = 0; i < division; i++) {
                RawImage instance = Instantiate(armPrefab);
                instance.rectTransform.SetParent(transform);
                instance.rectTransform.localPosition = Vector3.zero;
                instance.rectTransform.localScale = Vector3.one;
                instance.rectTransform.localRotation = Quaternion.identity;

                //set degrees
                float deg = degSpace * i;
                instance.rectTransform.Rotate(0, 0, deg);
            }
        }
    }
}