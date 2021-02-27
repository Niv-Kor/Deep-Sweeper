using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu.Contract
{
    public class DifficultyButtonsManager : MonoBehaviour
    {
        private class ButtonOrderComparer : Comparer<DifficultyButton>
        {
            /// <inheritdoc/>
            public override int Compare(DifficultyButton x, DifficultyButton y) {
                return (x.PlacementOrder < y.PlacementOrder) ? 1 : -1;
            }
        }
        
        private struct ButtonInfo
        {
            public DifficultyButton Button;
            public Vector2 startingPos;
        }

        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The difficulty buttons to manage.")]
        [SerializeField] private List<DifficultyButton> buttons;

        [Header("Position")]
        [Tooltip("The local position of the first button to the left.")]
        [SerializeField] private Vector2 firstButtonPos;

        [Tooltip("The space between two buttons.")]
        [SerializeField] private float space;

        [Header("Scale")]
        [Tooltip("The scale of a selected difficulty button.")]
        [SerializeField] private float selectedScale = 1;

        [Tooltip("The scale of an unselected difficulty button.")]
        [SerializeField] private float unselectedScale = .5f;
        #endregion

        #region Class Members
        private float fromSelectedSpace;
        private ButtonInfo selectedButton;
        private List<ButtonInfo> buttonsInfo;
        #endregion

        private void Start() {
            this.buttonsInfo = new List<ButtonInfo>();
            float scaleDiff = selectedScale - unselectedScale;
            this.fromSelectedSpace = (space + 1) * scaleDiff;

            if (buttons.Count > 0) {
                buttons.Sort(new ButtonOrderComparer());
                InitButtons();
                SelectButton(buttonsInfo[0], true, true);
            }
        }

        /// <summary>
        /// Initialize the buttons in their respectful positions.
        /// </summary>
        private void InitButtons() {
            //find the size of the buttons
            RectTransform rect = buttons[0].GetComponent<RectTransform>();
            Vector2 buttonSize = rect.sizeDelta;
            Vector2 autoMargin = Vector2.right * (space + buttonSize.x);

            for (int i = 0; i < buttons.Count; i++) {
                DifficultyButton button = buttons[i];
                Vector2 position = firstButtonPos + autoMargin * i;
                ButtonInfo info;
                info.Button = button;
                info.startingPos = position;
                buttonsInfo.Add(info);
                button.transform.localPosition = position;
                button.ClickedEvent += OnButtonClicked;
            }
        }

        /// <summary>
        /// Activate when a difficulty button is selected.
        /// </summary>
        /// <param name="order"></param>
        private void OnButtonClicked(int order) {
            ButtonInfo button = buttonsInfo.Find(x => x.Button.PlacementOrder == order);
            int index = buttonsInfo.IndexOf(button);

            //neutralize selected button and select the clicked one
            SelectButton(selectedButton, false);
            SelectButton(button, true);
        }

        /// <summary>
        /// Change the scale and position of a button.
        /// </summary>
        /// <param name="buttonInfo">The button to select or deselect</param>
        /// <param name="flag">True to select the button, or false to deselect it</param>
        /// <param name="instant">True to instantly select the button without animations</param>
        private void SelectButton(ButtonInfo buttonInfo, bool flag, bool instant = false) {
            bool isSelected = buttonInfo.Button == selectedButton.Button;
            bool isUnselected = buttonInfo.Button != selectedButton.Button;
            bool noChange = (isSelected && flag) || (isUnselected && !flag);
            if (noChange) return;

            //change the button's scale
            float scale = flag ? selectedScale : unselectedScale;
            buttonInfo.Button.Scale(scale, instant);

            //translate the other buttons
            var leftButtons = buttonsInfo.FindAll(x => x.Button.PlacementOrder < buttonInfo.Button.PlacementOrder);
            var rightButtons = buttonsInfo.FindAll(x => x.Button.PlacementOrder > buttonInfo.Button.PlacementOrder);
            float innerSpace = flag ? fromSelectedSpace : space;
            Vector2 delta = Vector2.right * fromSelectedSpace;

            //move the left buttons
            foreach (ButtonInfo btn in leftButtons) {
                Vector2 origin = btn.startingPos;
                btn.Button.Translate(origin + delta * -1, instant);
            }

            //move the right buttons
            foreach (ButtonInfo btn in rightButtons) {
                Vector2 origin = btn.startingPos;
                btn.Button.Translate(origin + delta, instant);
            }

            selectedButton = buttonInfo;
        }
    }
}