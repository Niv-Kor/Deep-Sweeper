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

        [Tooltip("The offset needed to be applied on the sprite in the designated container.")]
        [SerializeField] public Vector2 Offset;
    }
}