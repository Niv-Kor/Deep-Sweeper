using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu.Contract
{
    public class TextFlowButton : MonoBehaviour
    {
        private enum ButtonFlow
        {
            Rewind,
            Forward
        }

        #region Exposed Editor Parameters
        [Tooltip("The flow direction of the button.")]
        [SerializeField] private ButtonFlow flow;

        [Tooltip("The color of the button while not touched.")]
        [SerializeField] private Color color;

        [Tooltip("The color of the button when hovered by the cursor.")]
        [SerializeField] private Color hoverColor;
        #endregion

        #region Properties
        #endregion

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}