using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu.Contract
{
    public class TextFlowButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        private TextBox textBox;
        private Image sprite;
        #endregion

        private void Awake() {
            this.textBox = GetComponentInParent<TextBox>();
            this.sprite = GetComponent<Image>();
            sprite.color = color;
            textBox.PageChangedEvent += OnPageChange;
            textBox.TextLoadedEvent += OnTextLoad;
        }

        /// <inheritdoc/>
        public void OnPointerEnter(PointerEventData eventData) {
            sprite.color = hoverColor;
        }

        /// <inheritdoc/>
        public void OnPointerExit(PointerEventData eventData) {
            sprite.color = color;
        }

        /// <summary>
        /// Activate when the text box's page is changed.
        /// </summary>
        /// <param name="nextPage">The next page (after the change)</param>
        private void OnPageChange(int _, int nextPage) {
            bool frontEdge = flow == ButtonFlow.Forward && nextPage >= textBox.PageCount - 1;
            bool rearEdge = flow == ButtonFlow.Rewind && nextPage <= 0;
            sprite.enabled = !frontEdge && !rearEdge;
        }

        /// <summary>
        /// Activate when a new text had been loaded to the text box.
        /// </summary>
        /// <param name="pages">Amount of pages the text takes</param>
        private void OnTextLoad(string _, int pages) {
            if (flow == ButtonFlow.Rewind) sprite.enabled = false;

            switch (flow) {
                case ButtonFlow.Rewind:
                    sprite.enabled = false;
                    break;

                case ButtonFlow.Forward:
                    sprite.enabled = pages > 1;
                    break;
            }
        }
    }
}