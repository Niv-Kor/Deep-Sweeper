﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FieldMeta
{
    public class FieldMetaPromt : MonoBehaviour
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
        [Tooltip("The promt's entry buttons.")]
        [SerializeField] private List<DifficultyEntryButton> buttons;
        #endregion

        #region Class Members
        private FieldMetaValue[] values;
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
    }
}