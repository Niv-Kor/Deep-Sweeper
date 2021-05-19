using DeepSweeper.Characters;
using System;
using UnityEngine;

namespace DeepSweeper.Gameplay.UI.Diegetics.Commander
{
    [Serializable]
    public struct ThumbnailModel
    {
        [Tooltip("The character of this commander.")]
        public CharacterPersona Character;

        [Tooltip("The sprite of the commander.")]
        public Texture Sprite;

        [Tooltip("The commander's theme color.")]
        public Color Theme;
    }
}