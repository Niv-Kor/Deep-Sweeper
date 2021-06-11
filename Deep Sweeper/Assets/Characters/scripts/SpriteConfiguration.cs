using System;
using UnityEngine;

namespace DeepSweeper.Characters
{
    [Serializable]
    public struct SpriteConfiguration
    {
        [Tooltip("The character to which this sprite relates.")]
        [SerializeField] public Persona Character;

        [Tooltip("The character's sprite.")]
        [SerializeField] public Texture Sprite;

        [Tooltip("The orientation of the sprite (direction towards which its body tends).")]
        [SerializeField] public SpriteOrientation Orientation;

        [Tooltip("The offset needed to be applied on the sprite in the designated container.")]
        [SerializeField] public Vector2 Offset;
    }
}