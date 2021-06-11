using System;
using UnityEngine;

namespace DeepSweeper.Characters
{
    [Serializable]
    public struct CharacterAbilityConfig<T>
    {
        [Tooltip("The character that possesses the ability.")]
        [SerializeField] public Persona Character;

        [Tooltip("Special character's ability.")]
        [SerializeField] public T Ability;
    }
}