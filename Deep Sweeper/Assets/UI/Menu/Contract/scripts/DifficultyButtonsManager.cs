using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSweeper.Menu.Contract
{
    public class DifficultyButtonsManager : MonoBehaviour
    {
        #region Exposed Editor Parameters
        [Tooltip("The local position of the first button to the left.")]
        [SerializeField] private DifficultyButton defaultButton;
        #endregion

        #region Class Members
        private List<DifficultyButton> buttons;
        private DifficultyButton selectedButton;
        private ContractScreen contract;
        #endregion

        private void Awake() {
            this.contract = GetComponentInParent<ContractScreen>();
        }

        private void Start() {
            var buttonsArr = GetComponentsInChildren<DifficultyButton>();
            this.buttons = new List<DifficultyButton>(buttonsArr);

            if (buttons.Count > 0) {
                foreach (DifficultyButton btn in buttons)
                    btn.ClickedEvent += OnButtonClicked;

                if (buttons.Contains(defaultButton))
                    SelectButton(defaultButton, true, true);
            }
        }

        /// <summary>
        /// Activate when a difficulty button is selected.
        /// This function neutralizes the selected button and selects the clicked one.
        /// </summary>
        /// <param name="order"></param>
        private void OnButtonClicked(DifficultyButton button) {
            if (selectedButton != button) {
                SelectButton(selectedButton, false);
                SelectButton(button, true);
                contract.SetContext(button.Difficulty);
            }
        }

        /// <summary>
        /// Change the scale and position of a button.
        /// </summary>
        /// <param name="button">The button to select or deselect</param>
        /// <param name="flag">True to select the button, or false to deselect it</param>
        /// <param name="instant">True to instantly select the button without animations</param>
        private void SelectButton(DifficultyButton button, bool flag, bool instant = false) {
            bool exists = button != null;
            bool isSelected = button == selectedButton;
            bool noChange = (isSelected && flag) || (!isSelected && !flag);

            if (exists && !noChange) {
                button.Select(flag, instant);
                selectedButton = button;
            }
        }
    }
}