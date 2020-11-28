﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FieldMeta
{
    public abstract class FieldMetaValue : MonoBehaviour
    {
        [Serializable]
        private struct FieldMetaIconColor
        {
            [Tooltip("The entry's difficulty.")]
            [SerializeField] public DifficultyLevel Difficulty;

            [Tooltip("The color of the icon for the corresponding difficulty.")]
            [SerializeField] public Color Color;
        }

        #region Exposed Editor Parameters
        [Header("Prefabs")]
        [Tooltip("The value's text component.")]
        [SerializeField] private Text value;

        [Tooltip("Color configurations of the icon.")]
        [SerializeField] private List<FieldMetaIconColor> iconColors;
        #endregion

        #region Constants
        private static readonly Color TEXT_COLOR = Color.white;
        #endregion

        #region Class Members
        protected Image icon;
        protected MineField field;
        protected Color defaultIconColor;
        #endregion

        #region Properties
        public string Value {
            get { return value.text; }
            set { this.value.text = value; }
        }
        #endregion

        protected virtual void Awake() {
            this.icon = GetComponent<Image>();
            this.defaultIconColor = icon.color;
            this.field = GameFlow.Instance.CurrentPhase.Field;
        }

        /// <param name="difficulty">Selected difficulty</param>
        /// <returns>The corresponding meta value of the current field.</returns>
        protected abstract string GetFieldValue(DifficultyLevel difficulty);

        /// <param name="difficulty">A level difficulty</param>
        /// <returns>The configuration of the specified difficulty in the current phase.</returns>
        protected DifficultyConfig GetCurrentConfig(DifficultyLevel difficulty) {
            return GameFlow.Instance.CurrentPhase.Config.Levels.Find(x => x.Difficulty == difficulty);
        }

        /// <summary>
        /// Update the meta value and color.
        /// </summary>
        /// <param name="difficulty">Selected difficulty level</param>
        public void UpdateValue(DifficultyLevel difficulty) {
            Value = GetFieldValue(difficulty);
            FieldMetaIconColor colorConfig = iconColors.Find(x => x.Difficulty == difficulty);
            Color color = (colorConfig.Difficulty == difficulty) ? colorConfig.Color : defaultIconColor;
            value.color = TEXT_COLOR;
            icon.color = color;
        }
    }
}