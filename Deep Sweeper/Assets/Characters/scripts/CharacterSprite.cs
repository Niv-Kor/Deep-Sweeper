using System;
using UnityEngine;

namespace DeepSweeper.Characters
{
    [Serializable]
    public struct CharacterSprite
    {
        [Tooltip("The character to which this sprite relates.")]
        [SerializeField] public CharacterPersona Character;

        [Tooltip("The character's sprite.")]
        [SerializeField] public Texture Sprite;

        [Tooltip("The orientation of the sprite (direction towards which its body tends).")]
        [SerializeField] public SpriteOrientation orientation;
    }
}