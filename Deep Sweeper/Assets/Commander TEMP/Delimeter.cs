using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeepSweeper.UI.Ingame.Spatials.Commander
{
    public class Delimeter : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [SerializeField] private RawImage armPrefab;
        #endregion

        #region Class Members
        #endregion

        public void Build(int amount) {
            float degSpace = 360f / amount;

            for (int i = 0; i < amount; i++) {
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