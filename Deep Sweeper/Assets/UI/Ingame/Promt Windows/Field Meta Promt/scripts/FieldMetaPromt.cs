using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FieldMeta
{
    public class FieldMetaPromt : PromtWindow
    {
        [Serializable]
        private struct DifficultyEntryButton
        {
            [Tooltip("The entry's difficulty.")]
            [SerializeField] public DifficultyLevel Difficulty;

            [Tooltip("The entry's button object")]
            [SerializeField] public GameObject ButtonObject;
        }

        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The promt's entry buttons.")]
        [SerializeField] private List<DifficultyEntryButton> buttons;

        [Header("Blank Screen")]
        [Tooltip("The time it takes to display a fully blank screen after pressing the start button.")]
        [SerializeField] private float blankScreenTime;

        [Tooltip("The time it takes to start fading the blank screen after it reached the fully blank state.")]
        [SerializeField] private float blankScreenPause;
        #endregion

        #region Class Members
        private FieldMetaValue[] values;
        #endregion

        #region Events
        public event UnityAction PromtClosedEvent;
        #endregion

        private void Start() {
            this.values = GetComponentsInChildren<FieldMetaValue>();

            foreach (DifficultyEntryButton button in buttons) {
                Button buttonCmp = button.ButtonObject.GetComponent<Button>();
                buttonCmp.onClick.AddListener(delegate { UpdatePromtValues(button.Difficulty); });
            }
        }

        /// <summary>
        /// Update each of the meta values in the promt
        /// </summary>
        /// <param name="difficulty">Selected difficulty entry button</param>
        private void UpdatePromtValues(DifficultyLevel difficulty) {
            foreach (FieldMetaValue metaValue in values)
                metaValue.UpdateValue(difficulty);
        }
        
        /// <summary>
        /// Start the phase and close the window.
        /// </summary>
        public void StartPhase() {
            PromtClosedEvent?.Invoke();
            void blankAction() { base.Close(); }
            BlankScreen.Instance.Apply(blankScreenTime, blankScreenPause, blankAction);
            OnConfirm();
        }
    }
}