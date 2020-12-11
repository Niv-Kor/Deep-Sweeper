using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        [Header("Settings")]
        [Tooltip("The difficulty that is initially selected.")]
        [SerializeField] private DifficultyLevel defaultDifficulty = DifficultyLevel.Easy;

        [Header("Blank Screen")]
        [Tooltip("The time it takes to display a fully blank screen after pressing the start button.")]
        [SerializeField] private float blankScreenTime;

        [Tooltip("The time it takes to start fading the blank screen after it reached the fully blank state.")]
        [SerializeField] private float blankScreenPause;
        #endregion

        #region Class Members
        private List<FieldMetaValue> values;
        private DifficultyLevel selectedDifficulty;
        #endregion

        protected override void Awake() {
            base.Awake();

            FieldMetaValue[] valuesCmpArr = GetComponentsInChildren<FieldMetaValue>();
            this.values = new List<FieldMetaValue>(valuesCmpArr);
            this.selectedDifficulty = defaultDifficulty;
        }

        private void Start() {
            foreach (DifficultyEntryButton button in buttons) {
                Button buttonCmp = button.ButtonObject.GetComponent<Button>();
                buttonCmp.onClick.AddListener(delegate { UpdatePromtValues(button.Difficulty); });
            }

            //set the action being triggered by the prompt's confirmation
            FieldMinesMeta minesValueCmp = (FieldMinesMeta) values.Find(x => x is FieldMinesMeta);
            FieldRewardMeta rewardValueCmp = (FieldRewardMeta) values.Find(x => x is FieldRewardMeta);

            WindowConfirmedEvent += delegate {
                int minesAmount = int.Parse(minesValueCmp.Value);
                long reward = long.Parse(rewardValueCmp.Value);

                void OnBlankScreen() {
                    GameFlow.Instance.CurrentPhase.Field.gameObject.SetActive(true);
                    GameFlow.Instance.CurrentPhase.Field.Init(minesAmount, reward);
                }

                void OnTransparentScreen() {
                    GameFlow.Instance.CurrentPhase.Begin(selectedDifficulty);
                }

                BlankScreen.Instance.Apply(blankScreenTime, blankScreenPause, OnBlankScreen, OnTransparentScreen);
            };

            //initially select one of the difficulties
            GameObject defButtonObj = buttons.Find(x => x.Difficulty == defaultDifficulty).ButtonObject;
            Button defButton = defButtonObj?.GetComponent<Button>();
            defButton?.onClick.Invoke();
            EventSystem.current.SetSelectedGameObject(defButtonObj);
        }

        /// <summary>
        /// Update each of the meta values in the promt.
        /// </summary>
        /// <param name="difficulty">Selected difficulty entry button</param>
        private void UpdatePromtValues(DifficultyLevel difficulty) {
            selectedDifficulty = difficulty;

            foreach (FieldMetaValue metaValue in values)
                metaValue.UpdateValue(difficulty);
        }
    }
}